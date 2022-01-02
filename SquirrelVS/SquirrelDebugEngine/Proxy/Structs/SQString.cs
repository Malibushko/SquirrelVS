using System;
using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQString")]
  internal class SQString : SQObject
  {
    public class Fields
    {
      public StructField<PointerProxy> _val;
    }

    private readonly Fields m_Fields;

    public SQString(
        DkmProcess _Process,
        ulong      _Address
      ) : base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public new string Read()
    {
      var StringAddress = RawValue.Read() + (ulong)m_Fields._val.Offset;
      
      if (StringAddress != 0)
        return Utility.ReadStringVariable(Process, StringAddress, 1024);

      return null;
    }

    public override object ReadValue()
    {
      return Read();
    }
  }
}
