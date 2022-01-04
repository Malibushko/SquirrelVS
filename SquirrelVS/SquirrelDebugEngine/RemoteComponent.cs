using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Exceptions;
using Microsoft.VisualStudio.Debugger.Stepping;
using Microsoft.VisualStudio.Debugger.Symbols;
using Microsoft.VisualStudio.Debugger.Native;
using Microsoft.VisualStudio.Debugger.CallStack;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Diagnostics;
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SquirrelDebugEngine.Proxy;

namespace SquirrelDebugEngine
{
  public class RemoteComponent : 
    IComponentBase,
    IDkmRuntimeBreakpointReceived, 
    IDkmRuntimeMonitorBreakpointHandler, 
    IDkmRuntimeStepper
  {
    private static readonly uint InvalidStepKind = uint.MaxValue;

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct BreakpointData
    {
      public ulong SourceName;
      public Int64 Line;
      public ulong SourceLength;
    };

    private class BreakpointsGuidsDataHolder : DkmDataItem
    {
      public Guid BreakpointHitGuid;
      public Guid StepCompleteGuid;
      public Guid StepFallthroughGuid;
    }

    private class LocationsDataHolder : DkmDataItem
    {
      public AddressRange       HelperLocation;
      public List<AddressRange> SquirrelLocations;

      public ulong StringBufferAddress;
      public ulong ActiveBreakpointsAddress;
    }

    private class StepperDataHolder : DkmDataItem
    {
      public DkmStepper  ActiveStepper;
      public UInt32Proxy ActiveStepKind;
    }

    private class ActiveBreakpointsDataHolder : DkmDataItem
    {
      public UInt64Proxy                                       HitBreakpointIndex;
      public UInt64Proxy                                       ActiveBreakpointsSize;
      public List<Tuple<SourceLocation, DkmRuntimeBreakpoint>> ActiveBreakpoints = new List<Tuple<SourceLocation, DkmRuntimeBreakpoint>>();
    }

    public RemoteComponent() : base(Guids.SquirrelRemoteComponentGuid)
    {
      // Empty
    }

    #region IDkmRuntimeBreakpointReceived
    void IDkmRuntimeBreakpointReceived.OnRuntimeBreakpointReceived(
        DkmRuntimeBreakpoint _Breakpoint, 
        DkmThread            _Thread,
        bool                 _HasException,
        DkmEventDescriptorS  _EventDescriptior
      )
    {
      var Process           = _Thread.Process;
      var HelperBreakpoints = Utility.GetOrCreateDataItem<BreakpointsGuidsDataHolder>(Process);

      if (_Breakpoint.UniqueId == HelperBreakpoints.BreakpointHitGuid ||
          _Breakpoint.UniqueId == HelperBreakpoints.StepCompleteGuid  || 
          _Breakpoint.UniqueId == HelperBreakpoints.StepFallthroughGuid)
      {
        _EventDescriptior.Suppress();

        if (_Breakpoint.UniqueId == HelperBreakpoints.BreakpointHitGuid)
        {
          OnBreakpointHit(_Thread);
        }
        else
        if (_Breakpoint.UniqueId == HelperBreakpoints.StepCompleteGuid)
        {
          OnStepCompleted(_Thread);
        }
        else
        if (_Breakpoint.UniqueId == HelperBreakpoints.StepFallthroughGuid)
        {
          OnStepFallthrough(_Thread);
        }

        return;
      }

      _Thread.GetCurrentFrameInfo(out ulong _ReturnAddress, out ulong _FrameBase, out ulong _VFrame);

      new LocalComponent.HandleBreakpointRequest()
      {
        BreakpointID  = _Breakpoint.UniqueId,
        ThreadID      = _Thread.UniqueId,
        ReturnAddress = _ReturnAddress,
        FrameBase     = _FrameBase,
        VFrame        = _VFrame
      }.SendHigher(Process);
    }
    #endregion

    #region IDkmRuntimeMonitorBreakpointHandler

    void IDkmRuntimeMonitorBreakpointHandler.EnableRuntimeBreakpoint(
        DkmRuntimeBreakpoint _Breakpoint
      )
    {
      var RuntimeInstructionBreakpoint = _Breakpoint as DkmRuntimeInstructionBreakpoint;

      if (RuntimeInstructionBreakpoint == null)
        return;

      var InstructionAddress = RuntimeInstructionBreakpoint.InstructionAddress as DkmCustomInstructionAddress;

      if (InstructionAddress == null)
        return;
      
      ActiveBreakpointsDataHolder Data = Utility.GetOrCreateDataItem<ActiveBreakpointsDataHolder>(_Breakpoint.Process);

      var SourceInfo = SourceLocation.Decode(InstructionAddress.EntityId.ToArray());

      Data.ActiveBreakpoints.Add(new Tuple<SourceLocation, DkmRuntimeBreakpoint>(SourceInfo, _Breakpoint));

      UpdateBreakpoints(_Breakpoint.Process, Data);
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
      var RuntimeInstructionBreakpoint = _Breakpoint as DkmRuntimeInstructionBreakpoint;

      if (RuntimeInstructionBreakpoint == null)
        return;

      var InstructionAddress = RuntimeInstructionBreakpoint.InstructionAddress as DkmCustomInstructionAddress;

      if (InstructionAddress == null)
        return;

      ActiveBreakpointsDataHolder Data = Utility.GetOrCreateDataItem<ActiveBreakpointsDataHolder>(_Breakpoint.Process);

      var SourceInfo = SourceLocation.Decode(InstructionAddress.EntityId.ToArray());

      Data.ActiveBreakpoints.RemoveAll(Location => Location.Item1 == SourceInfo);

      UpdateBreakpoints(_Breakpoint.Process, Data);
    }

    #endregion

    #region IDkmRuntimeStepper

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
      return OwnsCurrentExecuteLocation(_RuntimeInstance, _Stepper);
    }

