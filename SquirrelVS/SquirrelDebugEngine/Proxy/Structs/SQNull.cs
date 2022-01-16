using Microsoft.VisualStudio.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine.Proxy
{
  internal class SQNull : StructProxy, ISQObject, IVisualizableObject
  {
    public SQNull(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      // Empty
    }

    public string GetDisplayNativeType()
    {
      return "void *";
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Null.ToString();
    }

    public string GetDisplayValue()
    {
      return "null";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.DefaultEvaluationFlags;
    }

    public FieldDataItem[] GetChildren()
    {
      return new FieldDataItem[0];
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
