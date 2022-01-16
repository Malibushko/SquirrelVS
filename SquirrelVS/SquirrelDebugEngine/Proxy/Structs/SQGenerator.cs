using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using System;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQGenerator")]
  internal class SQGenerator : StructProxy, ISQObject, IVisualizableObject
  {
    public enum EGeneratorState
    {
      Running   = 0,
      Suspended = 1,
      Dead      = 2
    };

    public class Fields
    {
#pragma warning disable 0649
      
      public StructField<SQObjectPtr>    _closure;
      public StructField<SQObjectPtrVec> _stack;
      public StructField<SQObjectPtrVec> _vargsstack;
      public StructField<Int64Proxy>     _state;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQGenerator(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Closure
    {
      get
      {
        return GetFieldProxy(m_Fields._closure);
      }
    }

    public SQObjectPtrVec Stack
    {
      get
      {
        return GetFieldProxy(m_Fields._stack);
      }
    }

    public SQObjectPtrVec VariableArgumentsStack
    {
      get
      {
        return GetFieldProxy(m_Fields._vargsstack);
      }
    }

    public EGeneratorState State
    {
      get
      {
        return (EGeneratorState)GetFieldProxy(m_Fields._state).Read();
      }
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Generator.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQGenerator";
    }

    public string GetDisplayValue()
    {
      return $"[Generator ({State.ToString()})]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.ExpandableEvaluationFlags;
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      Items.Add(new FieldDataItem
      {
        Name         = "[Closure]",
        Object = Closure
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Stack]",
        Object = Stack
      });

      Items.Add(new FieldDataItem
      {
        Name         = "[Variable Arguments Stack]",
        Object = VariableArgumentsStack
      });

      Items.Add(new FieldDataItem
      {
        Name = "State",
        Object = new PrimitiveVisualizationProxy<EGeneratorState>
        {
          Type       = "Integer",
          Object     = State,
          NativeType = "SQInteger"
        }
      });

      return Items.ToArray();
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
