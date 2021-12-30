using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;
using System;


namespace SquirrelDebugEngine.Proxy
{
  internal class SquirrelInteger : ISquirrelObject
  {
    public override object ReadValue(
        ulong                _NativeObjectAddress,
        DkmInspectionSession _Session, 
        DkmThread            _Thread, 
        DkmStackWalkFrame    _StackFrame
      )
    {
      return Utility.ReadLongVariable(_Session.Process, _NativeObjectAddress);
    }
  }
}