    void IDkmRuntimeStepper.Step(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason
      )
    {
      if (_Stepper.StepKind == DkmStepKind.StepIntoSpecific)
        throw new NotSupportedException();

      DkmProcess        Process     = _RuntimeInstance.Process;
      StepperDataHolder StepperData = Utility.GetOrCreateDataItem<StepperDataHolder>(Process);

      if (StepperData.ActiveStepper != null)
      {
        StepperData.ActiveStepper.CancelStepper(_RuntimeInstance);

        ClearStepperData(Process);
      }

      StepperData.ActiveStepper = _Stepper;

      StepperData.ActiveStepKind.Write((uint)_Stepper.StepKind);

      new LocalComponent.OnStepNotification()
      { 
        StepKind = _Stepper.StepKind
      }.SendHigher(Process);
    }

    void IDkmRuntimeStepper.StopStep(
        DkmRuntimeInstance _RuntimeInstance, 
        DkmStepper         _Stepper
      )
    {
      ClearStepperData(_RuntimeInstance.Process);
    }

    void IDkmRuntimeStepper.AfterSteppingArbitration(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _NewControllingRuntimeInstance
      )
    {
      ClearStepperData(_RuntimeInstance.Process);
    }

    void IDkmRuntimeStepper.OnNewControllingRuntimeInstance(
        DkmRuntimeInstance       _RuntimeInstance, 
        DkmStepper               _Stepper, 
        DkmStepArbitrationReason _Reason, 
        DkmRuntimeInstance       _ControllingRuntimeInstance
      )
    {
      ClearStepperData(_RuntimeInstance.Process);
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
      ClearStepperData(_RuntimeInstance.Process);
    }

    void IDkmRuntimeStepper.NotifyStepComplete(
        DkmRuntimeInstance _RuntimeInstance, 
        DkmStepper         _Stepper
      )
    {
      /*
        When we step into some transition functions like sq_call step manager calls OwnsCurrentExecution
        only on next step. This means the users will have to make two identical steps to eventually get into scripts. 
        To avoid this behaviour we check after-step instruction address and recreate a stepper to step into 
        Squirrel code without any pause
      */
      TryInterceptTransitionStep(_RuntimeInstance, _Stepper);
    }

    #endregion

    #region Service
    
