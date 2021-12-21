using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;
using Microsoft.VisualStudio.Debugger.Symbols;
using System.Text;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  public class LocalComponent : 
      IDkmCallStackFilter, 
      IDkmModuleInstanceLoadNotification, 
      IDkmCustomMessageCallbackReceiver, 
      IDkmSymbolQuery, 
      IDkmSymbolCompilerIdQuery, 
      IDkmSymbolDocumentCollectionQuery, 
      IDkmSymbolDocumentSpanQuery, 
      IDkmModuleUserCodeDeterminer, 
      IDkmSymbolHiddenAttributeQuery,
      IDkmLanguageExpressionEvaluator
  {
    private bool IsInjectRequested = false;

    #region Interface
    public void OnModuleInstanceLoad(
        DkmModuleInstance   _ModuleInstance,
        DkmWorkList         _WorkList,
        DkmEventDescriptorS _EventDescriptor)
    {
      DkmNativeModuleInstance NativeModuleInstance = _ModuleInstance as DkmNativeModuleInstance;

      if (NativeModuleInstance == null)
        return;

      string           ModuleName  = NativeModuleInstance.FullName;
      DkmProcess       Process     = NativeModuleInstance.Process;
      LocalProcessData ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(NativeModuleInstance.Process);

      if (ModuleName != null)
      {
        if (ModuleName.EndsWith("kernel32.dll"))
        {
          // Save load library address
          ProcessData.LoadLibraryWAddress = Utility.FindFunctionAddress(Process.GetNativeRuntimeInstance(), "LoadLibraryW");
        }
        else
        if (ModuleName.EndsWith("SquirrelDebugHelper.dll"))
        {
          // Init helper
          InitalizeDebugHelper(NativeModuleInstance, ProcessData);
        }
        else
        if (ModuleName.EndsWith(".exe") || ModuleName.Contains("squirrel"))
        {
          Debug.WriteLine("Trying to create runtime");

          DkmCustomMessage.Create(
              Process.Connection,
              Process,
              MessageToRemote.Guid,
              (int)MessageToRemote.MessageType.CreateRuntime,
              null,
              null
            ).SendLower();

          if (Process.LivePart == null)
            return;

          TryInitSquirrelModule(NativeModuleInstance, ProcessData);
        }

        if (ProcessData.SquirrelModule != null && ProcessData.LoadLibraryWAddress != 0)
          InjectDebugHelper(Process, ProcessData);
      }
    }

    public DkmCustomMessage SendHigher(
        DkmCustomMessage _Message
      )
    {
      switch ((MessageToLocal.MessageType)_Message.MessageCode)
      {
        case MessageToLocal.MessageType.BreakpointHit:
        {
            BreakpointHitData Data = new BreakpointHitData();
            
            Data.Decode(_Message.Parameter1 as byte[]);

            OnBreakPointHit(_Message.Process, Data);

            break;
        }

        case MessageToLocal.MessageType.ComponentException:
        {
          string Message = Encoding.UTF8.GetString(_Message.Parameter1 as byte[]);

          Debug.WriteLine($"Exception in RemoteComponent: '{Message}'");
          break;
        }

        case MessageToLocal.MessageType.Symbols:
        {
          LocalProcessData Data = Utility.GetOrCreateDataItem<LocalProcessData>(_Message.Process);
          var RuntimeInstances = _Message.Process.GetRuntimeInstances();

          string Message = _Message.Parameter1 as string;

          Debug.WriteLine($"Message from RemoteComponent: '{Message}'");
          break;
        }

        case MessageToLocal.MessageType.FetchCallstack:
        {
          LocalProcessData  Data      = Utility.GetOrCreateDataItem<LocalProcessData>(_Message.Process);
          SquirrelCallStack Callstack = Utility.GetOrCreateDataItem<SquirrelCallStack>(_Message.Process);
          
          Callstack.Callstack.Clear();

          ulong? FrameCount = Utility.ReadUlongVariable(_Message.Process, Data.SquirrelLocations.CallstackSizeAddress);

          if (FrameCount.HasValue && FrameCount.Value > 0)
          {
            int Offset = 0;
            for (int i = 0; i < (int)FrameCount.Value; ++i)
            {
              Callstack.Callstack.Push(new CallstackFrame(_Message.Process, Data.SquirrelLocations.CallstackAddress + (ulong)Offset));
              
              Offset += 24;
            }
          }

          break;
        }
      }

      return null;
    }
    #endregion


    #region Service
    private bool TryInitSquirrelModule(
        DkmNativeModuleInstance _Module,
        LocalProcessData        _ProcessData
      )
    {
      DkmWorkerProcessConnection Connection = DkmWorkerProcessConnection.GetLocalSymbolsConnection();

      if (Connection != null)
      {
          DkmCustomMessage Reply = DkmCustomMessage.Create(
            _Module.Process.Connection, 
            _Module.Process,
            MessageToLocalWorker.Guid,
            (int)MessageToLocalWorker.MessageType.FetchSquirrelSymbols,
            _Module.UniqueId.ToByteArray(),
            null,
            null,
            Connection
          ).SendLower();

        if (Reply != null)
        {
          SquirrelLocations Locations = Reply.Parameter1 as SquirrelLocations;

          _ProcessData.SquirrelModule    = _Module;
          _ProcessData.SquirrelLocations = Locations;
        }
      }

      return false;
    }

    private bool InjectDebugHelper(
        DkmProcess       _Process,
        LocalProcessData _ProcessData
      )
    {
      if (IsInjectRequested == true)
        return false;

      IsInjectRequested = true;

      if (_ProcessData.SquirrelLocations != null)
      {
        SquirrelBreakpoints BreakpointsInfo = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);

        BreakpointsInfo.SquirrelOpenBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_open",
          "Create new squirrel vm",
          _ProcessData.SquirrelLocations.OpenEndLocation).GetValueOrDefault(Guid.Empty);

        BreakpointsInfo.SquirrelCloseBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_close",
          "Close squirrel vm",
          _ProcessData.SquirrelLocations.CloseStartLocation).GetValueOrDefault(Guid.Empty);

        BreakpointsInfo.SquirrelLoadFileBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_compilebuffer",
          "Loads a new script",
          _ProcessData.SquirrelLocations.LoadFileEndLocation).GetValueOrDefault(Guid.Empty);

        string AssemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        string DLLPathName = Path.Combine(AssemblyFolder, "SquirrelDebugHelper.dll");

        if (!File.Exists(DLLPathName))
          return false;

        var DLLNameAddress = _Process.AllocateVirtualMemory(0ul, 4096, 0x3000, 0x04);

        byte[] bytes = Encoding.Unicode.GetBytes(DLLPathName);

        _Process.WriteMemory(DLLNameAddress, bytes);
        _Process.WriteMemory(DLLNameAddress + (ulong)bytes.Length, new byte[2] { 0, 0 });

        string ExePathName = Path.Combine(AssemblyFolder, "SquirrelDebugAttacher.exe");

        if (!File.Exists(ExePathName))
          return false;

        var ProcessStartInfo = new ProcessStartInfo(ExePathName, $"{_Process.LivePart.Id} {_ProcessData.LoadLibraryWAddress} {DLLNameAddress} \"{DLLPathName}\"")
        {
          CreateNoWindow = false,
          RedirectStandardError = false,
          RedirectStandardInput = true,
          RedirectStandardOutput = false,
          UseShellExecute = false
        };

        try
        {
          var AttachProcess = Process.Start(ProcessStartInfo);

          AttachProcess.WaitForExit();

          if (AttachProcess.ExitCode != 0)
            return false;
        }
        catch (Exception Exception)
        {
          return false;
        }
      }

      return true;
    }

    private bool InitalizeDebugHelper(
        DkmNativeModuleInstance _Module,
        LocalProcessData        _ProcessData
      )
    {
      var IsInitialzeAddress = _Module.FindExportName("IsInitialized", false);
      var Process            = _Module.Process;

      if (IsInitialzeAddress == null)
        return false;

      SquirrelBreakpoints HelperBreaks = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Module.Process);

      HelperBreaks.SquirrelHelperStepComplete  = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepComplete").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperAsyncBreak    = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperAsyncBreak").GetValueOrDefault(Guid.Empty);

      HelperBreaks.WorkingDirectoryAddress               = AttachmentHelpers.FindVariableAddress(_Module, "WorkingDirectory");
      HelperBreaks.SquirrelHitBreakpointIndex            = AttachmentHelpers.FindVariableAddress(_Module, "HitBreakpointIndex");
      HelperBreaks.SquirrelActiveBreakpointsCountAddress = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpointsCount");
      HelperBreaks.SquirrelActiveBreakpointsAddress      = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpoints");

      HelperBreaks.SquirrelStackInfoAddress      = AttachmentHelpers.FindVariableAddress(_Module, "StackInfo");
      HelperBreaks.StepperStateAddress           = AttachmentHelpers.FindVariableAddress(_Module, "StepperState");

      DkmCustomMessage.Create(
          Process.Connection,
          Process, 
          MessageToRemote.Guid,
          (int)MessageToRemote.MessageType.BreakpointsInfo,
          HelperBreaks.Encode(),
          null
        ).SendLower();

      _ProcessData.HelperStartAddress = _Module.BaseAddress;
      _ProcessData.HelperEndAddress   = _ProcessData.HelperStartAddress + _Module.Size;

      _ProcessData.SquirrelLocations.HelperStartAddress = _ProcessData.HelperStartAddress;
      _ProcessData.SquirrelLocations.HelperEndAddress   = _ProcessData.HelperEndAddress;

      _ProcessData.SquirrelLocations.HelperSQUnicodeFlagAddress = AttachmentHelpers.FindVariableAddress(_Module, "IsSQUnicode");
      _ProcessData.SquirrelLocations.StringBufferAddress        = AttachmentHelpers.FindVariableAddress(_Module, "StringBuffer");
      _ProcessData.SquirrelLocations.CallstackAddress           = AttachmentHelpers.FindVariableAddress(_Module, "Callstack");
      _ProcessData.SquirrelLocations.CallstackSizeAddress       = AttachmentHelpers.FindVariableAddress(_Module, "CallstackSize");
      
      DkmCustomMessage.Create(
          _Module.Process.Connection,
          _Module.Process,
          MessageToRemote.Guid,
          (int)MessageToRemote.MessageType.Locations,
          _ProcessData.SquirrelLocations.Encode(),
          null
        ).SendLower();

      _ProcessData.TraceRoutineAddress = AttachmentHelpers.FindFunctionAddress(_Module, "TraceRoutine");

      var InitializedValue = Utility.ReadUintVariable(Process, IsInitialzeAddress.CPUInstructionPart.InstructionPointer);

      if (InitializedValue.HasValue)
      {
        switch (InitializedValue.Value)
        {
          case 0: // Not Initialized
          {
            var OnInitializedBreakpoint = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperInitialized");

            if (!OnInitializedBreakpoint.HasValue)
              return false;

            HelperBreaks.SquirrelHelperInitialized = OnInitializedBreakpoint.Value;
              
            _ProcessData.HelperState               = HelperState.WaitingForInitialization;
            break; 
          }
          case 1:
          {
              _ProcessData.HelperState = HelperState.Initialized;
              break;  
          }
        }
      }
      return true;
    }

    void RegisterSquirrelState(
        DkmProcess           _Process,
        DkmInspectionSession _Session, 
        DkmThread            _Thread, 
        DkmStackWalkFrame    _Frame, 
        ulong?               _SquirrelHandleAddress
      )
    {
      LocalProcessData LocalData = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);

      ulong? _debughook_native = EvaluationHelpers.TryEvaluateAddressExpression(
          $"&((SQVM*){_SquirrelHandleAddress})->_debughook_native",
          _Session,
          _Thread,
          _Frame,
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        );

      ulong? _debughook = EvaluationHelpers.TryEvaluateAddressExpression(
          $"&((SQVM*){_SquirrelHandleAddress})->_debughook",
          _Session, 
          _Thread, 
          _Frame, 
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        );

      long? IsSQUnicode = EvaluationHelpers.TryEvaluateNumberExpression(
          "sizeof(SQChar) == sizeof(wchar_t)", 
          _Session,
          _Thread,
          _Frame,
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        );

      if (IsSQUnicode.HasValue && LocalData.SquirrelLocations.HelperSQUnicodeFlagAddress != 0)
        Utility.TryWriteByteVariable(_Process, LocalData.SquirrelLocations.HelperSQUnicodeFlagAddress, IsSQUnicode.Value == 0 ? (byte)0 : (byte)1);

      if (!_debughook_native.HasValue || !_debughook.HasValue)
        return;

      LocalData.HookData = new HookData
      {
        TraceRoutineFlagAddress = _debughook_native.Value,
        TraceRoutine            = _debughook.Value,
        TraceRoutineAddress     = LocalData.TraceRoutineAddress
      };

      if (LocalData.HelperState != HelperState.Initialized)
      {
        LocalData.SquirrelThread        = _Thread;
        LocalData.SquirrelHandleAddress = _SquirrelHandleAddress.Value;

        _Thread.Suspend(true);

        return;
      }

      LocalData.Symbols.FetchOrCreate(_SquirrelHandleAddress.Value);

      DkmCustomMessage.Create(
          _Process.Connection,
          _Process,
          MessageToRemote.Guid,
          (int)MessageToRemote.MessageType.RegisterState,
          LocalData.HookData.Encode(),
          null
        ).SendLower();
    }

    #endregion


    #region Symbol Provider
    object IDkmSymbolQuery.GetSymbolInterface(
        DkmModule _Module, 
        Guid      _InterfaceID
      )
    {
      return _Module.GetSymbolInterface(_InterfaceID);
    }

    DkmSourcePosition IDkmSymbolQuery.GetSourcePosition(
        DkmInstructionSymbol   _Instruction, 
        DkmSourcePositionFlags _Flags, 
        DkmInspectionSession   _Session, 
        out bool               _StartOfLine
      )
    {
      return SymbolsManager.GetSourcePosition(_Instruction, _Flags, _Session, out _StartOfLine);
    }

    DkmCompilerId IDkmSymbolCompilerIdQuery.GetCompilerId(
        DkmInstructionSymbol _Instruction, 
        DkmInspectionSession _Session
      )
    {
      return new DkmCompilerId(Guids.SquirrelCompilerID, Guids.SquirrelLanguageID);
    }

    DkmResolvedDocument[] IDkmSymbolDocumentCollectionQuery.FindDocuments(
        DkmModule       _Module, 
        DkmSourceFileId _SourceField
      )
    {
      return SymbolsManager.FindDocuments(_Module, _SourceField);
    }

    DkmInstructionSymbol[] IDkmSymbolDocumentSpanQuery.FindSymbols(
        DkmResolvedDocument _ResolvedDocument, 
        DkmTextSpan         _TextSpan, 
        string              _Text, 
        out DkmSourcePosition[] _SymbolLocation
      )
    {
      return SymbolsManager.FindSymbols(_ResolvedDocument, _TextSpan, _Text, out _SymbolLocation);
    }

    bool IDkmModuleUserCodeDeterminer.IsUserCode(
        DkmModuleInstance _Module
      )
    {
      return true;
    }

    #endregion

    #region Events

    bool OnBreakPointHit(
          DkmProcess        _Process,
          BreakpointHitData _BreakpointData
        )
    {
      SquirrelBreakpoints KnownBreakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);
      LocalProcessData    ProcessData      = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      DkmThread           Thread           = _Process.GetThreads().FirstOrDefault(_Thread => _Thread.UniqueId == _BreakpointData.ThreadID);

      if (_BreakpointData.BreakpointID == KnownBreakpoints.SquirrelHelperInitialized)
      {
        ProcessData.HelperState                  = HelperState.Initialized;
        ProcessData.HookData.TraceRoutineAddress = ProcessData.TraceRoutineAddress;

        DkmCustomMessage.Create(
            _Process.Connection,
            _Process,
            MessageToRemote.Guid,
            (int)MessageToRemote.MessageType.RegisterState,
            ProcessData.HookData.Encode(),
            null
          ).SendLower();

        ProcessData.WorkingDirectory = Utility.ReadStringVariable(_Process, KnownBreakpoints.WorkingDirectoryAddress, 1024);

        ProcessData.SquirrelThread.Resume(true);

        return true;
      }

      if (_BreakpointData.BreakpointID == KnownBreakpoints.SquirrelOpenBreakpoint)
      {
        var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, _BreakpointData, out DkmStackWalkFrame _Frame);
        
        ulong? SquirrelHandleAddress = EvaluationHelpers.TryEvaluateAddressExpression(
            $"v",
            InspectionSession, 
            Thread, 
            _Frame, 
            DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
          );

        if (!SquirrelHandleAddress.HasValue)
          SquirrelHandleAddress = EvaluationHelpers.TryEvaluateAddressExpression("@rax", InspectionSession, Thread, _Frame, DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects);
        
        if (SquirrelHandleAddress != 0)
          RegisterSquirrelState(_Process, InspectionSession, Thread, _Frame, SquirrelHandleAddress);

        InspectionSession.Close();

        return true;
      }
      
      if (_BreakpointData.BreakpointID == KnownBreakpoints.SquirrelLoadFileBreakpoint)
      {
        var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, _BreakpointData, out DkmStackWalkFrame Frame);

        var SourceNameAddress = EvaluationHelpers.TryEvaluateAddressExpression("filename", InspectionSession, Thread, Frame, DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects);

        if (!SourceNameAddress.HasValue)
          throw new Exception("Unable to locate source name");

        string SourceName = Utility.ReadStringVariable(_Process, SourceNameAddress.Value, 256);

        string SourcePath = Path.Combine(ProcessData.WorkingDirectory, SourceName);

        string ScriptContent = File.ReadAllText(SourcePath);
        
        var Message = DkmCustomMessage.Create(
            _Process.Connection,
            _Process,
            Guid.Empty,
            1,
            Encoding.UTF8.GetBytes(SourcePath),
            null
          );

        SymbolsVM Symbols = ProcessData.Symbols.FetchOrCreate(ProcessData.SquirrelHandleAddress);
        
        Symbols.AddScriptSource(SourceName, ScriptContent, null);
        Symbols.FetchScriptSource(SourceName).ResolvedFilename = SourcePath;

        Message.SendToVsService(Guids.SquirelDebuggerComponentID, true);
      }

      return false;
    }

    void IDkmSymbolHiddenAttributeQuery.IsHiddenCode(
        DkmInstructionSymbol instruction, 
        DkmWorkList workList, 
        DkmInspectionSession inspectionSession, 
        DkmInstructionAddress instructionAddress, 
        DkmCompletionRoutine<DkmIsHiddenCodeAsyncResult> completionRoutine
      )
    {
      try
      {
        completionRoutine.Invoke(new DkmIsHiddenCodeAsyncResult(Microsoft.VisualStudio.Debugger.Clr.DkmNonUserCodeFlags.None, null));
        
      } catch (Exception Exception)
      {
        Debug.WriteLine($"Expection thrown in IsHiddenCode: ${Exception.Message}");
      }
    }

    DkmStackWalkFrame[] IDkmCallStackFilter.FilterNextFrame(
        DkmStackContext   _StackContext, 
        DkmStackWalkFrame _NativeFrame
      )
    {
      if (_NativeFrame == null)
        return null;

      var Filter = Utility.GetOrCreateDataItem<CallStackFilter>(_NativeFrame.Process);

      try
      {
        if (Filter != null)
          return Filter.FilterNextFrame(_StackContext, _NativeFrame);
      }
      catch (DkmException _Exception)
      {
        // TODO: Add log
      }

      return new DkmStackWalkFrame[1] { _NativeFrame };
    }

    #endregion

    #region Expression Evaluator
    void IDkmLanguageExpressionEvaluator.EvaluateExpression(
        DkmInspectionContext                                   _InspectionContext, 
        DkmWorkList                                            _WorkList, 
        DkmLanguageExpression                                  _Expression, 
        DkmStackWalkFrame                                      _StackFrame, 
        DkmCompletionRoutine<DkmEvaluateExpressionAsyncResult> _CompletionRoutine
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_StackFrame.Process);

      try
      {
        Evaluator.EvaluateExpression(_InspectionContext, _WorkList, _Expression, _StackFrame, _CompletionRoutine);
      }
      catch (DkmException)
      {
        // TODO: Add log
      }
    }

    void IDkmLanguageExpressionEvaluator.GetChildren(
        DkmEvaluationResult                             _Result, 
        DkmWorkList                                     _WorkList, 
        int                                             _InitialRequestChild, 
        DkmInspectionContext                            _InspectionContext, 
        DkmCompletionRoutine<DkmGetChildrenAsyncResult> _CompletionRoutine
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_InspectionContext.RuntimeInstance.Process);

      try
      {
        Evaluator.GetChildren(_Result, _WorkList, _InitialRequestChild, _InspectionContext, _CompletionRoutine);
      }
      catch (DkmException)
      {
        // TODO: Add log
      }
    }

    void IDkmLanguageExpressionEvaluator.GetFrameLocals(
        DkmInspectionContext                               _InspectionContext, 
        DkmWorkList                                        _WorkList, 
        DkmStackWalkFrame                                  _StackFrame, 
        DkmCompletionRoutine<DkmGetFrameLocalsAsyncResult> _CompletionRoutine
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_StackFrame.Process);

      try
      {
        Evaluator.GetFrameLocals(_InspectionContext, _WorkList, _StackFrame, _CompletionRoutine);
      }
      catch (DkmException)
      {
        // TODO: Add log
      }
    }

    void IDkmLanguageExpressionEvaluator.GetFrameArguments(
        DkmInspectionContext                                  _InspectionContext, 
        DkmWorkList                                           _WorkList, 
        DkmStackWalkFrame                                     _StackFrame, 
        DkmCompletionRoutine<DkmGetFrameArgumentsAsyncResult> _CompletionRoutine
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_StackFrame.Process);

      try
      {
        Evaluator.GetFrameArguments(_InspectionContext, _WorkList, _StackFrame, _CompletionRoutine);
      }
      catch (DkmException)
      {
        // TODO: Add log
      }
    }

    void IDkmLanguageExpressionEvaluator.GetItems(
        DkmEvaluationResultEnumContext                     _EnumContext, 
        DkmWorkList                                        _WorkList, 
        int                                                _StartIndex, 
        int                                                _Count, 
        DkmCompletionRoutine<DkmEvaluationEnumAsyncResult> _CompletionRoutine
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_EnumContext.RuntimeInstance.Process);

      try
      {
        Evaluator.GetItems(_EnumContext, _WorkList, _StartIndex, _Count, _CompletionRoutine);
      }
      catch (DkmException)
      {
        // TODO: Add log
      }
    }

    void IDkmLanguageExpressionEvaluator.SetValueAsString(
        DkmEvaluationResult _Result,
        string              _Value, 
        int                 _Timeout, 
        out string          _ErrorText
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_Result.RuntimeInstance.Process);

      try
      {
        Evaluator.SetValueAsString(_Result, _Value, _Timeout, out _ErrorText);
      }
      catch (DkmException)
      {
        // TODO: Add log

        _Result.SetValueAsString(_Value, _Timeout, out _ErrorText);
      }
    }

    string IDkmLanguageExpressionEvaluator.GetUnderlyingString(
        DkmEvaluationResult _Result
      )
    {
      var Evaluator = Utility.GetOrCreateDataItem<ExpressionEvaluator>(_Result.RuntimeInstance.Process);

      try
      {
        return Evaluator.GetUnderlyingString(_Result);
      }
      catch (DkmException)
      {
        // TODO: Add log

        return _Result.GetUnderlyingString();
      }
    }

    #endregion
  }
}
