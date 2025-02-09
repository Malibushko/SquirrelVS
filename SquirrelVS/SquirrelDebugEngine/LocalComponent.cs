﻿using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;
using Microsoft.VisualStudio.Debugger.Stepping;
using Microsoft.VisualStudio.Debugger.Symbols;
using System.Text;
using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SquirrelDebugEngine.Proxy;
using System.Runtime.InteropServices;
using System.Collections.Generic;

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
      IDkmCustomVisualizer,
      IDkmLanguageExpressionEvaluator
  {
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct HelperOffsetsDataHolder
    {
      public UInt64 StackTopOffset;
      public UInt64 StackAddress;
      public UInt64 SquirrelObjectPtrSize;
      public UInt64 SquirrelObjectValueOffset;
      public UInt64 SquirrelStrignValueOffset;
    }

    private class HelperHookDataHolder : DkmDataItem
    {
      public DkmThread SuspendThread;

      public PointerProxy SquirrelNewClosure;
      public PointerProxy SquirrelSetDebugHook;
      public PointerProxy SquirrelHandle;

      public Guid SquirrelNativeClosureCreated;

      public ulong HelperOffsetsAddress;
    }

    private class SquirrelBreakpointsDataHolder : DkmDataItem
    {
      public Guid                            SquirrelOpenBreakpoint;
      public Guid                            SquirrelCloseBreakpoint;
      public Guid                            SquirrelLoadFileBreakpoint;
      public Guid                            SquirrelHelperInitialized;
      public DkmRuntimeInstructionBreakpoint SquirrelCallNativeBreakpoint; // used only while stepping
      public DkmRuntimeInstructionBreakpoint SquirrelStepInFallthroughBreakpoint;
    }

    internal class HelperLocationsDataHolder : DkmDataItem
    {
      public AddressRange ModuleAddresses;
      public ulong        WorkingDirectoryAddress;
      public UInt64Proxy  IsSquirrelUnicode;
      public Int64Proxy   LastLine;
      public Int64Proxy   LastType;
    }

    internal class SquirrelLocations : DkmDataItem
    {
      public AddressRange SquirrelOpen;         // sq_open 
      public AddressRange SquirrelClose;        // sq_close
      public AddressRange SquirrelLoadFile;     // sqstd_loadfile
      public AddressRange SquirrelCall;         // sq_call
      public ulong        SquirrelNewClosure;   // sq_newclosure
      public ulong        SquirrelSetDebugHook; // sq_setdebughook
      public AddressRange SquirrelCallNative;   // SQVM::CallNative
    }

    public LocalComponent() : base(Guids.SquirrelLocalComponentGuid)
    {
      // Empty
    }

    #region Interface
    public void OnModuleInstanceLoad(
        DkmModuleInstance _ModuleInstance,
        DkmWorkList _WorkList,
        DkmEventDescriptorS _EventDescriptor)
    {
      DkmNativeModuleInstance NativeModuleInstance = _ModuleInstance as DkmNativeModuleInstance;

      if (NativeModuleInstance == null)
        return;

      string ModuleName = NativeModuleInstance.FullName;
      DkmProcess Process = NativeModuleInstance.Process;
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
        LocalProcessData _ProcessData
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
          _Module.Process.SetDataItem(DkmDataCreationDisposition.CreateNew, Reply.Parameter1 as SquirrelLocations);

          _ProcessData.SquirrelModule = _Module;
        }
      }

      return false;
    }

    private static bool TrySetupDebugHooks(
          DkmProcess _Process
        )
    {
      var               LocalData       = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      var               HookData        = Utility.GetOrCreateDataItem<HelperHookDataHolder>(_Process);
      var               HelperLocations = Utility.GetOrCreateDataItem<HelperLocationsDataHolder>(_Process);
      SquirrelLocations Locations       = Utility.GetOrCreateDataItem<SquirrelLocations>(_Process);

      if (LocalData.SquirrelHandle         == null || 
          LocalData.SquirrelHandle.Address == 0    ||
          LocalData.HelperState            != HelperState.Initialized)
      {
        return false;
      }

      HookData.SquirrelSetDebugHook.Write(Locations.SquirrelSetDebugHook);
      HookData.SquirrelNewClosure  .Write(Locations.SquirrelNewClosure);
      HookData.SquirrelHandle      .Write(LocalData.SquirrelHandle.Address);

      LocalData.WorkingDirectory = Utility.ReadStringVariable(_Process, HelperLocations.WorkingDirectoryAddress, 1024);

      InitHelperOffsets(_Process);

      return true;
    }

    private bool InjectDebugHelper(
        DkmProcess       _Process,
        LocalProcessData _ProcessData
      )
    {
      HelperHookDataHolder ExistingData = _Process.GetDataItem<HelperHookDataHolder>();

      if (ExistingData != null)
        return false;

      HelperHookDataHolder          Holder      = new HelperHookDataHolder();
      SquirrelBreakpointsDataHolder Breakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpointsDataHolder>(_Process);
      SquirrelLocations             Locations   = Utility.GetOrCreateDataItem<SquirrelLocations>(_Process);

      _Process.SetDataItem(DkmDataCreationDisposition.CreateNew, Holder);
      
      Breakpoints.SquirrelOpenBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
        _Process,
        _ProcessData.SquirrelModule,
        "sq_open",
        "Create new squirrel vm",
        Locations.SquirrelOpen.End).GetValueOrDefault(Guid.Empty);

      Breakpoints.SquirrelCloseBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
        _Process,
        _ProcessData.SquirrelModule,
        "sq_close",
        "Close squirrel vm",
        Locations.SquirrelClose.Start).GetValueOrDefault(Guid.Empty);

      Breakpoints.SquirrelLoadFileBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointAtAddress(
        _Process,
        _ProcessData.SquirrelModule,
        "sq_compilebuffer",
        "Loads a new script",
        Locations.SquirrelLoadFile.End).GetValueOrDefault(Guid.Empty);

      Breakpoints.SquirrelCallNativeBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointObjectAtAddress(
          _Process,
          _ProcessData.SquirrelModule,
          "SQVM:CallNative",
          "Calls a native closure",
          Locations.SquirrelCallNative.Start,
          false
        );

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

      return true;
    }

    private bool InitalizeDebugHelper(
        DkmNativeModuleInstance _Module,
        LocalProcessData        _ProcessData
      )
    {
      var IsInitialzeAddress = _Module.FindExportName("IsInitialized", false);
      var Process = _Module.Process;

      if (IsInitialzeAddress == null)
        return false;

      var HelperLocations     = Utility.GetOrCreateDataItem<HelperLocationsDataHolder>(_Module.Process);
      var SquirrelLocations   = Utility.GetOrCreateDataItem<SquirrelLocations>(_Module.Process);
      var SquirrelBreakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpointsDataHolder>(_Module.Process);

      HelperLocations.WorkingDirectoryAddress = AttachmentHelpers.FindVariableAddress(_Module, "WorkingDirectory");
      HelperLocations.ModuleAddresses         = new AddressRange(_Module.BaseAddress, _Module.BaseAddress + _Module.Size);
      HelperLocations.IsSquirrelUnicode       = new UInt64Proxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "IsSQUnicode"));
      HelperLocations.LastLine                = new Int64Proxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "LastLine"));
      HelperLocations.LastType                = new Int64Proxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "LastType"));

      new RemoteComponent.StepperLocationsNotification
      {
        StepperKindAddress = AttachmentHelpers.FindVariableAddress(_Module, "StepKind"),
        SteppingStackDepth = AttachmentHelpers.FindVariableAddress(_Module, "SteppingStackDepth")
      }.SendLower(Process);

      new RemoteComponent.HelperBreakpointsNotification
      {
        StepCompleteBPGuid  = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepComplete").GetValueOrDefault(Guid.Empty),
        BreakpointHitBPGuid = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperAsyncBreak").GetValueOrDefault(Guid.Empty),
        
      }.SendLower(Process);

      new RemoteComponent.HelperLocationsNotification
      {
        HelperAddresses   = new AddressRange(_Module.BaseAddress, _Module.BaseAddress + _Module.Size),
        SquirrelAddresses = new List<AddressRange>{ new AddressRange(SquirrelLocations.SquirrelCall), new AddressRange(SquirrelLocations.SquirrelCallNative) },

        StringBufferAddress = AttachmentHelpers.FindVariableAddress(_Module, "StringBuffer"),
        ActiveBreakpointsDataAddress = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpoints"),
        ActiveBreakpointsCountAddress = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpointsCount"),
        HitBreakpointIndexAddress = AttachmentHelpers.FindVariableAddress(_Module, "HitBreakpointIndex")
      }.SendLower(Process);
      
      var DataHolder = _Module.Process.GetDataItem<HelperHookDataHolder>();

      DataHolder.SquirrelNativeClosureCreated = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnCreatedDebugHookNativeClosure").GetValueOrDefault(Guid.Empty);

      DataHolder.SquirrelNewClosure   = new PointerProxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "sq_newclosure"));
      DataHolder.SquirrelSetDebugHook = new PointerProxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "sq_setdebughook"));
      DataHolder.SquirrelHandle       = new PointerProxy(Process, AttachmentHelpers.FindVariableAddress(_Module, "SquirrelHandle"));
      
      DataHolder.HelperOffsetsAddress = AttachmentHelpers.FindVariableAddress(_Module, "Offsets");

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

            Process.GetDataItem<SquirrelBreakpointsDataHolder>().SquirrelHelperInitialized = OnInitializedBreakpoint.Value;

            _ProcessData.HelperState = HelperState.WaitingForInitialization;

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
      var HookData          = _Process.GetDataItem<HelperHookDataHolder>();
      var LocalData         = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      var HelperLocations   = Utility.GetOrCreateDataItem<HelperLocationsDataHolder>(_Process);

      long? IsSQUnicode = EvaluationHelpers.TryEvaluateNumberExpression(
          "sizeof(SQChar) == sizeof(wchar_t)", 
          _Session,
          _Thread,
          _Frame,
          Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.TreatAsExpression | Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects
        );

      if (IsSQUnicode.HasValue && HelperLocations.IsSquirrelUnicode.Address != 0)
        HelperLocations.IsSquirrelUnicode.Write(IsSQUnicode.Value == 0 ? (byte)0 : (byte)1);

      LocalData.SquirrelHandle = new SQVM(_Process, _SquirrelHandleAddress.Value);

      if (TrySetupDebugHooks(_Process) || LocalData.HelperState != HelperState.Initialized)
      {
        HookData.SuspendThread = _Thread;

        _Thread.Suspend(true);
      }

      LocalData.Symbols.FetchOrCreate(_SquirrelHandleAddress.Value);
    }
    
    static public void InitHelperOffsets(
        DkmProcess _Process
      )
    {
      var LocalData = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      var HookData  = Utility.GetOrCreateDataItem<HelperHookDataHolder>(_Process);

      if (HookData.HelperOffsetsAddress == 0)
        return;

      var SQVectorMetadata = StructProxy.GetStructFields<SQObjectPtrVec, SQObjectPtrVec.Fields>(_Process);
      var SQObjectMetadata = StructProxy.GetStructFields<SQObject, SQObject.SQObjectFields>(_Process);
      var SQStringMetadata = StructProxy.GetStructFields<SQString, SQString.Fields>(_Process);

      var HelperOffsets = new HelperOffsetsDataHolder
      {
        StackAddress              = (ulong)LocalData.SquirrelHandle.StackOffset + (ulong)(SQVectorMetadata?._vals.Offset),
        StackTopOffset            = (ulong)LocalData.SquirrelHandle.StackTopOffset,
        SquirrelObjectPtrSize     = (ulong)StructProxy.GetStructMetadata<SQObjectPtr>(_Process).Size,
        SquirrelObjectValueOffset = (ulong)SQObjectMetadata._unVal.Offset,
        SquirrelStrignValueOffset = (ulong)SQStringMetadata._val.Offset
      };

      new CliStructProxy<HelperOffsetsDataHolder>(_Process, HookData.HelperOffsetsAddress).Write(HelperOffsets);
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
        DkmResolvedDocument     _ResolvedDocument, 
        DkmTextSpan             _TextSpan, 
        string                  _Text, 
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
      var Filter = Utility.GetOrCreateDataItem<CallStackFilter>(_StackContext.Thread.Process);

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

    #region Custom Visualizer
    void IDkmCustomVisualizer.EvaluateVisualizedExpression(
        DkmVisualizedExpression _Expression, 
        out DkmEvaluationResult _ResultObject
      )
    {
      _Expression.EvaluateVisualizedExpression(out _ResultObject);
    }

    void IDkmCustomVisualizer.UseDefaultEvaluationBehavior(
        DkmVisualizedExpression _Expression, 
        out bool                _UseDefaultEvaluationBehaviour, 
        out DkmEvaluationResult _DefaultEvaluationResult
      )
    {
      _Expression.UseDefaultEvaluationBehavior(out _UseDefaultEvaluationBehaviour, out _DefaultEvaluationResult);
    }

    void IDkmCustomVisualizer.GetChildren(
        DkmVisualizedExpression            _Expression, 
        int                                _InitialRequestSize, 
        DkmInspectionContext               _InspectionContext, 
        out DkmChildVisualizedExpression[] _InitialChildren, 
        out DkmEvaluationResultEnumContext _EnumContext
      )
    {
      _Expression.GetChildren(_InitialRequestSize, _InspectionContext, out _InitialChildren, out _EnumContext);
    }

    void IDkmCustomVisualizer.GetItems(
        DkmVisualizedExpression            _VisualizedExpression, 
        DkmEvaluationResultEnumContext     _EnumContext, 
        int                                _StartIndex, 
        int                                _Count, 
        out DkmChildVisualizedExpression[] _Items
      )
    {
      _VisualizedExpression.GetItems(_EnumContext, _StartIndex, _Count, out _Items);
    }

    void IDkmCustomVisualizer.SetValueAsString(
        DkmVisualizedExpression _Expression, 
        string                  _Value, 
        int                     _Timeout, 
        out string              _ErrorText
      )
    {
      _Expression.SetValueAsString(_Value, _Timeout, out _ErrorText);
    }

    string IDkmCustomVisualizer.GetUnderlyingString(
        DkmVisualizedExpression _Expression
      )
    {
      return _Expression.GetUnderlyingString();
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
        var SquirrelBreakpoints = Utility.GetOrCreateDataItem<SquirrelBreakpointsDataHolder>(_Process);
        var LocalData           = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
        var HelperLocations      = Utility.GetOrCreateDataItem<HelperLocationsDataHolder>(_Process);
        var HookData            = Utility.GetOrCreateDataItem<HelperHookDataHolder>(_Process);
        var Thread              = _Process.GetThreads()
                                          .FirstOrDefault(_Thread => _Thread.UniqueId == ThreadID);

        if (BreakpointID == HookData.SquirrelNativeClosureCreated)
        {
          HookData.SuspendThread?.Resume(true);
        }
        else
        if (BreakpointID == SquirrelBreakpoints.SquirrelHelperInitialized)
        {
          LocalData.HelperState = HelperState.Initialized;

          TrySetupDebugHooks(_Process);

          return;
        }
        else
        if (BreakpointID == SquirrelBreakpoints.SquirrelOpenBreakpoint)
        {
          var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, FrameBase, VFrame, out DkmStackWalkFrame _Frame);

          ulong? SquirrelHandleAddress = EvaluationHelpers.TryEvaluateAddressExpression(
              $"v",
              InspectionSession,
              Thread,
              _Frame,
              Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.TreatAsExpression | Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects
            );

          if (!SquirrelHandleAddress.HasValue)
            SquirrelHandleAddress = EvaluationHelpers.TryEvaluateAddressExpression("@rax", InspectionSession, Thread, _Frame, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.TreatAsExpression | Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects);

          if (SquirrelHandleAddress != 0)
            RegisterSquirrelState(_Process, InspectionSession, Thread, _Frame, SquirrelHandleAddress);

          InspectionSession.Close();

          return;
        }
        else
        if (BreakpointID == SquirrelBreakpoints.SquirrelLoadFileBreakpoint)
        {
          var InspectionSession = EvaluationHelpers.CreateInspectionSession(_Process, Thread, FrameBase, VFrame, out DkmStackWalkFrame Frame);

          var SourceNameAddress = EvaluationHelpers.TryEvaluateAddressExpression("filename", InspectionSession, Thread, Frame, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.TreatAsExpression | Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects);

          if (!SourceNameAddress.HasValue)
            throw new Exception("Unable to locate source name");

          string SourceName = Utility.ReadStringVariable(_Process, SourceNameAddress.Value, 256);

          string SourcePath = Path.GetFullPath(Path.Combine(LocalData.WorkingDirectory, SourceName));

          string ScriptContent = File.ReadAllText(SourcePath);

          var Message = DkmCustomMessage.Create(
              _Process.Connection,
              _Process,
              Guid.Empty,
              1,
              Encoding.UTF8.GetBytes(SourcePath),
              null
            );

          SymbolsVM Symbols = LocalData.Symbols.FetchOrCreate(LocalData.SquirrelHandle.Address);

          Symbols.AddScriptSource(SourceName, ScriptContent, null);
          Symbols.FetchScriptSource(SourceName).ResolvedFilename = SourcePath;

          Message.SendToVsService(Guids.SquirelDebuggerComponentID, true);
        }
        else
        if (BreakpointID == SquirrelBreakpoints.SquirrelCallNativeBreakpoint?.UniqueId)
        {
          var Session = EvaluationHelpers.CreateInspectionSession(_Process, Thread, FrameBase, VFrame, out DkmStackWalkFrame _Frame);

          var ClosureAddress = EvaluationHelpers.TryEvaluateAddressExpression("nclosure", Session, Thread, _Frame, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.NoSideEffects);

          if (ClosureAddress.HasValue)
          {
            var NativeClosure = new SQNativeClosure(_Process, ClosureAddress.Value);

            if (!NativeClosure.Function.IsNull)
            {
              ulong NativeAddress = NativeClosure.Function.Read();

              if (!HelperLocations.ModuleAddresses.In(NativeAddress))
              {
                SquirrelBreakpoints.SquirrelStepInFallthroughBreakpoint = AttachmentHelpers.CreateTargetFunctionBreakpointObjectAtAddress(
                    _Process,
                    LocalData.SquirrelModule,
                    "Step fallthrough",
                    string.Empty,
                    NativeAddress,
                    true
                  );
              
                if (SquirrelBreakpoints.SquirrelStepInFallthroughBreakpoint != null)
                {
                  new RemoteComponent.OnStepFallthroughNotification()
                  {
                    BreakpointGuid = SquirrelBreakpoints.SquirrelStepInFallthroughBreakpoint.UniqueId
                  }.SendLower(_Process);

                  SquirrelBreakpoints.SquirrelCallNativeBreakpoint.Disable();
                }
              }
            }
          }
        }
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class HandleCustomMessage : MessageBase<HandleCustomMessage>
    {
      [DataMember]
      public string Message;

      public override void Handle(
          DkmProcess _Process
        )
      {
        Debug.WriteLine(Message);
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class OnStepNotification : MessageBase<OnStepNotification>
    {
      [DataMember]
      public DkmStepKind StepKind;

      public override void Handle(
          DkmProcess _Process
        )
      {
        var SquirrelBreakpoints = _Process.GetDataItem<SquirrelBreakpointsDataHolder>();

        if (StepKind == DkmStepKind.Into)
          SquirrelBreakpoints.SquirrelCallNativeBreakpoint.Enable();
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class OnStepFinishedNotification : MessageBase<OnStepFinishedNotification>
    {
      public override void Handle(
          DkmProcess _Process
        )
      {
        _Process.GetDataItem<SquirrelBreakpointsDataHolder>().SquirrelCallNativeBreakpoint.Disable();
      }
    }

    [DataContract]
    [MessageTo(Guids.SquirrelLocalComponentID)]
    internal class OnStepFallthroughFinishedNotification : MessageBase<OnStepFallthroughFinishedNotification>
    {
      public override void Handle(
          DkmProcess _Process
        )
      {
        var SquirrelBreakpoints = _Process.GetDataItem<SquirrelBreakpointsDataHolder>();

        SquirrelBreakpoints.SquirrelStepInFallthroughBreakpoint?.Close();
      }
    }
    #endregion
  }
}
