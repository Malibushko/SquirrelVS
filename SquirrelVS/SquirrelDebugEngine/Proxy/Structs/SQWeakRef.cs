using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQWeakRef")]
  internal class SQWeakRef : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObject> _obj;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQWeakRef(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObject Object
    {
      get
      {
        return GetFieldProxy(m_Fields._obj);
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.WeakRef.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQWeakRef";
    }

    public string GetDisplayValue()
    {
      return "[Weak Reference]";
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
        Name         = "[Object]",
        Object = Object
      });

      return Items.ToArray();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
