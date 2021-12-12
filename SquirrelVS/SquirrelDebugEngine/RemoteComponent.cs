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
  public class RemoteComponent : IDkmCustomMessageForwardReceiver, IDkmRuntimeBreakpointReceived, IDkmRuntimeMonitorBreakpointHandler
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
  }
}
