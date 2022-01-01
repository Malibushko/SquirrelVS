﻿using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "sqvector<SQObjectPtr>")]
  internal class SQObjectPtrVec : StructProxy
  {
    public class Fields
    {
      public StructField<PointerProxy> _vals;
      public StructField<UInt64Proxy>  _size;
      public StructField<UInt64Proxy>  _allocated;
    }

    private readonly Fields m_Fields;

    public SQObjectPtrVec(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }
  }
}