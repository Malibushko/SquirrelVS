using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;

namespace SquirrelDebugEngine
{
  internal class CallStackFilter : DkmDataItem
  {
    public DkmStackWalkFrame[] FilterNextFrame(
        DkmStackContext   _StackContext,
        DkmStackWalkFrame _NativeFrame
      )
    {
      if (_NativeFrame?.InstructionAddress?.ModuleInstance == null)
        return new DkmStackWalkFrame[1] { _NativeFrame };

      if (_NativeFrame.ModuleInstance != null && _NativeFrame.ModuleInstance.Name == "SquirrelDebugHelper.dll" && false)
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

      DkmProcess       Process          = _StackContext.InspectionSession.Process;
      LocalProcessData ProcessData      = Utility.GetOrCreateDataItem<LocalProcessData>(Process);

      string MethodName = GetFrameMethodName(_NativeFrame);

      if (MethodName == null)
        return new DkmStackWalkFrame[1] { _NativeFrame };

      if (MethodName == "sq_call")
      {
        if (ProcessData.RuntimeInstance == null)
        {
          ProcessData.RuntimeInstance = Process
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

        var Callstack      = Utility.GetOrCreateDataItem<SquirrelCallStack>(Process).Callstack;
        var SquirrelFrames = new List<DkmStackWalkFrame>();

        int IndexFromTop = 0;

        foreach (var Call in Callstack)
        {
          DkmInstructionAddress InstructionAddress = DkmCustomInstructionAddress.Create(
              ProcessData.RuntimeInstance,
              ProcessData.ModuleInstance,
              new SourceLocation { Source = Call.SourceName, Line = Call.Line }.Encode(),
              0,
              null,
              null
            );

          SquirrelFrames.Add(DkmStackWalkFrame.Create(
              _StackContext.Thread,
              InstructionAddress,
              _NativeFrame.FrameBase,
              _NativeFrame.FrameSize,
              SquirrelFrameFlags,
              // TODO: Add args to callstack frame
              $"[{Call.FunctionName}(), line {Call.Line}]",
              _NativeFrame.Registers,
              _NativeFrame.Annotations,
              null,
              null,
              DkmStackWalkFrameData.Create(
                  _StackContext.InspectionSession, 
                  new SquirrelStackFrameData 
                  { 
                    ParentFrame  = _NativeFrame, 
                    IndexFromTop = IndexFromTop++ 
                  }
                )
              )
            );
        }

        return SquirrelFrames.ToArray();
      }
      else
      if (MethodName.StartsWith("SQVM") || MethodName.StartsWith("sq_") || MethodName.StartsWith("sqstd_"))
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
  }
}