    void UpdateBreakpoints(
        DkmProcess                  _Process,
        ActiveBreakpointsDataHolder _BreaksData
      )
    {
      var Locations = Utility.GetOrCreateDataItem<LocationsDataHolder>(_Process);

      if (Locations.StringBufferAddress == 0 || Locations.ActiveBreakpointsAddress == 0)
        return;

      var StringBufferAddress     = Locations.StringBufferAddress;
      var ActiveBreakpointAddress = Locations.ActiveBreakpointsAddress;

      for (int i = 0; i < _BreaksData.ActiveBreakpoints.Count; i++)
      {
        var Breakpoint = _BreaksData.ActiveBreakpoints[i].Item1;

        byte[] Encoded = Encoding.UTF8.GetBytes(Breakpoint.Source);

        // Write file name to buffer
        Utility.TryWriteRawBytes    (_Process, StringBufferAddress, Encoded);
        Utility.TryWriteByteVariable(_Process, StringBufferAddress + (ulong)Encoded.Length, 0);

        // Write to pointer
        Utility.TryWritePointerVariable(_Process, ActiveBreakpointAddress, StringBufferAddress);

        StringBufferAddress += (ulong)Encoded.Length + 1;

        // Write Line
        Utility.TryWriteUlongVariable(_Process, ActiveBreakpointAddress + sizeof(ulong), (ulong)Breakpoint.Line);

        // Write length as zero
        Utility.TryWriteUlongVariable(_Process, ActiveBreakpointAddress + sizeof(ulong) * 2, 0);

        ActiveBreakpointAddress += (ulong)Marshal.SizeOf(typeof(BreakpointData));
      }

      _BreaksData.ActiveBreakpointsSize.Write((ulong)_BreaksData.ActiveBreakpoints.Count);
    }

    void OnBreakpointHit(
        DkmThread _Thread
      )
    {
      var ActiveBreakpointData = Utility.GetOrCreateDataItem<ActiveBreakpointsDataHolder>(_Thread.Process);
      var HitBreakpointIndex = ActiveBreakpointData.HitBreakpointIndex.Read();

      if (HitBreakpointIndex < (ulong)ActiveBreakpointData.ActiveBreakpoints.Count && HitBreakpointIndex != ulong.MaxValue)
      {
        var RuntimeBreakpoint = ActiveBreakpointData.ActiveBreakpoints[(int)HitBreakpointIndex].Item2;

        _Thread.GetCurrentFrameInfo(out ulong _ReturnAddress, out ulong _FrameBase, out ulong _VFrame);

        if (RuntimeBreakpoint != null)
        {
          new LocalComponent.FetchCallstackRequest().SendHigher(_Thread.Process);

          RuntimeBreakpoint.OnHit(_Thread, false);

          ActiveBreakpointData.HitBreakpointIndex.Write(ulong.MaxValue);

          return;
        }
      }
    }

    void OnStepCompleted(
        DkmThread _Thread
      )
    {
      var StepperData = Utility.GetOrCreateDataItem<StepperDataHolder>(_Thread.Process);

      if (StepperData.ActiveStepper != null)
      {
        // Stepper breakpoint
        ulong State = StepperData.ActiveStepKind.Read();

        if (State != InvalidStepKind)
        {
          new LocalComponent.FetchCallstackRequest().SendHigher(_Thread.Process);

          new LocalComponent.OnStepFinishedNotification().SendHigher(_Thread.Process);
          
          StepperData.ActiveStepKind.Write(InvalidStepKind);

          StepperData.ActiveStepper.OnStepComplete(_Thread, false);

          StepperData.ActiveStepper = null;
        }
      }
    }

    void OnStepFallthrough(
        DkmThread _Thread
      )
    {
      var StepperData = Utility.GetOrCreateDataItem<StepperDataHolder>(_Thread.Process);

      StepperData.ActiveStepper.OnStepArbitration(DkmStepArbitrationReason.ExitRuntime, StepperData.ActiveStepper.GetControllingRuntimeInstance());

      new LocalComponent.OnStepFallthroughFinishedNotification().SendHigher(_Thread.Process);
    }

