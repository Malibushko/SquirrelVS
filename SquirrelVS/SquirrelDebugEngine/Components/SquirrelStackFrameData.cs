using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;

namespace SquirrelDebugEngine
{
  public class SquirrelStackFrameData : DkmDataItem
  {
    internal CallstackFrame NativeFrame;
  }
}
