using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine.Proxy
{
  internal class NativeVisualizationProxy : IVisualizableObject
  {
    public ulong  NativeAddress { get; set; }

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
      return NativeType;
    }

    public string GetDisplayValue()
    {
      return "0x" + NativeAddress.ToString("x");
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return true;
    }
  }
}
