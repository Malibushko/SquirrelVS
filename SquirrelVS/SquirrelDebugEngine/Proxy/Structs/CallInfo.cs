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
      public StructField<SQClosure> _closure;
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

    public SQClosure Closure
    {
      get
      {
        return GetFieldProxy(m_Fields._closure);
      }
    }
  }
}
