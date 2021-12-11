using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;
using Microsoft.VisualStudio.Debugger.Symbols;
using System;

namespace SquirrelDebugEngine
{
  public enum HelperState
  {
    NotInitialized,
    WaitingForInitialization,
    Initialized
  };

  public class LocalProcessData : DkmDataItem
  {
    public DkmCustomRuntimeInstance RuntimeInstance;
    public DkmCustomModuleInstance  ModuleInstance;

    public DkmNativeModuleInstance  SquirrelModule;
    public SquirrelLocations        SquirrelLocations;

    public ulong                    LoadLibraryWAddress;
    public ulong                    HelperStartAddress;
    public ulong                    HelperEndAddress;
    
    public ulong                    DebugHookAddress;
    public HelperState              HelperState = HelperState.NotInitialized;
    public HookData                 HookData;

    public DkmThread                SquirrelThread;
    public ulong                    SquirrelHandleAddress;

    public SymbolStore              Symbols = new SymbolStore();

    public string                   WorkingDirectory;
  }

  public class SquirrelBreakpoints : DkmDataItem
  {
    public Guid SquirrelOpenBreakpoint;
    public Guid SquirrelCloseBreakpoint;
    public Guid SquirrelLoadFile;

    public Guid SquirrelHelperBreakpointHit;
    public Guid SquirrelHelperStepComplete;
    public Guid SquirrelHelperStepInto;
    public Guid SquirrelHelperStepOut;
    public Guid SquirrelHelperAsyncBreak;
    public Guid SquirrelHelperInitialized;

    public ulong WorkingDirectoryAddress;
    public ulong SquirrelBreakpointDataAddress;
  }
}
