using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Exceptions;
using Microsoft.VisualStudio.Debugger.Stepping;
using Microsoft.VisualStudio.Debugger.Symbols;
using System;
using System.Text;
using System.Linq;

namespace SquirrelDebugEngine
{
  public enum StepperState
  {
    None,
    StepInto,
    StepOver,
    StepOut
  };

  public class RemoteComponent : IDkmCustomMessageForwardReceiver, IDkmRuntimeBreakpointReceived, IDkmRuntimeMonitorBreakpointHandler, IDkmRuntimeStepper
  {
    void IDkmRuntimeBreakpointReceived.OnRuntimeBreakpointReceived(
        DkmRuntimeBreakpoint _Breakpoint, 
        DkmThread            _Thread,
        bool                 _HasException,
        DkmEventDescriptorS  _EventDescriptior
      )
    {
      try
      {
        DkmProcess          Process     = _Thread.Process;
        RemoteProcessData   ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(Process);
        SquirrelBreakpoints Breakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(Process);

        if (_Breakpoint.UniqueId == Breakpoints.SquirrelHelperAsyncBreak)
        {
          _EventDescriptior.Suppress();

          var BreakpointIndex = Utility.ReadUlongVariable(Process, Breakpoints.SquirrelHitBreakpointIndexAddress);
          
          if (BreakpointIndex.HasValue)
          {
            if (BreakpointIndex.Value < (ulong)ProcessData.ActiveBreakpoints.Count && BreakpointIndex.Value != ulong.MaxValue)
            {
              if (ProcessData.ActiveBreakpoints[(int)BreakpointIndex.Value].Breakpoint != null)
              {
                ProcessData.ActiveBreakpoints[(int)BreakpointIndex.Value].Breakpoint.OnHit(_Thread, false);
                
                Utility.TryWriteUlongVariable(Process, Breakpoints.SquirrelHitBreakpointIndexAddress, ulong.MaxValue);

                return;
              }
              else 
              if (ProcessData.ActiveStepper != null)
              {
                // Stepper breakpoint
                StepperState State = (StepperState)Utility.ReadIntVariable(Process, Breakpoints.StepperStateAddress).GetValueOrDefault(0);

                if (State != StepperState.None)
                {
                  Utility.TryWriteIntVariable(Process, Breakpoints.StepperStateAddress, (int)StepperState.None);

                  ProcessData.ActiveStepper.OnStepComplete(_Thread, false);

                  ProcessData.ActiveStepper = null;
                }
              }
            }
          }
        }

        _Thread.GetCurrentFrameInfo(out ulong _ReturnAddress, out ulong _FrameBase, out ulong _VFrame);

        BreakpointHitData Data = new BreakpointHitData
        {
          BreakpointID = _Breakpoint.UniqueId,
          ThreadID = _Thread.UniqueId,
          ReturnAddress = _ReturnAddress,
          FrameBase = _FrameBase,
          vFrame = _VFrame
        };

        DkmCustomMessage.Create(
              _Thread.Process.Connection,
              _Thread.Process,
              MessageToLocal.Guid,
              (int)MessageToLocal.MessageType.BreakpointHit,
              Data.Encode(),
              null
            ).SendHigher();
      }
      catch (Exception Exception)
      {
        DkmCustomMessage.Create(
          _Thread.Process.Connection,
          _Thread.Process,
          MessageToLocal.Guid,
          (int)MessageToLocal.MessageType.ComponentException,
          Encoding.UTF8.GetBytes(Exception.Message),
          null
        ).SendHigher();
      }
    }

    #region Interface
    public DkmCustomMessage SendLower(
        DkmCustomMessage _Message
      )
    {
      DkmProcess        Process     = _Message.Process;
      RemoteProcessData ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(Process);

      try
      {
        switch ((MessageToRemote.MessageType)_Message.MessageCode)
        {
          case MessageToRemote.MessageType.CreateRuntime:
            {
              CreateRuntime(Process, ProcessData);
              break;
            }
          case MessageToRemote.MessageType.BreakpointsInfo:
            {
              SquirrelBreakpoints BreakpoinsData = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(Process);

              BreakpoinsData.ReadFrom(_Message.Parameter1 as byte[]);

              break;
            }

          case MessageToRemote.MessageType.RegisterState:
            {
              HookData Data = new HookData();
              
              Data.Decode(_Message.Parameter1 as byte[]);

              SetupHooks(Process, Data);

              break;
            }

          case MessageToRemote.MessageType.Locations:
          {
            if (ProcessData.Locations == null)
              ProcessData.Locations = new SquirrelLocations();

            ProcessData.Locations.ReadFrom(_Message.Parameter1 as byte[]);
            break;
          }
          default:
            throw new Exception("Invalid message code");
        }
      }
      catch (Exception Exception)
      {
        DkmCustomMessage.Create(
          Process.Connection,
          Process,
          MessageToLocal.Guid,
          (int)MessageToLocal.MessageType.ComponentException,
          Encoding.ASCII.GetBytes(Exception.Message),
          null).SendHigher();
      }

      return null;
    }

