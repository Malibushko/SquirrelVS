using Microsoft.VisualStudio.Debugger;
using System;

namespace SquirrelDebugEngine.Proxy
{
  internal class SQObjectType : IWritableDataProxy<SquirrelVariableInfo.Type>
  {
    public long ObjectSize
    {
      get
      {
        return EvaluationHelpers.Is64Bit(Process) ? sizeof(ulong) : sizeof(int);
      }
    }
    public DkmProcess Process { get; private set; }
    public ulong Address { get; private set; }

    public SQObjectType(
        DkmProcess _Process,
        ulong      _Address
      )
    {
      Process = _Process;
      Address = _Address;
    }

    public unsafe SquirrelVariableInfo.Type Read()
    {
      return EvaluationHelpers.Is64Bit(Process) ? (SquirrelVariableInfo.Type)Utility.ReadUlongVariable(Process, Address).GetValueOrDefault(0) :
                                                  (SquirrelVariableInfo.Type)Utility.ReadIntVariable(Process, Address).GetValueOrDefault(0);
    }

    object IValueStore.Read()
    {
      return Read();
    }

    public void Write(
        SquirrelVariableInfo.Type _Value
      )
    {
      if (EvaluationHelpers.Is64Bit(Process))
        Utility.TryWriteUlongVariable(Process, Address, (ulong)_Value);
      else
        Utility.TryWriteIntVariable(Process, Address, (int)_Value);
    }

    public void Write(
        object _Value
      )
    {
      Write((SquirrelVariableInfo.Type)_Value);
    }
  }
}