    static void ClearStepperData(
        DkmProcess _Process
      )
    {
      var StepperData = Utility.GetOrCreateDataItem<StepperDataHolder>(_Process);

      StepperData.ActiveStepper = null;

      StepperData.ActiveStepKind.Write(InvalidStepKind);
    }

    bool OwnsCurrentExecuteLocation(
        DkmRuntimeInstance _Instance,
        DkmStepper         _Stepper
      )
    {
      if (_Stepper.StartingAddress == null)
        return false;

      var Locations          = Utility.GetOrCreateDataItem<LocationsDataHolder>(_Instance.Process);
      var InstructionAddress = _Stepper.Thread.GetCurrentRegisters(new DkmUnwoundRegister[0]).GetInstructionPointer();

      return Locations.HelperLocation.In(InstructionAddress) || Locations.SquirrelLocations.Any(Range => Range.In(InstructionAddress));
    }

    bool TryInterceptTransitionStep(
        DkmRuntimeInstance _RuntimeInstance,
        DkmStepper         _Stepper
      )
    {
      if (_Stepper.StepKind == DkmStepKind.StepIntoSpecific)
        return false;

      if (OwnsCurrentExecuteLocation(_RuntimeInstance, _Stepper))
      {
        try
        {
          DkmProcess        Process     = _RuntimeInstance.Process;
          StepperDataHolder StepperData = Utility.GetOrCreateDataItem<StepperDataHolder>(Process);
          RemoteProcessData ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(_RuntimeInstance.Process);

          if (ProcessData.ModuleInstance == null)
            return false;

          if (!_Stepper.StepControlRequested(DkmStepArbitrationReason.EnterRuntime, _RuntimeInstance))
            return false;

          _Stepper.TakeStepControl(true, DkmStepArbitrationReason.EnterRuntime, _RuntimeInstance);

          var InstructionAddress = _Stepper.Thread.GetCurrentRegisters(new DkmUnwoundRegister[0]).GetInstructionPointer();

          var Instruction = DkmCustomInstructionAddress.Create(
              _RuntimeInstance,
              ProcessData.ModuleInstance,
              null,
              0,
              null,
              new DkmInstructionAddress.CPUInstruction(InstructionAddress)
           );

          StepperData.ActiveStepper = DkmStepper.Create(
              _Stepper.Thread,
              Instruction,
              _Stepper.FrameBase,
              DkmStepKind.Into,
              _Stepper.StepUnit,
              _Stepper.SourceId,
              null,
              null,
              null
            );

          StepperData.ActiveStepKind.Write((uint)_Stepper.StepKind);

          StepperData.ActiveStepper.Enable(true);

          return true;
        }
        catch (Exception _Exception)
        {
          new LocalComponent.HandleCustomMessage()
          {
            Message = $"Exception in TryInterceptTransitionStep: '{_Exception.Message}'"
          }.SendHigher(_RuntimeInstance.Process);

          // TODO add log
          if (_Stepper?.GetControllingRuntimeInstance() == _RuntimeInstance)
            _Stepper.Close();
        }
      }

      return false;
    }

    #endregion

    #region Requests
    [DataContract]
    [MessageTo(Guids.SquirrelRemoteComponentID)]
    internal class StepperLocationsNotification : MessageBase<StepperLocationsNotification>
    {
      [DataMember]
      public ulong StepperKindAddress;

      public override void Handle(
          DkmProcess _Process
        )
      {
        StepperDataHolder Data = Utility.GetOrCreateDataItem<StepperDataHolder>(_Process);

        Data.ActiveStepKind = new UInt32Proxy(_Process, StepperKindAddress);
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelRemoteComponentID)]
    internal class HelperLocationsNotification : MessageBase<HelperLocationsNotification>
    {
      [DataMember]
      public AddressRange HelperAddresses;

      [DataMember]
      public List<AddressRange> SquirrelAddresses;

      [DataMember]
      public ulong StringBufferAddress;

      [DataMember]
      public ulong ActiveBreakpointsDataAddress;

      [DataMember]
      public ulong ActiveBreakpointsCountAddress;

