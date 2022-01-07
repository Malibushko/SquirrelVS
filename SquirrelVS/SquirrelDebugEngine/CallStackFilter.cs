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
        {
          CallstackData.Callstack.Clear();
          CallstackData.ThreadStack.Clear();
        }

        return null;
      }

      if (_NativeFrame.InstructionAddress?.ModuleInstance == null)
        return new DkmStackWalkFrame[1] { _NativeFrame };

      if (_NativeFrame.ModuleInstance != null && _NativeFrame.ModuleInstance.Name == "SquirrelDebugHelper.dll")
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

      DkmProcess        Process   = _StackContext.InspectionSession.Process;
      LocalProcessData  LocalData = Utility.GetOrCreateDataItem<LocalProcessData>(Process);
      SquirrelCallStack Callstack = Utility.GetOrCreateDataItem<SquirrelCallStack>(Process);

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
          if (Callstack.ThreadStack.Count == 0 || !Callstack.ThreadStack.Any((VM => VM.Address == ThreadHandle.Value)))
          {
            Callstack.ThreadStack.Push(new SQVM(Process, ThreadHandle.Value));

            new LocalComponent.FetchCallstackRequest
            {
              SquirrelHandle = ThreadHandle.Value
            }.SendHigher(Process);
          }

          return GetNextSquirrelFrames(Process, _NativeFrame, _StackContext, Callstack.ThreadStack.Peek(), (MethodName != "sq_call"));
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
      var Callstack           = CallstackDataHolder.Callstack;
      var SquirrelFrames      = new List<DkmStackWalkFrame>();

      foreach (var CallFrame in Callstack)
      {
        if (CallFrame.Thread != _Thread)
          continue;

        if (CallFrame.ParentFrameBase == 0)
        {
          if (CallFrame.IsClosure())
            CallFrame.ParentFrameBase = _NativeFrame.FrameBase;
        }

        if (CallFrame.ParentFrameBase != _NativeFrame.FrameBase)
          continue;

        if (CallFrame.IsNativeClosure())
        {
          var NativeClosure = CallFrame.Closure.Value as SQNativeClosure;

          if (HelperLocations.ModuleAddresses.In(NativeClosure.Function.Read()))
            continue;

          break;
        };

        DkmInstructionAddress InstructionAddress = DkmCustomInstructionAddress.Create(
            ProcessData.RuntimeInstance,
            ProcessData.ModuleInstance,
            new SourceLocation { Source = CallFrame.SourceName, Line = CallFrame.Line }.Encode(),
            0,
            null,
            null
          );

        CallstackDataHolder.GetFrameStackBase(CallFrame, _Thread.StackBase.Read());

        SquirrelFrames.Add(DkmStackWalkFrame.Create(
            _StackContext.Thread,
            InstructionAddress,
            _NativeFrame.FrameBase,
            _NativeFrame.FrameSize,
            SquirrelFrameFlags,
            CallFrame.FrameName,
            _NativeFrame.Registers,
            _NativeFrame.Annotations,
            null,
            null,
            DkmStackWalkFrameData.Create(
                _StackContext.InspectionSession,
                new SquirrelStackFrameData
                {
                  NativeFrame = CallFrame
                }
              )
            )
          );
      }

      if (_KeepNativeFrame)
        SquirrelFrames.Add(_NativeFrame);

      return SquirrelFrames.ToArray();
    }
  }
}