using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  internal class SquirrelObject : ISquirrelObject
  {
    public static object FromAddress(
        DkmProcess _Process,
        ulong      _NativeObjectAddress
      )
    {
      return new SquirrelObject();
    }
  }
}