      [DataMember]
      public ulong HitBreakpointIndexAddress;

      public override void Handle(
          DkmProcess _Process
        )
      {
        LocationsDataHolder         Data            = Utility.GetOrCreateDataItem<LocationsDataHolder>(_Process);
        ActiveBreakpointsDataHolder BreakpointsData = Utility.GetOrCreateDataItem<ActiveBreakpointsDataHolder>(_Process);

        Data.HelperLocation           = new AddressRange(HelperAddresses);
        Data.SquirrelLocations        = new List<AddressRange>(SquirrelAddresses);
        Data.StringBufferAddress      = StringBufferAddress;
        Data.ActiveBreakpointsAddress = ActiveBreakpointsDataAddress;

        BreakpointsData.ActiveBreakpointsSize = new UInt64Proxy(_Process, ActiveBreakpointsCountAddress);
        BreakpointsData.HitBreakpointIndex    = new UInt64Proxy(_Process, HitBreakpointIndexAddress);
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelRemoteComponentID)]
    internal class HelperBreakpointsNotification : MessageBase<HelperBreakpointsNotification>
    {
      [DataMember]
      public Guid BreakpointHitBPGuid;

      [DataMember]
      public Guid StepCompleteBPGuid;

      public override void Handle(
          DkmProcess _Process
        )
      {
        BreakpointsGuidsDataHolder Data = Utility.GetOrCreateDataItem<BreakpointsGuidsDataHolder>(_Process);

        Data.BreakpointHitGuid = BreakpointHitBPGuid;
        Data.StepCompleteGuid  = StepCompleteBPGuid;
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelRemoteComponentID)]
    internal class CreateSquirrelRuntimeRequest : MessageBase<CreateSquirrelRuntimeRequest>
    {
      public override void Handle(
          DkmProcess _Process
        )
      {
        RemoteProcessData ProcessData = Utility.GetOrCreateDataItem<RemoteProcessData>(_Process);

        if (ProcessData.Language == null)
        {
          ProcessData.CompilerID = new DkmCompilerId(Guids.SquirrelCompilerID, Guids.SquirrelLanguageID);
          ProcessData.Language = DkmLanguage.Create("Squirrel", ProcessData.CompilerID);
        }

        if (ProcessData.RuntimeInstance == null)
        {
          DkmRuntimeInstanceId RuntimeID = new DkmRuntimeInstanceId(Guids.SquirrelRuntimeID, 0);

          ProcessData.RuntimeInstance = DkmCustomRuntimeInstance.Create(_Process, RuntimeID, null);
        }

        if (ProcessData.Module == null)
        {
          DkmModuleId ModuleID = new DkmModuleId(Guid.NewGuid(), Guids.SquirrelSymbolProviderID);

          ProcessData.Module = DkmModule.Create(ModuleID, "sq.vm.code", ProcessData.CompilerID, _Process.Connection, null);
        }

        if (ProcessData.ModuleInstance == null)
        {
          DkmDynamicSymbolFileId SymbolFileID = DkmDynamicSymbolFileId.Create(Guids.SquirrelSymbolProviderID);
          ProcessData.ModuleInstance = DkmCustomModuleInstance.Create(
                "Squirrel",
                "Squirrel.code",
                0,
                ProcessData.RuntimeInstance,
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

          ProcessData.ModuleInstance.SetModule(ProcessData.Module, true);
        }
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelRemoteComponentID)]
    internal class OnStepFallthroughNotification : MessageBase<OnStepFallthroughNotification>
    {
      [DataMember]
      public Guid BreakpointGuid;

      public override void Handle(
          DkmProcess _Process
        )
      {
        StepperDataHolder          StepperData     = Utility.GetOrCreateDataItem<StepperDataHolder>(_Process);
        BreakpointsGuidsDataHolder BreakpointsData = Utility.GetOrCreateDataItem<BreakpointsGuidsDataHolder>(_Process);

        if (StepperData.ActiveStepper != null)
        {
          BreakpointsData.StepFallthroughGuid = BreakpointGuid;
        }
      }
    }
    #endregion
  }
}
