using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using System;

namespace SquirrelDebugEngine.Proxy
{
  internal class SQVector<T> : StructProxy
    where T : IDataProxy
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<PointerProxy<ArrayProxy<T>>> _vals;
      public StructField<UInt64Proxy>                 _size;
      public StructField<UInt64Proxy>                 _allocated;

#pragma warning restore 0649
    }

    protected Fields m_Fields;

    public SQVector(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      // Empty
    }

    public UInt64 Size
    {
      get
      {
        return GetFieldProxy(m_Fields._size).Read();
       }
    }

    public PointerProxy<ArrayProxy<T>> Values
    {
      get
      {
        return GetFieldProxy(m_Fields._vals);
      }
    }

    public T this[long _Index]
    {
      get => Values.Read()[_Index];
    }
  }

  internal class SQVectorVisualizable<T> : SQVector<T>, IVisualizableObject
    where T: IVisualizableObject, IDataProxy
  {
    public SQVectorVisualizable(
        DkmProcess _Process,
        ulong      _Address
      ) :
      base(_Process, _Address)
    {
      // Empty
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      var ValuesData = Values.Read();
      int ValuesSize = (int)Size;

      for (int i = 0; i < ValuesSize; i++)
      {
        var NativeItem = ValuesData[i];

        var Item = new FieldDataItem()
        {
          Object = NativeItem,
          Name         = $"[{i}]"
        };

        Items.Add(Item);
      }

      return Items.ToArray();
    }

    public string GetDisplayNativeType()
    {
      foreach (var Attribute in typeof(T).GetCustomAttributes(true))
      {
        if (Attribute is StructProxyAttribute)
          return (Attribute as StructProxyAttribute).StructName;
      }

      return $"sqvector<{typeof(T).Name}";
    }

    public string GetDisplayType()
    {
      return $"SQVector<{typeof(T).Name}>";
    }

    public string GetDisplayValue()
    {
      return $"[Vector of {Size} element(s)]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return Size > 0 ? SQObject.ExpandableEvaluationFlags : SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }

  [StructProxy(StructName = "sqvector<SQObjectPtr>")]
  internal class SQObjectPtrVec : SQVectorVisualizable<SQObjectPtr>
  {
    public SQObjectPtrVec(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }
  }

  [StructProxy(StructName = "sqvector<SQClassMember>")]
  internal class SQClassMemberVec : SQVectorVisualizable<SQClassMember>
  {
    public SQClassMemberVec(
        DkmProcess _Process,
        ulong      _Address
      ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }
  }
}
