using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Symbols;
using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine
{
  public class RemoteProcessData : DkmDataItem
  {
    public DkmLanguage   Language = null;
    public DkmCompilerId CompilerID;

    public DkmCustomRuntimeInstance RuntimeInstance = null;
    public DkmCustomModuleInstance  ModuleInstance  = null;
    public DkmModule                Module          = null;
  }
}
