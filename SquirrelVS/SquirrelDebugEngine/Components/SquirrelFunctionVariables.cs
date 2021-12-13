using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  public class SquirrelFunctionVariables : DkmDataItem
  {
    public List<SquirrelVariableInfo> Variables = new List<SquirrelVariableInfo>();
  }
}
