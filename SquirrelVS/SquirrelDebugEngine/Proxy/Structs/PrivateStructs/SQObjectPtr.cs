using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQObjectPtr")]
  internal class SQObjectPtr : SQObject
  {
    public SQObjectPtr(
          DkmProcess _Process, 
          ulong      _Address
        ) :
      base(_Process, _Address)
    {
      // Empty
    }
  }
}
