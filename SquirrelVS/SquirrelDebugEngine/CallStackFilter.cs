using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using SquirrelDebugEngine.Proxy;

namespace SquirrelDebugEngine
{
  internal class CallStackFilter : DkmDataItem
  {
    public DkmStackWalkFrame[] FilterNextFrame(
        DkmStackContext   _StackContext,
        DkmStackWalkFrame _NativeFrame
      )
    {
      if (_NativeFrame == null) // End of stack
      {
        var CallstackData = _StackContext.Thread.Process.GetDataItem<SquirrelCallStack>();
        
        if (CallstackData != null)
          CallstackData.ActiveThreads.Clear();

        return null;
      }

      if (_NativeFrame.InstructionAddress?.ModuleInstance == null)
        return new DkmStackWalkFrame[1] { _NativeFrame };

      if (_NativeFrame.ModuleInstance      != null &&
          _NativeFrame.ModuleInstance.Name == "SquirrelDebugHelper.dll")
      {
        return new DkmStackWalkFrame[1] { DkmStackWalkFrame.Create(
            _StackContext.Thread,
            _NativeFrame.InstructionAddress,
            _NativeFrame.FrameBase,
            _NativeFrame.FrameSize,
            DkmStackWalkFrameFlags.NonuserCode | DkmStackWalkFrameFlags.Hidden,
            "[Squirrel Debugger Helper]",
            _NativeFrame.Registers,
            _NativeFrame.Annotations
          ) };
      }

      DkmProcess        Process             = _StackContext.InspectionSession.Process;
      SquirrelCallStack CallstackDataHolder = Utility.GetOrCreateDataItem<SquirrelCallStack>(Process);

      string MethodName = GetFrameMethodName(_NativeFrame);

      if (MethodName == null)
        return new DkmStackWalkFrame[1] { _NativeFrame };

      if (MethodName  == "sq_wakeupvm" || 
          MethodName  == "sq_call")
      {
        var ThreadHandle = EvaluationHelpers.TryEvaluateAddressExpression(
            "v",
            _StackContext.InspectionSession,
            _StackContext.Thread,
            _NativeFrame,
            Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects
          );

        if (ThreadHandle.HasValue)
        {
          var Thread = new SQVM(Process, ThreadHandle.Value);

          if (!CallstackDataHolder.ActiveThreads.ContainsKey(Thread))
          {
            CallstackDataHolder.ActiveThreads[Thread] = new SquirrelThreadData
            {
              Callstack         = FetchThreadCallstack(Process, Thread),
              LastFramePosition = 0
            };
          }

          return GetNextSquirrelFrames(Process, _NativeFrame, _StackContext, Thread, true);
        }
      }

      if (MethodName == "sq_suspendvm")
      {

      }

      if (MethodName == "sq_resume")
      {
        // TODO: Add support for generators
      }
      

      if (MethodName.StartsWith("SQVM"))
      {
        var Flags = (_NativeFrame.Flags & ~DkmStackWalkFrameFlags.UserStatusNotDetermined) | DkmStackWalkFrameFlags.NonuserCode;

        Flags |= DkmStackWalkFrameFlags.Hidden;

        return new DkmStackWalkFrame[1] {
          DkmStackWalkFrame.Create(
              _StackContext.Thread,
              _NativeFrame.InstructionAddress,
              _NativeFrame.FrameBase,
              _NativeFrame.FrameSize,
              Flags,
              _NativeFrame.Description,
              _NativeFrame.Registers,
              _NativeFrame.Annotations
            ) };
      }

      return new [] { _NativeFrame };
    }

    private string GetFrameMethodName(
        DkmStackWalkFrame _Input
      )
    {
      string MethodName = null;

      if (_Input.BasicSymbolInfo != null)
        MethodName = _Input.BasicSymbolInfo.MethodName;

      return MethodName;
    }

    private DkmStackWalkFrame[] GetNextSquirrelFrames(
          DkmProcess        _Process,
          DkmStackWalkFrame _NativeFrame,
          DkmStackContext   _StackContext,
          SQVM              _Thread,
          bool              _KeepNativeFrame
        )
    {
      LocalProcessData ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);

