using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQLineInfo")]
  internal class SQLineInfo : StructProxy
  {
    public class Fields
    {
      public StructField<Int64Proxy> _line;
      public StructField<Int64Proxy> _op;
    }

    private readonly Fields m_Fields;

    public SQLineInfo(
        DkmProcess _Process,
        ulong _Address
      ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public Int64Proxy Opcode
    {
      get
      {
        return GetFieldProxy(m_Fields._op);
      }
    }

    public Int64Proxy Line
    {
      get
      {
        return GetFieldProxy(m_Fields._line);
      }
    }
  }
}
