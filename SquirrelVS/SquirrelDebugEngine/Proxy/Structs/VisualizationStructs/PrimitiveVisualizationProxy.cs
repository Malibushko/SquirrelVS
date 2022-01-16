using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine.Proxy
{
  internal class PrimitiveVisualizationProxy<T> : IVisualizableObject
    where T : unmanaged
  {
    public T      Object { get; set; }

    public string Type { get; set; }

    public string NativeType { get; set; }

    public FieldDataItem[] GetChildren()
    {
      throw new NotImplementedException();
    }

    public string GetDisplayNativeType()
    {
      return NativeType;
    }

    public string GetDisplayType()
    {
      return Type;
    }

    public string GetDisplayValue()
    {
      return Object.ToString();
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
