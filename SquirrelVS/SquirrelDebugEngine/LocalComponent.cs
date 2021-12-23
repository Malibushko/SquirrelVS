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
using System.Runtime.Serialization;

namespace SquirrelDebugEngine
{
  public class LocalComponent : 
      IComponentBase,
      IDkmCallStackFilter, 
      IDkmModuleInstanceLoadNotification, 
      IDkmSymbolQuery, 
      IDkmSymbolCompilerIdQuery, 
      IDkmSymbolDocumentCollectionQuery, 
      IDkmSymbolDocumentSpanQuery, 
      IDkmModuleUserCodeDeterminer, 
      IDkmSymbolHiddenAttributeQuery,
      IDkmLanguageExpressionEvaluator
  {
    private class HelperHookDataHolder : DkmDataItem
    {
      public DkmThread  SuspendThread;
      public ulong      SquirrelDebugHookAddress;
      public ulong      SquirrelDebugHookFlagAddress;
      public ulong      DebugHookRoutineAddress;
    }

    public LocalComponent() : base(Guids.SquirrelLocalComponentGuid)
    {
      // Empty
    }

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

          new RemoteComponent.CreateSquirrelRuntimeRequest().SendLower(Process);

          if (Process.LivePart == null)
            return;

          TryInitSquirrelModule(NativeModuleInstance, ProcessData);
        }

