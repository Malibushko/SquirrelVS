using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQVM::CallInfo")]
  internal class CallInfo : StructProxy, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr>               _closure;
      public StructField<PointerProxy>              _ip;
      public StructField<PointerProxy<SQGenerator>> _generator;
      public StructField<Int32Proxy>                _ncalls;
      public StructField<Int32Proxy>                _prevstkbase;

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

    public string GetDisplayType()
    {
      return "Call Info";
    }

    public string GetDisplayNativeType()
    {
      return "SQVM::CallInfo";
    }

    public string GetDisplayValue()
    {
      return Closure.GetDisplayValue();
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
        Name   = "Closure",
        Object = Closure
      });

      Items.Add(new FieldDataItem
      {
        Name = "Previous stack base",
        Object = new PrimitiveVisualizationProxy<int>
        {
          NativeType = "int",
          Object     = PreviousStackBase,
          Type       = SquirrelVariableInfo.Type.Integer.ToString()
        }
      });

      Items.Add(new FieldDataItem
      {
        Name = "Calls count",
        Object = new PrimitiveVisualizationProxy<int>
        {
          NativeType = "int",
          Object     = GetFieldProxy(m_Fields._ncalls).Read(),
          Type       = SquirrelVariableInfo.Type.Integer.ToString()
        }
      });

      var GeneratorPointerProxy = GetFieldProxy(m_Fields._generator);

      if (!GeneratorPointerProxy.IsNull)
      {
        Items.Add(new FieldDataItem
        {
          Name = "[Generator]",
          Object = GetFieldProxy(m_Fields._generator).Read()
        });
      }

      return Items.ToArray();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
