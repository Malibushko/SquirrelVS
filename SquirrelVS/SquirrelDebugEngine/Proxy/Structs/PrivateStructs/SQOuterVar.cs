using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQOuterVar")]
  internal class SQOuterVar : StructProxy
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<SQString>   _name;
      public StructField<Int64Proxy> _type;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQOuterVar(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQString Name
    {
      get
      {
        return GetFieldProxy(m_Fields._name);
      }
    }

    public Int64Proxy Type
    {
      get
      {
        return GetFieldProxy(m_Fields._type);
      }
    }
  }
}
