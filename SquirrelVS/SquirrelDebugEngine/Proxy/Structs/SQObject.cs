using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Evaluation;

namespace SquirrelDebugEngine.Proxy
{
  internal interface ISQObject : IDataProxy<StructProxy>
  {

  }

  internal interface IVisualizableObject
  {
    string GetDisplayType();

    string GetDisplayNativeType();

    string GetDisplayValue();

    DkmEvaluationFlags GetEvaluationFlags();

    ExpandableDataItem[] GetChildren();
  }

  internal class ExpandableDataItem : DkmDataItem
  {
    public IVisualizableObject NativeObject;
    public string              Name;

  }

  [StructProxy(StructName = "tagSQObject")]
  internal class SQObject : StructProxy, ISQObject, IVisualizableObject
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
            return new SQNull(Process, ValueAddress);
          case SquirrelVariableInfo.Type.Integer:
            return new Int64Proxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.Float:
            return new SingleProxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.UserPointer:
          case SquirrelVariableInfo.Type.UserData:
            return new PointerProxy(Process, ValueAddress).Read();
          case SquirrelVariableInfo.Type.Bool:
            return new UInt64Proxy(Process, ValueAddress).Read() == 0;
        }

        /* Structs below are being stored as pointers so we have to read pointer value first */
        var SquirrelObjectAddress = new PointerProxy(Process, ValueAddress).Read();

        switch (Type)
        { 
          case SquirrelVariableInfo.Type.Table:
            return new SQTable(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Array:
            return new SQArray(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Closure:
            return new SQClosure(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Generator:
            break;
          case SquirrelVariableInfo.Type.NativeClosure:
            return new SQNativeClosure(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.String:
            return new SQString(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.FuncProto:
            return new SQFunctionProto(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Thread:
            break;
          case SquirrelVariableInfo.Type.Class:
            return new SQClass(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.Instance:
            return new SQInstance(Process, SquirrelObjectAddress);
          case SquirrelVariableInfo.Type.WeakRef:
            return new SQWeakRef(Process, SquirrelObjectAddress);
          default:
            break;
        }

        return null;
      }
    }

    public ulong ValueAddress
    {
      get
      {
        return Address.OffsetBy(m_Fields._unVal.Offset);
      }
    }

    public bool IsValueVizualizable()
    {
      return Value is IVisualizableObject;
    }

    public string GetDisplayType()
    {
      return IsValueVizualizable() ? (Value as IVisualizableObject).GetDisplayType() : Type.ToString();
    }

    public string GetDisplayValue()
    {
      return IsValueVizualizable() ? (Value as IVisualizableObject).GetDisplayValue() : Value.ToString();
    }

    public string GetDisplayNativeType()
    {
      return IsValueVizualizable() ? (Value as IVisualizableObject).GetDisplayNativeType() : Type.ToString();
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return IsValueVizualizable() ? (Value as IVisualizableObject).GetEvaluationFlags() : DefaultEvaluationFlags;
    }

    public ExpandableDataItem[] GetChildren()
    {
      return IsValueVizualizable() ? (Value as IVisualizableObject).GetChildren() : new ExpandableDataItem[0];
    }

    public static DkmEvaluationFlags DefaultEvaluationFlags
    {
      get
      {
        return new DkmEvaluationFlags()
        {
          Flags = DkmEvaluationResultFlags.ReadOnly,
          Category = DkmEvaluationResultCategory.Data,
          AccessType = DkmEvaluationResultAccessType.Public,
          StorageType = DkmEvaluationResultStorageType.None,
          TypeModifierFlags = DkmEvaluationResultTypeModifierFlags.None
        };
      }
    }

  }
}
