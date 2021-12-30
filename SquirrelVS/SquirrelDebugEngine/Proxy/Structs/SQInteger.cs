using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;
using System;


namespace SquirrelDebugEngine.Proxy
{
  internal class SQInteger : SQObject
  {
    private class Fields
    {
      public StructField<Int64Proxy> _unVal;
    }

    private readonly Fields m_Fields;

    public SQInteger(
          DkmProcess process, 
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }
    public new long Read()
    {
      return GetFieldProxy(m_Fields._unVal).Read();
    }
  }
}
