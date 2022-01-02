﻿using System;
using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQString")]
  internal class SQString : StructProxy, ISQObject
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

    public string Read()
    {
      return Utility.ReadStringVariable(Process, Address.OffsetBy(m_Fields._val.Offset), 1024);
    }
  }
}
