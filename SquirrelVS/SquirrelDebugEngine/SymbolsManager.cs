using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Symbols;
using Microsoft.VisualStudio.Debugger.Evaluation;
using System.Linq;
using System.IO;
using System;

namespace SquirrelDebugEngine
{
  internal class SymbolsManager : DkmDataItem
  {
    static public DkmResolvedDocument[] FindDocuments(
       DkmModule       _Module,
       DkmSourceFileId _SourceField
     )
    {
      DkmModuleInstance ModuleInstance = _Module.GetModuleInstances()
                                          .OfType<DkmModuleInstance>()
                                          .FirstOrDefault(Module => Module.Module.CompilerId.VendorId == Guids.SquirrelCompilerID);

      if (ModuleInstance == null)
        return _Module.FindDocuments(_SourceField);

      DkmProcess       Process     = ModuleInstance.Process;
      LocalProcessData ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(Process);

      lock (ProcessData.Symbols)
      {
        foreach (var SquirrelVM in ProcessData.Symbols.SquirrelHandles)
        {
          foreach (var Source in SquirrelVM.Value.Scripts)
          {
            if (Source.Value.ResolvedFilename == null)
            {
              var ScriptSource = ProcessData.Symbols.FetchScriptSource(Source.Key);

              if (ScriptSource?.ResolvedFilename != null)
                Source.Value.ResolvedFilename = ScriptSource.ResolvedFilename;
              else
                throw new NotImplementedException($"Unable to locate {Source.Key}");
            }

            var Filename = Source.Value.ResolvedFilename;

            if (Filename == _SourceField.DocumentName)
            {
              var DataItem = new ResolvedDocumentItem
              {
                ScriptData = Source.Value
              };

              return new DkmResolvedDocument[1] {
                DkmResolvedDocument.Create(
                    _Module,
                    _SourceField.DocumentName,
                    null,
                    DkmDocumentMatchStrength.FullPath,
                    DkmResolvedDocumentWarning.None,
                    false,
                    DataItem)
              };
            }
          }
        }
      }

      return _Module.FindDocuments(_SourceField);
    }

    static public DkmInstructionSymbol[] FindSymbols(
        DkmResolvedDocument     _ResolvedDocument,
        DkmTextSpan             _TextSpan,
        string                  _Text,
        out DkmSourcePosition[] _SymbolLocation
      )
    {
      var SourceFileID = DkmSourceFileId.Create(_ResolvedDocument.DocumentName, null, null, null);

      var ResultSpan   = new DkmTextSpan(_TextSpan.StartLine, _TextSpan.StartLine, 0, 0);

      _SymbolLocation  = new DkmSourcePosition[1] { DkmSourcePosition.Create(SourceFileID, ResultSpan) };

      var BreakpointData = new SourceLocation
      {
        Source = _ResolvedDocument.GetDataItem<ResolvedDocumentItem>().ScriptData.SourceName,
        Line   = _TextSpan.StartLine
      };

      return new DkmInstructionSymbol[1] { DkmCustomInstructionSymbol.Create(_ResolvedDocument.Module, Guids.SquirrelRuntimeID, BreakpointData.Encode(), 0, null) };
    }

    static public DkmSourcePosition GetSourcePosition(
        DkmInstructionSymbol   _Instruction,
        DkmSourcePositionFlags _Flags,
        DkmInspectionSession   _Session,
        out bool               _StartOfLine
      )
    {
      var Process = _Session?.Process;

      if (Process == null)
      {
        DkmCustomModuleInstance ModuleInstance = _Instruction.Module.GetModuleInstances()
                                                    .OfType<DkmCustomModuleInstance>()
                                                    .FirstOrDefault(el => el.Module.CompilerId.VendorId == Guids.SquirrelCompilerID);

        if (ModuleInstance == null)
          return _Instruction.GetSourcePosition(_Flags, _Session, out _StartOfLine);

        Process = ModuleInstance.Process;
      }

      LocalProcessData ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(Process);

      var InstructionSymbol = _Instruction as DkmCustomInstructionSymbol;

      if (InstructionSymbol != null && InstructionSymbol.EntityId != null)
      {
        CallstackFrame CallData = new CallstackFrame();

        CallData.ReadFrom(InstructionSymbol.EntityId.ToArray());

        string FilePath = Path.Combine(ProcessData.WorkingDirectory, CallData.SourceName);

        _StartOfLine = true;

        return DkmSourcePosition.Create(
            DkmSourceFileId.Create(FilePath, null, null, null),
            new DkmTextSpan((int)CallData.Line, (int)CallData.Line, 0, 0)
          );
      }

      return _Instruction.GetSourcePosition(_Flags, _Session, out _StartOfLine);
    }
  }
}
