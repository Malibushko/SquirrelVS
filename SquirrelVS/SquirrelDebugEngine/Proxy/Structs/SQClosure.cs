using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQClosure")]
  internal class SQClosure : SQObject
  {
    private class Fields
    {
      public StructField<SQObjectPtr> _function;
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
        return GetFieldProxy(m_Fields._function, RawValue.Read(), false);
      }
    }
  }
}