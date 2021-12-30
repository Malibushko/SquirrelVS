using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  internal class SquirrelCallStack : DkmDataItem
  {
    public Stack<CallstackFrame> Callstack = new Stack<CallstackFrame>();
  }
}