      if (ProcessData.RuntimeInstance == null)
      {
        ProcessData.RuntimeInstance = _Process
                                        .GetRuntimeInstances()
                                        .OfType<DkmCustomRuntimeInstance>()
                                        .FirstOrDefault(el => el.Id.RuntimeType == Guids.SquirrelRuntimeID);

        if (ProcessData.RuntimeInstance == null)
          return new DkmStackWalkFrame[1] { _NativeFrame };

        ProcessData.ModuleInstance = ProcessData.RuntimeInstance.GetModuleInstances().OfType<DkmCustomModuleInstance>().FirstOrDefault(el => el.Module != null && el.Module.CompilerId.VendorId == Guids.SquirrelCompilerID);

        if (ProcessData.ModuleInstance == null)
          return new DkmStackWalkFrame[1] { _NativeFrame };
      }

      var SquirrelFrameFlags = _NativeFrame.Flags;

      SquirrelFrameFlags &= ~(DkmStackWalkFrameFlags.NonuserCode | DkmStackWalkFrameFlags.UserStatusNotDetermined);

      if ((_NativeFrame.Flags | DkmStackWalkFrameFlags.TopFrame) != 0)
        SquirrelFrameFlags |= DkmStackWalkFrameFlags.TopFrame;

      var HelperLocations     = Utility.GetOrCreateDataItem<LocalComponent.HelperLocationsDataHolder>(_Process);
      var CallstackDataHolder = Utility.GetOrCreateDataItem<SquirrelCallStack>(_Process);
      var ThreadData          = CallstackDataHolder.ActiveThreads[_Thread];
      var Callstack           = ThreadData.Callstack;
      var SquirrelFrames      = new List<DkmStackWalkFrame>();

      long? ForcedLine       = null;
      bool  SkipNextFrame    = false;
      bool  HitHelperHook    = false;
      bool  HitNativeClosure = false;

      for (int i = 0; i < Callstack.Count; i++)
      {
        var Frame = Callstack[i];

        if (Frame.ParentFrameBase == 0)
          Frame.ParentFrameBase = _NativeFrame.FrameBase;

        if (Frame.ParentFrameBase != _NativeFrame.FrameBase)
          continue;

        if (Frame.IsNativeClosure())
        {
          var NativeClosure = Frame.Closure.Value as SQNativeClosure;

          if (HelperLocations.ModuleAddresses.In(NativeClosure.Function.Read()))
          {
            if (HelperLocations.LastType.Read() == (byte)'r')
              SkipNextFrame = true;
            else
              ForcedLine = HelperLocations.LastLine.Read();

            HitHelperHook = true;
          }
          else
          {
            if (HitNativeClosure == true || HitHelperHook == true)
            {
              Frame.ParentFrameBase = 0;
              break;
            }

            HitNativeClosure = true;
          }

          continue;
        }

        if (SkipNextFrame)
        {
          SkipNextFrame = false;
          continue;
        }

        if (ForcedLine.HasValue)
        {
          Frame.ForcedLine = ForcedLine.Value;

          ForcedLine = null;
        }

        DkmInstructionAddress InstructionAddress = DkmCustomInstructionAddress.Create(
            ProcessData.RuntimeInstance,
            ProcessData.ModuleInstance,
            new SourceLocation { Source = Frame.SourceName, Line = Frame.Line }.Encode(),
            0,
            null,
            null
          );

        CallstackDataHolder.GetFrameStackBase(Frame, _Thread.StackBase.Read());

        SquirrelFrames.Add(DkmStackWalkFrame.Create(
            _StackContext.Thread,
            InstructionAddress,
            _NativeFrame.FrameBase,
            _NativeFrame.FrameSize,
            SquirrelFrameFlags,
            Frame.FrameName,
            _NativeFrame.Registers,
            _NativeFrame.Annotations,
            null,
            null,
            DkmStackWalkFrameData.Create(
                _StackContext.InspectionSession,
                new SquirrelStackFrameData
                {
                  NativeFrame = Frame
                }
              )
            )
          );
      }

      if (_KeepNativeFrame)
        SquirrelFrames.Add(_NativeFrame);

      return SquirrelFrames.ToArray();
    }
  
    private List<CallstackFrame> FetchThreadCallstack(
        DkmProcess _Process,
        SQVM       _Thread
      )
    {
      List<CallstackFrame> Callstack = new List<CallstackFrame>();

      if (_Thread?.Address == 0)
        return Callstack;

      long CallStackSize    = _Thread.CallStackSize.Read();
      var  CallstackPointer = _Thread.CallStack;

      if (CallstackPointer.IsNull)
        return Callstack;

      for (long i = 0; i < CallStackSize; ++i)
      {
        Callstack.Add(new CallstackFrame(CallstackPointer.Read()[i])
        {
          Thread = _Thread
        });
      }

      Callstack.Reverse();

      return Callstack;
    }
  }
}