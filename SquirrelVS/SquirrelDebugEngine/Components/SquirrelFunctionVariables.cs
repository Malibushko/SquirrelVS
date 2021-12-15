using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  public class SquirrelFunctionVariables : DkmDataItem
  {
    public DkmProcess                 Process   = null;
    public List<SquirrelVariableInfo> Variables = new List<SquirrelVariableInfo>();
  }
}
