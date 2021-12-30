using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQFunctionProto")]
  internal class SQFunctionProto : SQObject
  {
    private class Fields
    {
      public StructField<SQObjectPtr>             _name;

      public StructField<Int64Proxy>              _nparameters;
      public StructField<ArrayProxy<SQObjectPtr>> _parameters;

      public StructField<Int64Proxy>              _noutervalues;
      public StructField<ArrayProxy<SQObjectPtr>> _outervalues;
    }

    private readonly Fields m_Fields;
    public SQFunctionProto(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }
  }
}
