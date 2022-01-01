using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQVM")]
  internal class SQVM : StructProxy
  {
    private class Fields
    {
      public StructField<PointerProxy>                       _debughook;

      public StructField<PointerProxy<ArrayProxy<CallInfo>>> _callsstack;
      public StructField<Int64Proxy>                         _callsstacksize;

      public StructField<PointerProxy>                       _top;
      public StructField<PointerProxy>                       _stack;
    }

    private readonly Fields FieldsData;

    public SQVM(
        DkmProcess process,
        ulong address
      ) : 
      base(process, address)
    {
      InitializeStruct(this, out FieldsData);
    }

    public long StackTopOffset
    {
      get
      {
        return FieldsData._top.Offset;
      }
    }

    public long StackOffset
    {
      get
      {
        return FieldsData._stack.Offset;
      }
    }

    public PointerProxy DebugHookClosure
    {
      get
      {
        return GetFieldProxy(FieldsData._debughook);
      }
    }

    public Int64Proxy CallStackSize
    {
      get
      {
        return GetFieldProxy(FieldsData._callsstacksize);
      }
    }

    public PointerProxy<ArrayProxy<CallInfo>> CallStack
    {
      get
      {
        return GetFieldProxy(FieldsData._callsstack);
      }
    }
  }
}
