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


namespace SquirrelDebugEngine
{
  public class RemoteComponent : IDkmCustomMessageForwardReceiver, IDkmRuntimeBreakpointReceived
  {
    void IDkmRuntimeBreakpointReceived.OnRuntimeBreakpointReceived(
        DkmRuntimeBreakpoint _Breakpoint, 
        DkmThread            _Thread,
        bool                 _HasException,
        DkmEventDescriptorS  _EventDescriptior
      )
    {
      DkmProcess Process = _Thread.Process;

      _Thread.GetCurrentFrameInfo(out ulong _ReturnAddress, out ulong _FrameBase, out ulong _VFrame);

      BreakpointHitData Data = new BreakpointHitData
      {
        BreakpointID  = _Breakpoint.UniqueId,
        ThreadID      = _Thread.UniqueId,
        ReturnAddress = _ReturnAddress,
        FrameBase     = _FrameBase,
        vFrame        = _VFrame
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

              BreakpoinsData = _Message.Parameter1 as SquirrelBreakpoints;
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
  }
}
