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
using System.IO;

namespace SquirrelDebugEngine
{
  public enum HelperState
  {
    NotInitialized,
    WaitingForInitialization,
    Initialized
  };

  internal class LocalProcessData : DkmDataItem
  {
    public DkmCustomRuntimeInstance RuntimeInstance;
    public DkmCustomModuleInstance  ModuleInstance;

    public DkmNativeModuleInstance  SquirrelModule;
    public SymbolStore              Symbols = new SymbolStore();

    public HelperState              HelperState = HelperState.NotInitialized;

    public Proxy.SQVM               SquirrelHandle;
    public ulong                    LoadLibraryWAddress;

    public string                   WorkingDirectory;
  }
}
