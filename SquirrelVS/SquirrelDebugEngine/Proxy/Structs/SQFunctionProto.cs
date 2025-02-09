﻿using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQFunctionProto")]
  internal class SQFunctionProto : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<SQObjectPtr> _name;
      public StructField<SQObjectPtr> _sourcename;

      public StructField<Int64Proxy>                               _nlocalvarinfos;
      public StructField<PointerProxy<ArrayProxy<SQLocalVarInfo>>> _localvarinfos;

      public StructField<Int64Proxy>                           _noutervalues;
      public StructField<PointerProxy<ArrayProxy<SQOuterVar>>> _outervalues;

      public StructField<Int64Proxy>                           _nparameters;

      public StructField<PointerProxy>                         _instructions;
      
      public StructField<PointerProxy<ArrayProxy<SQLineInfo>>> _lineinfos;
      public StructField<Int64Proxy>                           _nlineinfos;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;
    public SQFunctionProto(
          DkmProcess _Process,
          ulong _Address
        ) :
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public SQObjectPtr Name
    {
      get
      {
        return GetFieldProxy(m_Fields._name);
      }
    }

    public SQObjectPtr SourceName
    {
      get
      {
        return GetFieldProxy(m_Fields._sourcename);
      }
    }

    public int LocalVariablesCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._nlocalvarinfos).Read();
      }
    }
    public int ParametersCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._nparameters).Read();
      }
    }

    public int OuterVariablesCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._noutervalues).Read();
      }
    }

    public PointerProxy<ArrayProxy<SQOuterVar>> OuterVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._outervalues);
      }
    }

    public PointerProxy<ArrayProxy<SQLocalVarInfo>> LocalVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._localvarinfos);
      }
    }

    public int LineInfosCount
    {
      get
      {
        return (int)GetFieldProxy(m_Fields._nlineinfos).Read();
      }
    }

    public PointerProxy<ArrayProxy<SQLineInfo>> LineInfos
    {
      get
      {
        return GetFieldProxy(m_Fields._lineinfos);
      }
    }

    public PointerProxy Instructions
    {
      get
      {
        return GetFieldProxy(m_Fields._instructions);
      }
    }

    public long GetLine(
        PointerProxy _InstructionPointer
      )
    {
      if (!_InstructionPointer.IsNull)
      {
        var   InstructionObjectSize = StructProxy.GetStructMetadata<SQInstruction>(Process).Size;

        var   LineInfo      = LineInfos.Read();
        var   LineInfoCount = LineInfosCount;
        
        long OpcodeNumber  = (long)(_InstructionPointer.Read() - Instructions.Address) / InstructionObjectSize;
        long Line          = LineInfo[0].Line.Read();

        for (int i = 1; i < LineInfoCount; i++)
        {
          if (LineInfo[i].Opcode.Read() >= OpcodeNumber)
            return Line;

          Line = LineInfo[i].Line.Read();
        }

        return Line;
      }

      return 1;
    }

    public SquirrelVariableInfo GetLocalVariable(
          SQVM         _Thread,
          int          _Index,
          long         _StackBase,
          PointerProxy _InstructionPointer
        )
    {
      var VariablesPtr = LocalVariables;

      if (VariablesPtr.IsNull)
        return null;

      var Variables      = VariablesPtr.Read();
      var VariablesCount = LocalVariablesCount;

      UInt64 InstructionObjectSize = (UInt64)SizeOf<SQInstruction>(Process);
      UInt64 OpcodeNumber          = (UInt64)((_InstructionPointer.Read() - Instructions.Address) / InstructionObjectSize) - 1;
      
      var Stack              = _Thread.Stack.Values.Read();
      int SkipVariablesCount = _Index;

      for (int j = 0; j < VariablesCount; j++)
      {
        var Variable = Variables[j];

        if (Variable.StartOpcode.Read() <= OpcodeNumber && Variable.EndOpcode.Read() >= OpcodeNumber)
        {
          if (SkipVariablesCount == 0)
          {
            SquirrelVariableInfo LocalVariable = new SquirrelVariableInfo()
            {
              Name  = (Variable.Name.Value as SQString).Read(),
              Value = Stack[_StackBase + (long)Variables[j].Position.Read()]
            };

            var VariableInfoType = LocalVariable.Value?.Type;

            if (VariableInfoType != null &&
                VariableInfoType != SquirrelVariableInfo.Type.Invalid &&
                VariableInfoType != SquirrelVariableInfo.Type.Null)
            {
              return LocalVariable;
            }
          }
          --SkipVariablesCount;
        }
      }

      return null;
   }
    public List<SquirrelVariableInfo> GetLocals(
          SQVM         _Thread,
          long         _StackBase,
          PointerProxy _InstructionPointer
        )
    {
      List<SquirrelVariableInfo> Locals         = new List<SquirrelVariableInfo>();
      var                        VariablesCount = LocalVariablesCount;

      for (int i = 0; i < VariablesCount; i++)
      {
        var LocalVariable = GetLocalVariable(_Thread, i, _StackBase, _InstructionPointer);

        if (LocalVariable != null)
          Locals.Add(LocalVariable);
      }

      Locals.Reverse();

      return Locals;
    }

    public List<string> GetInputParameterNames()
    {
      var VariablesPtr = LocalVariables;

      if (VariablesPtr.IsNull)
        return null;

      var Variables      = VariablesPtr.Read();
      var VariablesCount = LocalVariablesCount;

      List<string> Names = new List<string>();

      for (int i = VariablesCount - ParametersCount; i < VariablesCount - 1; i++)
        Names.Add((Variables[i].Name.Value as SQString).Read());

      Names.Reverse();

      return Names;
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.FuncProto.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQFunctionProto";
    }

    public string GetDisplayValue()
    {
      return Name.GetDisplayValue();
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
        Name   = "Name",
        Object = Name
      });

      Items.Add(new FieldDataItem
      {
        Name   = "Source Name",
        Object = SourceName
      });

      Items.Add(new FieldDataItem
      {
        Name = "Parameters count",
        Object = new PrimitiveVisualizationProxy<int>
        {
          Type   = SquirrelVariableInfo.Type.Integer.ToString(),
          Object = ParametersCount
        }
      });

      Items.Add(new FieldDataItem
      {
        Name = "Outer variables count",
        Object = new PrimitiveVisualizationProxy<int>
        {
          Type   = SquirrelVariableInfo.Type.Integer.ToString(),
          Object = OuterVariablesCount
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
