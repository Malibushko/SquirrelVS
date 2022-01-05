﻿using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQNativeClosure")]
  internal class SQNativeClosure : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

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
      return "0x" + Function.Address.ToString("x");
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.DefaultEvaluationFlags;
    }

    public ExpandableDataItem[] GetChildren()
    {
      return new ExpandableDataItem[0];
    }
  }
}
