﻿using Microsoft.VisualStudio.Debugger;
using System;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "sqvector<SQObjectPtr>")]
  internal class SQObjectPtrVec : StructProxy
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<PointerProxy<ArrayProxy<SQObjectPtr>>> _vals;
      public StructField<UInt64Proxy>                           _size;
      public StructField<UInt64Proxy>                           _allocated;

#pragma warning restore 0649
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

    public UInt64 Size
    {
      get
      {
        return GetFieldProxy(m_Fields._size).Read();
       }
    }

    public PointerProxy<ArrayProxy<SQObjectPtr>> Values
    {
      get
      {
        return GetFieldProxy(m_Fields._vals);
      }
    }
  }
}
