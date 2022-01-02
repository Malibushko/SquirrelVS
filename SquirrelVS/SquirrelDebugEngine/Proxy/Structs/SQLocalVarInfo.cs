using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQLocalVarInfo")]
  internal class SQLocalVarInfo : StructProxy
  {
    public class Fields
    {
      public StructField<SQString>    _name;
      public StructField<UInt64Proxy> _pos;
      public StructField<UInt64Proxy> _start_op;
      public StructField<UInt64Proxy> _end_op;
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

    public UInt64Proxy Position
    {
      get
      {
        return GetFieldProxy(m_Fields._pos, false);
      }
    }

    public UInt64Proxy StartOpcode
    {
      get
      {
        return GetFieldProxy(m_Fields._start_op);
      }
    }

    public UInt64Proxy EndOpcode
    {
      get
      {
        return GetFieldProxy(m_Fields._end_op);
      }
    }
  }
}
