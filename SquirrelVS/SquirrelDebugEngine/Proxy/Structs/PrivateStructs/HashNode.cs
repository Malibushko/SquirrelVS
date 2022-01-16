using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQTable::_HashNode")]
  internal class HashNode : StructProxy
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr> val;
      public StructField<SQObjectPtr> key;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;
    
    public HashNode(
        DkmProcess _Process,
        ulong _Address
      ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Key
    {
      get
      {
        return GetFieldProxy(m_Fields.key);
      }
    }

    public SQObjectPtr Value
    {
      get
      {
        return GetFieldProxy(m_Fields.val);
      }
    }
  }
}
