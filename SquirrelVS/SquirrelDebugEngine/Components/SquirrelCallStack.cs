using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  public class SquirrelCallStack : DkmDataItem
  {
    public Stack<SquirrelBreakpointData> Callstack = new Stack<SquirrelBreakpointData>();
  }
}
