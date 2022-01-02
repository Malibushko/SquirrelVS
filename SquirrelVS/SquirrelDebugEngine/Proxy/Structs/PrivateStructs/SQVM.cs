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
#pragma warning disable 0649

      public StructField<PointerProxy>                       _debughook;

      public StructField<PointerProxy<ArrayProxy<CallInfo>>> _callsstack;
      public StructField<Int64Proxy>                         _callsstacksize;

      public StructField<PointerProxy>                       _top;
      public StructField<SQObjectPtrVec>                     _stack;
      public StructField<Int64Proxy>                         _stackbase;

#pragma warning restore 0649
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

    public SQObjectPtrVec Stack
    {
      get
      {
        return GetFieldProxy(FieldsData._stack);
      }
    }

    public long StackOffset
    {
      get
      {
        return FieldsData._stack.Offset;
      }
    }

    public Int64Proxy StackBase
    {
      get
      {
        return GetFieldProxy(FieldsData._stackbase);
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
