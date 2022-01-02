using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQVM::CallInfo")]
  internal class CallInfo : StructProxy
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr>  _closure;
      public StructField<PointerProxy> _ip;
      public StructField<Int32Proxy>   _prevstkbase;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public CallInfo(
          DkmProcess process, 
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Closure
    {
      get
      {
        return GetFieldProxy(m_Fields._closure);
      }
    }

    public PointerProxy InstructionPointer
    {
      get
      {
        return GetFieldProxy(m_Fields._ip);
      }
    }

    public Int32 PreviousStackBase
    {
      get
      {
        return GetFieldProxy(m_Fields._prevstkbase).Read();
      }
    }
  }
}
