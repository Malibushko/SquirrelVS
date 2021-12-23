using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Symbols;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.Stepping;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

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
