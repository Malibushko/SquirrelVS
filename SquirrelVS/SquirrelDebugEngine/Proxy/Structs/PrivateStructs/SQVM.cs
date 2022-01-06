using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQVM")]
  internal class SQVM : StructProxy, IVisualizableObject
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
      public StructField<UInt64Proxy>                        _suspended;

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

    public UInt64Proxy Suspend
    {
      get
      {
        return GetFieldProxy(FieldsData._suspended);
      }
    }

    public PointerProxy<ArrayProxy<CallInfo>> CallStack
    {
      get
      {
        return GetFieldProxy(FieldsData._callsstack);
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Thread.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQVM";
    }

    public string GetDisplayValue()
    {
      return $"[Thread {(Suspend.Read() > 0 ? "(Suspended)" : string.Empty)} {CallStackSize.Read()} calls, {Stack.Size} on stack]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.ExpandableEvaluationFlags;
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      Items.Add(new FieldDataItem
      {
        Name   = "[Stack]",
        Object = Stack
      });

      List<CallInfo> CallstackItems      = new List<CallInfo>();
      var            CallstackItemsCount = CallStackSize.Read();
      var            CallstackData       = CallStack.Read();

      for (int i = 0; i < CallstackItemsCount; i++)
        CallstackItems.Add(CallstackData[i]);

      Items.Add(new FieldDataItem
      {
        Name = "[Call Stack]",
        Object = new ArrayVisualizationProxy<CallInfo>
        {
          Elements = CallstackItems.ToArray()
        }
      });

      return Items.ToArray();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
