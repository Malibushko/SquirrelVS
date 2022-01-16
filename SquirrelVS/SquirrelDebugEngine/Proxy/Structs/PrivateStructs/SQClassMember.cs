using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQClassMember")]
  internal class SQClassMember : StructProxy, IVisualizableObject
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr> val;
      public StructField<SQObjectPtr> attrs;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQClassMember(
        DkmProcess _Process,
        ulong _Address
      ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Value
    {
      get
      {
        return GetFieldProxy(m_Fields.val);
      }
    }

    public SQObjectPtr Attributes
    {
      get
      {
        return GetFieldProxy(m_Fields.attrs);
      }
    }

    public string GetDisplayType()
    {
      return "Class Member";
    }

    public string GetDisplayNativeType()
    {
      return "SQClassMember";
    }

    public string GetDisplayValue()
    {
      return $"[Class Member]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      var Flags = SQObject.DefaultEvaluationFlags;

      Flags.Flags |= Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationResultFlags.Expandable;

      return Flags;
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      Items.Add(new FieldDataItem
      {
        Name         = "[Value]",
        Object = Value
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Attributes]",
        Object = Attributes
      });

      return Items.ToArray();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
