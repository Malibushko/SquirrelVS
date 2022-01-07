using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using SquirrelDebugEngine.Proxy;
using System;

namespace SquirrelDebugEngine
{
  internal class SquirrelCallStack : DkmDataItem
  {
    public List<CallstackFrame> Callstack   = new List<CallstackFrame>();
    public Stack<SQVM>          ThreadStack = new Stack<SQVM>();
    public Int64 GetFrameStackBase(
        CallstackFrame _Frame,
        Int64          _Base
      )
    {
      if (_Frame.StackBase == 0)
      {
        Int64 StackBase = _Base;

        for (int i = 0; i < Callstack.Count; i++)
        {
          if (Callstack[i].Thread.Address != _Frame.Thread.Address)
            continue;

          if (Callstack[i] == _Frame)
            break;

          StackBase -= Callstack[i].PreviousStackBase;
        }

        _Frame.StackBase = StackBase;
      }
      
      return _Frame.StackBase;
    }
  }
}
