using Microsoft.VisualStudio.Debugger;
using System.Diagnostics;

namespace SquirrelDebugEngine.Proxy
{
  internal interface ISQObject : IDataProxy<StructProxy>
  {
  }

  [StructProxy(StructName = "tagSQObject")]
  internal class SQObject : StructProxy, ISQObject
  {
    internal class SQObjectFields
    {
#pragma warning disable 0649

      public StructField<SQObjectType> _type;
      public StructField<PointerProxy> _unVal;

#pragma warning restore 0649
    }

    private readonly SQObjectFields m_Fields;

    public SQObject(
          DkmProcess _Process,
          ulong _Address
        ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SquirrelVariableInfo.Type Type
    {
      get
      {
        if (Address == 0)
          return SquirrelVariableInfo.Type.Invalid;

        return GetFieldProxy(m_Fields._type).Read();
      }
    }

    public object Value
    {
      get
      {
        var ValueAddress = Address.OffsetBy(m_Fields._unVal.Offset);

        switch (Type)
        {
          case SquirrelVariableInfo.Type.Null:
            return null;
          case SquirrelVariableInfo.Type.Integer:
            return new Int64Proxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.Float:
            return new SingleProxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.UserPointer:
            return new PointerProxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.Bool:
            return new UInt64Proxy(Process, ValueAddress).Read() == 0;
        }

        /* Structs below are being stored as pointers so we have to read pointer value first */
        var SquirrelObjectAddress = new PointerProxy(Process, ValueAddress).Read();

        switch (Type)
        { 
          case SquirrelVariableInfo.Type.Table:
            break;
          case SquirrelVariableInfo.Type.Array:
            break;
          case SquirrelVariableInfo.Type.Closure:
            return new SQClosure(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Generator:
            break;
          case SquirrelVariableInfo.Type.NativeClosure:
            return new SQNativeClosure(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.String:
            return new SQString(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.UserData:
            break;
          case SquirrelVariableInfo.Type.FuncProto:
            return new SQFunctionProto(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Thread:
            break;
          case SquirrelVariableInfo.Type.Class:
            break;
          case SquirrelVariableInfo.Type.Instance:
            break;
          case SquirrelVariableInfo.Type.WeakRef:
            break;
          default:
            break;
        }

        return this;
      }
    }
  }
}
