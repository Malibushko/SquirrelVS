using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQNativeClosure")]
  internal class SQNativeClosure : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr>    _name;
      public StructField<SQObjectPtrVec> _env;
      public StructField<PointerProxy>   _function;
      public StructField<SQObjectPtrVec> _outervalues;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQNativeClosure(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public PointerProxy Function
    {
      get
      {
        return GetFieldProxy(m_Fields._function);
      }
    }

    public SQObjectPtr Name
    {
      get
      {
        return GetFieldProxy(m_Fields._name);
      }
    }

    public SQObjectPtrVec EnvironmentalVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._env);
      }
    }

    public SQObjectPtrVec OuterValues
    {
      get
      {
        return GetFieldProxy(m_Fields._outervalues);
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.NativeClosure.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQNativeClosure";
    }

    public string GetDisplayValue()
    {
      return $"[Native Closure {(Name.Type == SquirrelVariableInfo.Type.String ? Name.GetDisplayValue() : "0x" + Function.Address.ToString("x"))}]";
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
        Name         = "Name",
        NativeObject = Name
      });
/*
      Items.Add(new FieldDataItem
      {
        Name         = "Function",
        NativeObject = Function
      });
*/
      Items.Add(new FieldDataItem
      {
        Name         = "[Outer Values]",
        NativeObject = OuterValues
      });

      Items.Add(new FieldDataItem
      {
        Name = "[Environmental Variables]",
        NativeObject = EnvironmentalVariables
      });

      return Items.ToArray();
    }
  }
}
