using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Native;
using SquirrelDebugEngine.Proxy;

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

    public SQVM                     SquirrelHandle;
    public ulong                    LoadLibraryWAddress;

    public string                   WorkingDirectory;
  }
}
