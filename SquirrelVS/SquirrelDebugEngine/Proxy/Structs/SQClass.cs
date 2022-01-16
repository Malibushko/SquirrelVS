using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQClass")]
  internal class SQClass : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<PointerProxy<SQTable>> _members;
      public StructField<PointerProxy<SQClass>> _base;
      public StructField<SQClassMemberVec>      _defaultvalues;
      public StructField<SQClassMemberVec>      _methods;
      public StructField<SQObjectPtr>           _attributes;
      public StructField<PointerProxy>          _typetag;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQClass(
          DkmProcess _Process,
          ulong      _Address
        ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public PointerProxy<SQTable> Members
    {
      get
      {
        return GetFieldProxy(m_Fields._members);
      }
    }

    public PointerProxy<SQClass> Base
    {
      get
      {
        return GetFieldProxy(m_Fields._base);
      }
    }

    public SQClassMemberVec DefaultValues
    {
      get
      {
        return GetFieldProxy(m_Fields._defaultvalues);
      }
    }

    public SQClassMemberVec Methods
    {
      get
      {
        return GetFieldProxy(m_Fields._methods);
      }
    }

    public SQObjectPtr Attributes
    {
      get
      {
        return GetFieldProxy(m_Fields._attributes);
      }
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      Items.Add(new FieldDataItem
      {
        Name         = "[Members]",
        Object = Members.Read()
      });
      
      Items.Add(new FieldDataItem
      {
        Name         = "[Methods]",
        Object = Methods
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Default Values]",
        Object = DefaultValues
      });
      
      Items.Add(new FieldDataItem
      {
        Name         = "[Attributes]",
        Object = Attributes
      });

      return Items.ToArray();
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Class.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQClass";
    }

    public string GetDisplayValue()
    {
      return $"[Class {Methods.Size} methods, {Members.Read().UsedNodeNumber} members]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      var Flags = SQObject.DefaultEvaluationFlags;

      Flags.Flags |= Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationResultFlags.Expandable;

      return Flags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}