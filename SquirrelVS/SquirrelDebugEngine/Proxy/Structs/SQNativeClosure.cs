﻿using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQNativeClosure")]
  internal class SQNativeClosure : StructProxy, ISQObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr> _function;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQNativeClosure(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Function
    {
      get
      {
        return GetFieldProxy(m_Fields._function);
      }
    }
  }
}
