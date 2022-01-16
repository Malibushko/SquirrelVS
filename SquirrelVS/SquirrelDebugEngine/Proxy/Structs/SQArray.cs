using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQArray")]
  internal class SQArray : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtrVec> _values;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQArray(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtrVec Values
    {
      get
      {
        return GetFieldProxy(m_Fields._values);
      }
    }

    public FieldDataItem[] GetChildren()
    {
      return Values.GetChildren();
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Array.ToString();
    }

    public string GetDisplayValue()
    {
      return $"[Array of {Values.Size} element(s)]";
    }

    public string GetDisplayNativeType()
    {
      return "SQArray";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return Values.GetEvaluationFlags();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
