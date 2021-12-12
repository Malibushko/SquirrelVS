﻿using Microsoft.VisualStudio.Debugger;
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
  public class LocalComponent : IDkmCallStackFilter, IDkmModuleInstanceLoadNotification, IDkmCustomMessageCallbackReceiver, 
                                IDkmSymbolQuery, IDkmSymbolCompilerIdQuery, IDkmSymbolDocumentCollectionQuery, IDkmSymbolDocumentSpanQuery, IDkmModuleUserCodeDeterminer, IDkmSymbolHiddenAttributeQuery,
                                IDkmLanguageExpressionEvaluator, IDkmLanguageInstructionDecoder
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
          System.Diagnostics.Debug.WriteLine("Trying to create runtime");

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

      HelperBreaks.SquirrelHelperBreakpointHit = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperBreakpointHit").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperStepComplete  = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepComplete").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperStepInto      = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepInto").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperStepOut       = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperStepOut").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperAsyncBreak    = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelHelperAsyncBreak").GetValueOrDefault(Guid.Empty);

      HelperBreaks.SquirrelHelperFunctionCall   = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelFunctionCall").GetValueOrDefault(Guid.Empty);
      HelperBreaks.SquirrelHelperFunctionReturn = AttachmentHelpers.CreateHelperFunctionBreakpoint(_Module, "OnSquirrelFunctionReturn").GetValueOrDefault(Guid.Empty);

      HelperBreaks.WorkingDirectoryAddress               = AttachmentHelpers.FindVariableAddress(_Module, "WorkingDirectory");
      HelperBreaks.SquirrelHitBreakpointIndexAddress     = AttachmentHelpers.FindVariableAddress(_Module, "HitBreakpointIndex");
      HelperBreaks.SquirrelActiveBreakpointsCountAddress = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpointCount");
      HelperBreaks.SquirrelActiveBreakpointsAddress      = AttachmentHelpers.FindVariableAddress(_Module, "ActiveBreakpoints");
      HelperBreaks.SquirrelBreakpointsBufferAddress      = AttachmentHelpers.FindVariableAddress(_Module, "BreakpointsStringBuffer");

      HelperBreaks.SquirrelStackInfoAddress      = AttachmentHelpers.FindVariableAddress(_Module, "StackInfo");

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

      _ProcessData.DebugHookAddress = AttachmentHelpers.FindFunctionAddress(_Module, "SquirrelDebugHook_3_1");

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

      if (!_debughook_native.HasValue || !_debughook.HasValue)
        return;

      LocalData.HookData = new HookData
      {
        DebugHookNativeAddress = _debughook_native.Value,
        DebugHookNative        = _debughook.Value,
        HelperHookAddress      = LocalData.DebugHookAddress
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
        SquirrelBreakpointData CallData = new SquirrelBreakpointData();
        
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

      /* 
        Compare by SHA1 hash also 
       */

      return _Module.FindDocuments(_SourceField);
    }

    DkmInstructionSymbol[] IDkmSymbolDocumentSpanQuery.FindSymbols(
        DkmResolvedDocument _ResolvedDocument, 
        DkmTextSpan         _TextSpan, 
        string              _Text, 
        out DkmSourcePosition[] _SymbolLocation
      )
    {
      var SourceFileID = DkmSourceFileId.Create(_ResolvedDocument.DocumentName, null, null, null);

      var ResultSpan = new DkmTextSpan(_TextSpan.StartLine, _TextSpan.StartLine, 0, 0);

      _SymbolLocation = new DkmSourcePosition[1] { DkmSourcePosition.Create(SourceFileID, ResultSpan) };

      var BreakpointData = new BreakpointData
      {
        SourceName = _ResolvedDocument.GetDataItem<ResolvedDocumentItem>().ScriptData.SourceName,
        Line       = (ulong)_TextSpan.StartLine
      };

      return new DkmInstructionSymbol[1] { DkmCustomInstructionSymbol.Create(_ResolvedDocument.Module, Guids.SquirrelRuntimeID, BreakpointData.Encode(), (ulong)((_TextSpan.StartLine << 16) + 0), null) };
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
        ProcessData.HelperState = HelperState.Initialized;

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
            $"L",
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

      if (_BreakpointData.BreakpointID == KnownBreakpoints.SquirrelHelperFunctionCall || 
          _BreakpointData.BreakpointID == KnownBreakpoints.SquirrelHelperFunctionReturn)
      {
        if (KnownBreakpoints.SquirrelStackInfoAddress == 0)
          return false;

        var Address = KnownBreakpoints.SquirrelStackInfoAddress;

        SquirrelBreakpointData BreakpointData = new SquirrelBreakpointData(_Process, Address);

        if (_BreakpointData.BreakpointID == KnownBreakpoints.SquirrelHelperFunctionCall)
          Utility.GetOrCreateDataItem<SquirrelCallStack>(_Process).Callstack.Push(BreakpointData);
        else
          Utility.GetOrCreateDataItem<SquirrelCallStack>(_Process).Callstack.Pop();
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
        DkmStackWalkFrame _Input
      )
    {
      if (_Input == null)
        return null; // End of stack

      if (_Input.InstructionAddress == null)
        return new DkmStackWalkFrame[1] { _Input };

      if (_Input.InstructionAddress.ModuleInstance == null)
        return new DkmStackWalkFrame[1] { _Input };

      var StackContextData = Utility.GetOrCreateDataItem<SquirrelStackContextData>(_StackContext);

      if (_Input.ModuleInstance != null && _Input.ModuleInstance.Name == "SquirrelDebugHelper.dll")
      {
        StackContextData.HideTopLibraryFrames = true;

        return new DkmStackWalkFrame[1] { DkmStackWalkFrame.Create(
            _StackContext.Thread,
            _Input.InstructionAddress, 
            _Input.FrameBase,
            _Input.FrameSize,
            DkmStackWalkFrameFlags.NonuserCode | DkmStackWalkFrameFlags.Hidden,
            "[Squirrel Debugger Helper]",
            _Input.Registers, 
            _Input.Annotations
          ) };
      }

      DkmProcess       Process     = _StackContext.InspectionSession.Process;
      LocalProcessData ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(Process);

      string MethodName = GetFrameMethodName(_Input);

      if (MethodName == null)
        return new DkmStackWalkFrame[1] { _Input };

      if (MethodName == "sq_call")
      {
        if (ProcessData.SquirrelHandleAddress != StackContextData.HandleAddress)
        {
          StackContextData.HandleAddress      = ProcessData.SquirrelHandleAddress;
          StackContextData.SeenSquirrelFrames = false;
          StackContextData.SkipFramesCount    = 0;
          StackContextData.SeenFramesCount    = 0;
        }

        if (ProcessData.RuntimeInstance == null)
        {
          ProcessData.RuntimeInstance = Process.GetRuntimeInstances().OfType<DkmCustomRuntimeInstance>().FirstOrDefault(el => el.Id.RuntimeType == Guids.SquirrelRuntimeID);

          if (ProcessData.RuntimeInstance == null)
            return new DkmStackWalkFrame[1] { _Input };

          ProcessData.ModuleInstance = ProcessData.RuntimeInstance.GetModuleInstances().OfType<DkmCustomModuleInstance>().FirstOrDefault(el => el.Module != null && el.Module.CompilerId.VendorId == Guids.SquirrelCompilerID);

          if (ProcessData.ModuleInstance == null)
            return new DkmStackWalkFrame[1] { _Input };
        }

        var SquirrelFrameFlags = _Input.Flags;

        SquirrelFrameFlags &= ~(DkmStackWalkFrameFlags.NonuserCode | DkmStackWalkFrameFlags.UserStatusNotDetermined);

        if ((_Input.Flags | DkmStackWalkFrameFlags.TopFrame) != 0)
          SquirrelFrameFlags |= DkmStackWalkFrameFlags.TopFrame;

        var Callstack      = Utility.GetOrCreateDataItem<SquirrelCallStack>(Process).Callstack;
        var SquirrelFrames = new List<DkmStackWalkFrame>();

        foreach (var Call in Callstack)
        {
          if (StackContextData.SkipFramesCount != 0)
          {
            StackContextData.SkipFramesCount--;
            continue;
          }

          DkmInstructionAddress InstructionAddress = DkmCustomInstructionAddress.Create(
              ProcessData.RuntimeInstance, 
              ProcessData.ModuleInstance, 
              Call.Encode(), 
              0, 
              null, 
              null
            );
          
          SquirrelFrames.Add(DkmStackWalkFrame.Create(
            _StackContext.Thread,
            InstructionAddress,
            _Input.FrameBase,
            _Input.FrameSize,
            SquirrelFrameFlags,
            $"[{Call.SourceName} {Call.FunctionName}:{Call.Line}]",
            _Input.Registers,
            _Input.Annotations
          ));

          StackContextData.SeenFramesCount++;
        }

        StackContextData.SkipFramesCount = StackContextData.SeenFramesCount;
        return SquirrelFrames.ToArray();
      }
      else 
      if (MethodName.StartsWith("SQVM") || MethodName.StartsWith("sq_") || MethodName.StartsWith("sqstd_"))
      {
        var Flags = (_Input.Flags & ~DkmStackWalkFrameFlags.UserStatusNotDetermined) | DkmStackWalkFrameFlags.NonuserCode;

        Flags |= DkmStackWalkFrameFlags.Hidden;

        return new DkmStackWalkFrame[1] { 
          DkmStackWalkFrame.Create(
              _StackContext.Thread, 
              _Input.InstructionAddress,
              _Input.FrameBase,
              _Input.FrameSize, 
              Flags,
              _Input.Description,
              _Input.Registers,
              _Input.Annotations
            ) };
      }

      return new DkmStackWalkFrame[1] { _Input };
    }
    string GetFrameMethodName(
        DkmStackWalkFrame _Input
      )
    {
      string MethodName = null;

      if (_Input.BasicSymbolInfo != null)
          MethodName = _Input.BasicSymbolInfo.MethodName;

      return MethodName;
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
      _InspectionContext.EvaluateExpression(_WorkList, _Expression, _StackFrame, _CompletionRoutine);
    }

    void IDkmLanguageExpressionEvaluator.GetChildren(
        DkmEvaluationResult                             _Result, 
        DkmWorkList                                     _WorkList, 
        int                                             _InitialRequestChild, 
        DkmInspectionContext                            _InspectionContext, 
        DkmCompletionRoutine<DkmGetChildrenAsyncResult> _CompletionRoutine
      )
    {
      _Result.GetChildren(_WorkList, _InitialRequestChild, _InspectionContext, _CompletionRoutine);
    }

    void IDkmLanguageExpressionEvaluator.GetFrameLocals(
        DkmInspectionContext                               _InspectionContext, 
        DkmWorkList                                        _WorkList, 
        DkmStackWalkFrame                                  _StackFrame, 
        DkmCompletionRoutine<DkmGetFrameLocalsAsyncResult> _CompletionRoutine
      )
    {
      _InspectionContext.GetFrameLocals(_WorkList, _StackFrame, _CompletionRoutine);
    }

    void IDkmLanguageExpressionEvaluator.GetFrameArguments(
        DkmInspectionContext                                  _InspectionContext, 
        DkmWorkList                                           _WorkList, 
        DkmStackWalkFrame                                     _Frame, 
        DkmCompletionRoutine<DkmGetFrameArgumentsAsyncResult> _CompletionRoutine
      )
    {
      _InspectionContext.GetFrameArguments(_WorkList, _Frame, _CompletionRoutine);
    }

    void IDkmLanguageExpressionEvaluator.GetItems(
        DkmEvaluationResultEnumContext                     _EnumContext, 
        DkmWorkList                                        _WorkList, 
        int                                                _StartIndex, 
        int                                                _Count, 
        DkmCompletionRoutine<DkmEvaluationEnumAsyncResult> _CompletionRoutine
      )
    {
      _EnumContext.GetItems(_WorkList, _StartIndex, _Count, _CompletionRoutine);
    }

    void IDkmLanguageExpressionEvaluator.SetValueAsString(
        DkmEvaluationResult _Result,
        string              _Value, 
        int                 _Timeout, 
        out string          _ErrorText
      )
    {
      _Result.SetValueAsString(_Value, _Timeout, out _ErrorText);
    }

    string IDkmLanguageExpressionEvaluator.GetUnderlyingString(
        DkmEvaluationResult _Result
      )
    {
      return _Result.GetUnderlyingString();
    }

    string IDkmLanguageInstructionDecoder.GetMethodName(
        DkmLanguageInstructionAddress _LanguageInstructionAddress, 
        DkmVariableInfoFlags          _ArgumentFlags
      )
    {
      if (_LanguageInstructionAddress.Language.Name != "C++")
        return "Test";
      return _LanguageInstructionAddress.GetMethodName(_ArgumentFlags);
    }

    #endregion
  }
}
