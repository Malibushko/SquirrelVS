using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Evaluation;

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

    public ulong ValueAddress
    {
      get
      {
        return Address.OffsetBy(m_Fields._unVal.Offset);
      }
    }

    private string NativeTypeString
    {
      get
      {
        switch (Type)
        {
          case SquirrelVariableInfo.Type.Closure:
            return "SQClosure *";
          case SquirrelVariableInfo.Type.Array:
            return "SQArray *";
          case SquirrelVariableInfo.Type.NativeClosure:
            return "SQNativeClosure *";
          case SquirrelVariableInfo.Type.Generator:
            return "SQGenerator *";
          case SquirrelVariableInfo.Type.UserData:
            return "SQUserData *";
          case SquirrelVariableInfo.Type.Thread:
            return "SQVM *";
          case SquirrelVariableInfo.Type.Class:
            return "SQClass *";
          case SquirrelVariableInfo.Type.Instance:
            return "SQInstance *";
          case SquirrelVariableInfo.Type.Table:
            return "SQTable *";
          case SquirrelVariableInfo.Type.FuncProto:
            return "SQFunctionProto *";
        }

        return "<invalid>";
      }
    }

    public SquirrelVariableEvaluatorData EvaluationData
    {
      get
      {
        SquirrelVariableEvaluatorData Data = new SquirrelVariableEvaluatorData()
        {
          Flags             = DkmEvaluationResultFlags.ReadOnly,
          Category          = DkmEvaluationResultCategory.Data,
          AccessType        = DkmEvaluationResultAccessType.Public,
          StorageType       = DkmEvaluationResultStorageType.None,
          TypeModifierFlags = DkmEvaluationResultTypeModifierFlags.None
        };

        switch (Type)
        {
          case SquirrelVariableInfo.Type.Invalid:
          {
            Data.Type = "<invalid>";
            Data.Value = "<invalid>";
            
            break;
          }
          case SquirrelVariableInfo.Type.Null:
          {
            Data.Type  = Type.ToString();
            Data.Value = "null";

            break;
          }
          case SquirrelVariableInfo.Type.Bool:
          {
            Data.Flags |= DkmEvaluationResultFlags.Boolean;

            goto case SquirrelVariableInfo.Type.Float;
          }
          case SquirrelVariableInfo.Type.WeakRef:
          case SquirrelVariableInfo.Type.UserPointer:
          case SquirrelVariableInfo.Type.Integer:
          case SquirrelVariableInfo.Type.Float:
          {
            Data.Type  = Type.ToString();
            Data.Value = Value.ToString();

            break; 
          }
          case SquirrelVariableInfo.Type.String:
          {
            Data.Type = Type.ToString();
            Data.Value = "\"" + (Value as SQString).Read() + "\"";

            break;
          }
          case SquirrelVariableInfo.Type.Closure:
          case SquirrelVariableInfo.Type.Array:
          case SquirrelVariableInfo.Type.NativeClosure:
          case SquirrelVariableInfo.Type.Generator:
          case SquirrelVariableInfo.Type.UserData:
          case SquirrelVariableInfo.Type.Thread:
          case SquirrelVariableInfo.Type.Class:
          case SquirrelVariableInfo.Type.Instance:
          case SquirrelVariableInfo.Type.Table:
          case SquirrelVariableInfo.Type.FuncProto:
          {
            Data.Value = "{...}";
            Data.Flags |= DkmEvaluationResultFlags.Expandable;

            Data.Type = NativeTypeString;
            break;
          }
        }

        return Data;
      }
    }
    
  }
}
