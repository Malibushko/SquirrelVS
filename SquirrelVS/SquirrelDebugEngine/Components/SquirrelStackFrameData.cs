using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;

namespace SquirrelDebugEngine
{
  public class SquirrelStackFrameData : DkmDataItem
  {
    public DkmStackWalkFrame ParentFrame;
    public int               IndexFromTop = -1;
  }
}
