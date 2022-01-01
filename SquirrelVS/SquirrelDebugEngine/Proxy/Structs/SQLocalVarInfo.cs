using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQLocalVarInfo")]
  internal class SQLocalVarInfo : StructProxy
  {
    public class Fields
    {
      public StructField<SQString> _name;
    }

    private readonly Fields m_Fields;

    public SQLocalVarInfo(
        DkmProcess _Process,
        ulong _Address
      ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQString Name
    {
      get
      {
        return GetFieldProxy(m_Fields._name, false);
      }
    }
  }
}