        if (ProcessData.SquirrelModule != null && ProcessData.LoadLibraryWAddress != 0)
          InjectDebugHelper(Process, ProcessData);
      }
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
      HelperHookDataHolder ExistingData = _Process.GetDataItem<HelperHookDataHolder>();

      if (ExistingData != null)
        return false;

      HelperHookDataHolder Holder = new HelperHookDataHolder();
      
      _Process.SetDataItem(DkmDataCreationDisposition.CreateNew, Holder);

      if (_ProcessData.SquirrelLocations != null)
      {
        SquirrelBreakpoints BreakpointsInfo = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);

        BreakpointsInfo.SquirrelOpenBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_open",
          "Create new squirrel vm",
          _ProcessData.SquirrelLocations.SquirrelOpen.End).GetValueOrDefault(Guid.Empty);

        BreakpointsInfo.SquirrelCloseBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_close",
          "Close squirrel vm",
          _ProcessData.SquirrelLocations.SquirrelClose.Start).GetValueOrDefault(Guid.Empty);

        BreakpointsInfo.SquirrelLoadFileBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "sq_compilebuffer",
          "Loads a new script",
          _ProcessData.SquirrelLocations.SquirrelLoadFile.End).GetValueOrDefault(Guid.Empty);

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
          // TODO: Add log
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

      HelperBreaks.WorkingDirectoryAddress = AttachmentHelpers.FindVariableAddress(_Module, "WorkingDirectory");

      _ProcessData.SquirrelLocations.IsSquirrelUnicode    = new UInt64Proxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "IsSQUnicode"));
      _ProcessData.SquirrelLocations.CallstackSize = new UInt64Proxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "CallstackSize"));

      _ProcessData.SquirrelLocations.CallstackAddress = AttachmentHelpers.FindVariableAddress(_Module, "Callstack");

      new RemoteComponent.StepperLocationsNotification
      {
        StepperKindAddress = AttachmentHelpers.FindVariableAddress(_Module, "StepKind")
      }.SendLower(Process);

      new RemoteComponent.HelperBreakpointsNotification
      {
        StepCompleteBPGuid  = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepComplete").GetValueOrDefault(Guid.Empty),
        BreakpointHitBPGuid = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperAsyncBreak").GetValueOrDefault(Guid.Empty)
      }.SendLower(Process);

      new RemoteComponent.HelperLocationsNotification
      {
        HelperAddresses               = new AddressRange(_Module.BaseAddress, _Module.BaseAddress + _Module.Size),
        SquirrelAddresses             = new AddressRange(_ProcessData.SquirrelLocations.SquirrelCall),

        StringBufferAddress           = AttachmentHelpers.FindVariableAddress(_Module, "StringBuffer"),
        ActiveBreakpointsDataAddress  = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpoints"),
        ActiveBreakpointsCountAddress = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpointsCount"),
        HitBreakpointIndexAddress     = AttachmentHelpers.FindVariableAddress(_Module, "HitBreakpointIndex")
    }.SendLower(Process);

      var DataHolder = _Module.Process.GetDataItem<HelperHookDataHolder>();

      DataHolder.DebugHookRoutineAddress = AttachmentHelpers.FindFunctionAddress(_Module, "TraceRoutine");

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

    static void RegisterSquirrelState(
        DkmProcess           _Process,
        DkmInspectionSession _Session, 
        DkmThread            _Thread, 
        DkmStackWalkFrame    _Frame, 
        ulong?               _SquirrelHandleAddress
      )
    {
      LocalProcessData     LocalData  = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      HelperHookDataHolder DataHolder = _Process.GetDataItem<HelperHookDataHolder>();

      DataHolder.SquirrelDebugHookAddress = EvaluationHelpers.TryEvaluateAddressExpression(
          $"&((SQVM*){_SquirrelHandleAddress})->_debughook_native",
          _Session,
          _Thread,
          _Frame,
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        ).GetValueOrDefault(0);

      DataHolder.SquirrelDebugHookFlagAddress = EvaluationHelpers.TryEvaluateAddressExpression(
          $"&((SQVM*){_SquirrelHandleAddress})->_debughook",
          _Session, 
          _Thread, 
          _Frame, 
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        ).GetValueOrDefault(0);

      long? IsSQUnicode = EvaluationHelpers.TryEvaluateNumberExpression(
          "sizeof(SQChar) == sizeof(wchar_t)", 
          _Session,
          _Thread,
          _Frame,
          DkmEvaluationFlags.TreatAsExpression | DkmEvaluationFlags.NoSideEffects
        );

      if (IsSQUnicode.HasValue && LocalData.SquirrelLocations.IsSquirrelUnicode.Address != 0)
        LocalData.SquirrelLocations.IsSquirrelUnicode.Write(IsSQUnicode.Value == 0 ? (byte)0 : (byte)1);

      if (LocalData.HelperState != HelperState.Initialized)
      {
        LocalData.SquirrelHandleAddress = _SquirrelHandleAddress.Value;

        DataHolder.SuspendThread = _Thread;

        _Thread.Suspend(true);

        return;
      }

      LocalData.Symbols.FetchOrCreate(_SquirrelHandleAddress.Value);

      new RemoteComponent.RegisterStateRequest()
      {
        DebugHookRoutine             = DataHolder.DebugHookRoutineAddress,
        SquirrelDebugHookAddress     = DataHolder.SquirrelDebugHookAddress,
        SquirrelDebugHookFlagAddress = DataHolder.SquirrelDebugHookFlagAddress
      }.SendLower(_Process);
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

    #region Requests

    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class HandleBreakpointRequest : MessageBase<HandleBreakpointRequest>
    {
      [DataMember]
      public Guid BreakpointID { get; set; }

      [DataMember]
      public Guid ThreadID { get; set; }

      [DataMember]
      public ulong FrameBase { get; set; }

      [DataMember]
      public ulong VFrame { get; set; }

      [DataMember]
      public ulong ReturnAddress { get; set; }

      public override void Handle(
          DkmProcess _Process
        )
      {
        SquirrelBreakpoints RuntimeDllBreakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpoints>(_Process);
        LocalProcessData    ProcessData           = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);

        DkmThread Thread = _Process.GetThreads().FirstOrDefault(_Thread => _Thread.UniqueId == ThreadID);

        if (BreakpointID == RuntimeDllBreakpoints.SquirrelHelperInitialized)
        {
          HelperHookDataHolder DataHolder = _Process.GetDataItem<HelperHookDataHolder>();

          new RemoteComponent.RegisterStateRequest()
          { 
            DebugHookRoutine             = DataHolder.DebugHookRoutineAddress,
            SquirrelDebugHookAddress     = DataHolder.SquirrelDebugHookAddress,
            SquirrelDebugHookFlagAddress = DataHolder.SquirrelDebugHookFlagAddress
          }.SendLower(_Process);

          ProcessData.WorkingDirectory = Utility.ReadStringVariable(_Process, RuntimeDllBreakpoints.WorkingDirectoryAddress, 1024);

          DataHolder.SuspendThread.Resume(true);
        }

        if (BreakpointID == RuntimeDllBreakpoints.SquirrelOpenBreakpoint)
        {
          var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, FrameBase, VFrame, out DkmStackWalkFrame _Frame);

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

          return;
        }

        if (BreakpointID == RuntimeDllBreakpoints.SquirrelLoadFileBreakpoint)
        {
          var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, FrameBase, VFrame, out DkmStackWalkFrame Frame);

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
      }
    }


    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class OnBeforeBreakpointHitNotification : MessageBase<OnBeforeBreakpointHitNotification>
    {
      public override void Handle(
          DkmProcess _Process
        )
      {
        LocalProcessData  Data      = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
        SquirrelCallStack Callstack = Utility.GetOrCreateDataItem<SquirrelCallStack>(_Process);

        Callstack.Callstack.Clear();

        ulong? FrameCount = Data.SquirrelLocations.CallstackSize.Read();

        if (FrameCount.HasValue && FrameCount.Value > 0)
        {
          int Offset = 0;
          for (int i = 0; i < (int)FrameCount.Value; ++i)
          {
            Callstack.Callstack.Push(new CallstackFrame(_Process, Data.SquirrelLocations.CallstackAddress + (ulong)Offset));

            Offset += 24;
          }
        }
      }
    }
    #endregion
  }
}
