using Microsoft.VisualStudio.Debugger;
using System.Diagnostics;

namespace SquirrelDebugEngine.Proxy
{
  internal interface ISQObject : IValueStore<SQObject>, IDataProxy<StructProxy>
  {
    SQObjectType _type { get; }
  }

  [StructProxy(StructName = "tagSQObject")]
  internal class SQObject : StructProxy, ISQObject
  {
    public SQObjectType _type
    {
      get
      {
        return GetFieldProxy(m_Fields._type, false);
      }
    }
    internal class SQObjectFields
    {
      public StructField<SQObjectType> _type;
      public StructField<PointerProxy> _unVal;
    }

    private readonly SQObjectFields m_Fields;

    public SQObject(
          DkmProcess _Process, 
          ulong      _Address
        ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    static public SQObject FromAddress(
        DkmProcess _Process,
        ulong      _Address
      )
    {
      if (_Address == 0)
        return null;

      var Fields = GetStructFields<SQObject, SQObjectFields>(_Process);
      var Type   = new SQObjectType(_Process, _Address.OffsetBy(Fields._type.Offset));

      switch (Type.Read())
      {
        case SquirrelVariableInfo.Type.Null:
          break;
        case SquirrelVariableInfo.Type.Integer:
          return new SQInteger(_Process, _Address);
        case SquirrelVariableInfo.Type.Float:
          break;
        case SquirrelVariableInfo.Type.UserPointer:
          break;
        case SquirrelVariableInfo.Type.String:
          return new SQString(_Process, _Address);
        case SquirrelVariableInfo.Type.Closure:
          return new SQClosure(_Process, _Address);
        case SquirrelVariableInfo.Type.Array:
          break;
        case SquirrelVariableInfo.Type.NativeClosure:
          return new SQNativeClosure(_Process, _Address);
        case SquirrelVariableInfo.Type.Generator:
          break;
        case SquirrelVariableInfo.Type.UserData:
          break;
        case SquirrelVariableInfo.Type.Thread:
          break;
        case SquirrelVariableInfo.Type.Class:
          break;
        case SquirrelVariableInfo.Type.Instance:
          break;
        case SquirrelVariableInfo.Type.WeakRef:
          break;
        case SquirrelVariableInfo.Type.Bool:
          break;
        case SquirrelVariableInfo.Type.Table:
          break;
        case SquirrelVariableInfo.Type.FuncProto:
          return new SQFunctionProto(_Process, _Address);
        default:
          return null;
      }

      return null;
    }

    public SQObject Read()
    {
      return this;
    }

    [DebuggerDisplay("& {_type.Read()}")]
    public SQObjectType Type
    {
      get
      {
        return _type;
      }
    }

    public PointerProxy RawValue
    {
      get
      {
        return GetFieldProxy(m_Fields._unVal, false);
      }
    }
    public SQObject Value
    {
      get
      {
        return FromAddress(Process, Address);
      }
    }
  }
}
