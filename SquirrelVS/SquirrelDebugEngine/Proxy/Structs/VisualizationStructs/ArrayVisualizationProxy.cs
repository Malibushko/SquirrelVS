using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine.Proxy
{
  internal class ArrayVisualizationProxy<T> : IVisualizableObject
    where T : IVisualizableObject
  {
    public T[] Elements;
    
    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      for (int i = 0; i < Elements.Length; i++)
      {
        var Item = new FieldDataItem()
        {
          Object = Elements[i],
          Name   = $"[{i}]"
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
          return $"{(Attribute as StructProxyAttribute).StructName} *";
      }

      return $"{typeof(T).Name} *";
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Array.ToString();
    }

    public string GetDisplayValue()
    {
      return $"[Array of {Elements.Length} element(s)";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return Elements.Length > 0 ? SQObject.ExpandableEvaluationFlags : SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
