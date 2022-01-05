using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQClosure")]
  internal class SQClosure : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr>    _env;
      public StructField<SQObjectPtr>    _function;
      public StructField<SQObjectPtrVec> _outervalues;
      public StructField<SQObjectPtrVec> _defaultparams;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQClosure(
          DkmProcess process,
          ulong      address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Function
    {
      get
      {
        return GetFieldProxy(m_Fields._function);
      }
    }

    public SQObjectPtr EnvironmentalVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._env);
      }
    }

    public SQObjectPtrVec OuterVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._outervalues);
      }
    }

    public SQObjectPtrVec DefaultParameters
    {
      get
      {
        return GetFieldProxy(m_Fields._defaultparams);
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Closure.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQClosure";
    }

    public string GetDisplayValue()
    {
      return $"[Squirrel Closure {Function?.GetDisplayValue()}]";
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
        Name         = "[Environmental Variables]",
        NativeObject = EnvironmentalVariables
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Function]",
        NativeObject = Function
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Outer Variables]",
        NativeObject = OuterVariables
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Default Parameters]",
        NativeObject = DefaultParameters
      });

      return Items.ToArray();
    }
  }
}