using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using System.Linq;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQFunctionProto")]
  internal class SQFunctionProto : SQObject
  {
    private class Fields
    {
      public StructField<SQObject> _name;
      public StructField<SQObject> _sourcename;

      public StructField<Int64Proxy>                               _nlocalvarinfos;
      public StructField<PointerProxy<ArrayProxy<SQLocalVarInfo>>> _localvarinfos;

      public StructField<Int64Proxy>                           _noutervalues;
      public StructField<PointerProxy<ArrayProxy<SQOuterVar>>> _outervalues;

      public StructField<Int64Proxy>                           _nparameters;

      public StructField<PointerProxy>                         _instructions;
      
      public StructField<PointerProxy<ArrayProxy<SQLineInfo>>> _lineinfos;
      public StructField<Int64Proxy>                           _nlineinfos;
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

    public SQObject Name
    {
      get
      {
        return GetFieldProxy(m_Fields._name, RawValue.Read());
      }
    }

    public SQObject SourceName
    {
      get
      {
        return GetFieldProxy(m_Fields._sourcename, RawValue.Read());
      }
    }

    public int LocalVariablesCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._nlocalvarinfos, RawValue.Read()).Read();
      }
    }
    public int ParametersCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._nparameters, RawValue.Read()).Read();
      }
    }

    public int OuterVariablesCount
    {
      get
      {
        return (int)GetFieldProxy<Int64Proxy>(m_Fields._noutervalues, RawValue.Read()).Read();
      }
    }

    public PointerProxy<ArrayProxy<SQOuterVar>> OuterVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._outervalues, RawValue.Read());
      }
    }

    public PointerProxy<ArrayProxy<SQLocalVarInfo>> LocalVariables
    {
      get
      {
        return GetFieldProxy(m_Fields._localvarinfos, RawValue.Read());
      }
    }

    public int LineInfosCount
    {
      get
      {
        return (int)GetFieldProxy(m_Fields._nlineinfos, RawValue.Read()).Read();
      }
    }

    public PointerProxy<ArrayProxy<SQLineInfo>> LineInfos
    {
      get
      {
        return GetFieldProxy(m_Fields._lineinfos, RawValue.Read());
      }
    }

    public PointerProxy Instructions
    {
      get
      {
        return GetFieldProxy(m_Fields._instructions, RawValue.Read());
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

    public List<string> GetInputParameterNames()
    {
      var VariablesPtr = LocalVariables;

      if (VariablesPtr.IsNull)
        return null;

      var Variables      = VariablesPtr.Read();
      var VariablesCount = LocalVariablesCount;

      List<string> Names = new List<string>();

      for (int i = VariablesCount - ParametersCount; i < VariablesCount - 1; i++)
      {
        var VariableName = Variables[i].Name;

        if (VariableName?.Type.Read() != SquirrelVariableInfo.Type.String)
        {
          Names.Add("<failed to get variable name>");
          continue;
        }

        Names.Add((VariableName.Value as SQString).Read());
      }
      
      Names.Reverse();

      return Names;
    }
  }
}
