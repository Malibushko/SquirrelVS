using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine.Proxy
{
  internal class TableVisualizationProxy : IVisualizableObject
  {
    public FieldDataItem[] Keys;

    public FieldDataItem[] GetChildren()
    {
      return Keys;
    }

    public string GetDisplayNativeType()
    {
      throw new NotImplementedException();
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Table.ToString();
    }

    public string GetDisplayValue()
    {
      return $"{{ {Keys.Length} elements }}";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return Keys.Length > 0 ? SQObject.ExpandableEvaluationFlags : SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
