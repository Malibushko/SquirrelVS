using System.Linq;
using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelDebugEngine
{
  internal class CallstackFrame
  {
    internal Proxy.SQFunctionProto NativeFunctionInfo;

    internal string SourceName;
    internal string FunctionName;
    internal int    Line;

    internal CallstackFrame(
        Proxy.SQFunctionProto _NativeFunction
      )
    {
      NativeFunctionInfo = _NativeFunction;
    }
  }
}
