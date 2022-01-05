using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQClosure")]
  internal class SQClosure : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr> _function;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQClosure(
          DkmProcess process,
          ulong      address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Function
    {
      get
      {
        return GetFieldProxy(m_Fields._function);
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Closure.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQClosure";
    }

    public string GetDisplayValue()
    {
      return "0x" + Address.ToString("x");
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.DefaultEvaluationFlags;
    }

    public ExpandableDataItem[] GetChildren()
    {
      return new ExpandableDataItem[0];
    }
  }
}