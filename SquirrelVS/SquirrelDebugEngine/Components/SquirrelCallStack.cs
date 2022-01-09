using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using SquirrelDebugEngine.Proxy;
using System;

namespace SquirrelDebugEngine
{
  internal class SquirrelThreadData
  {
    public int                  LastFramePosition = 0;
    public List<CallstackFrame> Callstack         = null;
  }
  internal class SquirrelCallStack : DkmDataItem
  {
    public Dictionary<SQVM, SquirrelThreadData> ActiveThreads = new Dictionary<SQVM, SquirrelThreadData>();
    
    public Int64 GetFrameStackBase(
        CallstackFrame _Frame,
        Int64          _Base
      )
    {
      if (_Frame.StackBase == 0)
      {
        var Callstack = ActiveThreads[_Frame.Thread].Callstack;

        Int64 StackBase = _Base;

        for (int i = 0; i < Callstack.Count; i++)
        {
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