    #endregion

    #region Service
    private void CreateRuntime(
        DkmProcess        _Process,
        RemoteProcessData _ProcessData
      )
    {
      if (_ProcessData.Language == null)
      {
        _ProcessData.CompilerID = new DkmCompilerId(Guids.SquirrelCompilerID, Guids.SquirrelLanguageID);
        _ProcessData.Language   = DkmLanguage.Create("Squirrel", _ProcessData.CompilerID);
      }

      if (_ProcessData.RuntimeInstance == null)
      {
        DkmRuntimeInstanceId RuntimeID = new DkmRuntimeInstanceId(Guids.SquirrelRuntimeID, 0);

        _ProcessData.RuntimeInstance = DkmCustomRuntimeInstance.Create(_Process, RuntimeID, null);
      }

      if (_ProcessData.Module == null)
      {
        DkmModuleId ModuleID = new DkmModuleId(Guid.NewGuid(), Guids.SquirrelSymbolProviderID);

        _ProcessData.Module = DkmModule.Create(ModuleID, "sq.vm.code", _ProcessData.CompilerID, _Process.Connection, null);
      }

      if (_ProcessData.ModuleInstance == null)
      {
        DkmDynamicSymbolFileId SymbolFileID = DkmDynamicSymbolFileId.Create(Guids.SquirrelSymbolProviderID);
        _ProcessData.ModuleInstance = DkmCustomModuleInstance.Create(
              "sq_vm",
              "sq.vm.code",
              0,
              _ProcessData.RuntimeInstance,
              null,
              SymbolFileID,
              DkmModuleFlags.None,
              DkmModuleMemoryLayout.Unknown,
              0,
              1,
              0,
              "Squirrel VM Code",
              false,
              null,
              null,
              null
            );

          _ProcessData.ModuleInstance.SetModule(_ProcessData.Module, true);
      }
    }

    private void SetupHooks(
        DkmProcess _Process, 
        HookData   _Data
      )
    {
      Utility.TryWritePointerVariable(_Process, _Data.DebugHookNativeAddress, _Data.HelperHookAddress);
      Utility.TryWriteByteVariable(_Process, _Data.DebugHookNative, 1);
    }

    #endregion
    
    #region RuntimeMonitorHandler

    void IDkmRuntimeMonitorBreakpointHandler.EnableRuntimeBreakpoint(
        DkmRuntimeBreakpoint _Breakpoint
      )
    {
      DkmProcess        Process     = _Breakpoint.Process;
      RemoteProcessData ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(Process);

      var RuntimeInstructionBreakpoint = _Breakpoint as DkmRuntimeInstructionBreakpoint;

      if (RuntimeInstructionBreakpoint == null)
        return;

      var InstructionAddress = RuntimeInstructionBreakpoint.InstructionAddress as DkmCustomInstructionAddress;

      if (InstructionAddress == null)
        return;

      BreakpointData Break = new BreakpointData();

      Break.ReadFrom(InstructionAddress.EntityId.ToArray());
      Break.Breakpoint = _Breakpoint;

      ProcessData.ActiveBreakpoints.Add(Break);

      UpdateBreakpoints(Process, ProcessData);
    }

    void IDkmRuntimeMonitorBreakpointHandler.TestRuntimeBreakpoint(
        DkmRuntimeBreakpoint _Breakpoint
      )
    {
      // Empty
    }

    void IDkmRuntimeMonitorBreakpointHandler.DisableRuntimeBreakpoint(
        DkmRuntimeBreakpoint _Breakpoint
      )
    {
      DkmProcess        Process     = _Breakpoint.Process;
      RemoteProcessData ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(Process);

      var RuntimeInstructionBreakpoint = _Breakpoint as DkmRuntimeInstructionBreakpoint;

      if (RuntimeInstructionBreakpoint == null)
        return;

      var InstructionAddress = RuntimeInstructionBreakpoint.InstructionAddress as DkmCustomInstructionAddress;

      if (InstructionAddress == null)
        return;

      BreakpointData Break = new BreakpointData();
      Break.ReadFrom(InstructionAddress.EntityId.ToArray());

      ProcessData.ActiveBreakpoints.Remove(Break);

      UpdateBreakpoints(Process, ProcessData);
    }

    void UpdateBreakpoints(
        DkmProcess        _Process, 
        RemoteProcessData _ProcessData
      )
    {
      var BreakpointsInfo = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);

      int BreakpointsCount = _ProcessData.ActiveBreakpoints.Count;

      if (BreakpointsCount > 256)
        BreakpointsCount = 256;

      ulong StringBufferAddress = BreakpointsInfo.SquirrelBreakpointsBufferAddress;

      Utility.TryWriteUlongVariable(_Process, BreakpointsInfo.SquirrelActiveBreakpointsCountAddress, (ulong)BreakpointsCount);

      for (int i = 0; i < BreakpointsCount; i++)
      {
        ulong DataAddress = BreakpointsInfo.SquirrelActiveBreakpointsAddress + (ulong)i * 32;
        var Breakpoint    = _ProcessData.ActiveBreakpoints[i];
        byte[] Encoded    = Encoding.BigEndianUnicode.GetBytes(Breakpoint.SourceName);

        // Write Type
        Utility.TryWriteUlongVariable(_Process, DataAddress, 0);
        
        // Write file name to buffer
        Utility.TryWriteRawBytes(_Process, StringBufferAddress, Encoded);
        Utility.TryWriteByteVariable(_Process, StringBufferAddress + (ulong)Encoded.Length, 0);

        // Write to pointer
        Utility.TryWritePointerVariable(_Process, DataAddress + sizeof(ulong), StringBufferAddress);

        StringBufferAddress += (ulong)Encoded.Length + 1;

        // Write Line
        Utility.TryWriteUlongVariable(_Process, DataAddress + sizeof(ulong) * 2, Breakpoint.Line);

        // Write function (zero for now)
        Utility.TryWritePointerVariable(_Process, DataAddress + sizeof(ulong) * 3, 0);
      }
    }
    #endregion

    #region Runtime Stepper

    void IDkmRuntimeStepper.BeforeEnableNewStepper(
        DkmRuntimeInstance _RuntimeInstance, 
        DkmStepper         _Stepper
      )
    {
      // Empty
    }

    bool IDkmRuntimeStepper.OwnsCurrentExecutionLocation(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason
      )
    {
      var ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(_RuntimeInstance.Process);

      if (_Stepper.StartingAddress == null)
        return false;

      var InstructionAddress = _Stepper.StartingAddress.CPUInstructionPart.InstructionPointer;

      if (ProcessData.Locations.HelperStartAddress >= InstructionAddress && InstructionAddress < ProcessData.Locations.HelperEndAddress)
        return true;

      if (ProcessData.Locations.CallStartLocation >= InstructionAddress && InstructionAddress < ProcessData.Locations.CallEndLocation)
        return true;

      return false;
    }

    void IDkmRuntimeStepper.Step(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason
      )
    {
      if (_Stepper.StepKind == DkmStepKind.StepIntoSpecific)
        throw new NotSupportedException();

      DkmProcess          Process     = _RuntimeInstance.Process;
      RemoteProcessData   ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(Process);
      SquirrelBreakpoints Breakpoitns = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(Process);

      if (ProcessData.ActiveStepper != null)
      {
        ProcessData.ActiveStepper.CancelStepper(_RuntimeInstance);

        ClearStepperData(Process, ProcessData);
      }

      StepperState State = StepperState.None;

      switch (_Stepper.StepKind)
      {
        case DkmStepKind.Into:
          State = StepperState.StepInto;
          break;
        case DkmStepKind.Out:
          State = StepperState.StepOut;
          break;
        case DkmStepKind.Over:
          State = StepperState.StepOver;
          break;
      }

      ProcessData.ActiveStepper = _Stepper;

      Utility.TryWriteIntVariable(Process, Breakpoitns.StepperStateAddress, (int)State);
    }

    void IDkmRuntimeStepper.StopStep(
        DkmRuntimeInstance _RuntimeInstance, 
        DkmStepper         _Stepper
      )
    {
      ClearStepperData(_RuntimeInstance.Process, Utility.GetOrCreateDataItem<RemoteProcessData>(_RuntimeInstance.Process));
    }

    void IDkmRuntimeStepper.AfterSteppingArbitration(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _NewControllingRuntimeInstance
      )
    {
      // Empty
    }

    void IDkmRuntimeStepper.OnNewControllingRuntimeInstance(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _ControllingRuntimeInstance
      )
    {
      ClearStepperData(_RuntimeInstance.Process, Utility.GetOrCreateDataItem<RemoteProcessData>(_RuntimeInstance.Process));
    }

    bool IDkmRuntimeStepper.StepControlRequested(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _CallingRuntimeInstance
      )
    {
      return true;
    }

    void IDkmRuntimeStepper.TakeStepControl(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        bool                     _LeaveGuardsInPlace, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _CallingRuntimeInstance
      )
    {
      ClearStepperData(_RuntimeInstance.Process, Utility.GetOrCreateDataItem<RemoteProcessData>(_RuntimeInstance.Process));
    }

    void IDkmRuntimeStepper.NotifyStepComplete(
        DkmRuntimeInstance _RuntimeInstance, 
        DkmStepper         _Stepper
      )
    {
      // Empty
    }

    void ClearStepperData(
        DkmProcess        _Process,
        RemoteProcessData _ProcessData
      )
    {
      var BreakInfo = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);

      Utility.TryWriteIntVariable(_Process, BreakInfo.StepperStateAddress, (int)StepperState.None);

      _ProcessData.ActiveStepper = null;
    }

    #endregion
  }
}
