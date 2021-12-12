using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;
using Microsoft.VisualStudio.Debugger.Symbols;
using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public enum HelperState
  {
    NotInitialized,
    WaitingForInitialization,
    Initialized
  };

  public class LocalProcessData : DkmDataItem
  {
    public DkmCustomRuntimeInstance RuntimeInstance;
    public DkmCustomModuleInstance  ModuleInstance;

    public DkmNativeModuleInstance  SquirrelModule;
    public SquirrelLocations        SquirrelLocations;

    public ulong                    LoadLibraryWAddress;
    public ulong                    HelperStartAddress;
    public ulong                    HelperEndAddress;
    
    public ulong                    DebugHookAddress;
    public HelperState              HelperState = HelperState.NotInitialized;
    public HookData                 HookData;

    public DkmThread                SquirrelThread;
    public ulong                    SquirrelHandleAddress;

    public SymbolStore              Symbols = new SymbolStore();

    public string                   WorkingDirectory;
  }

  public class SquirrelBreakpoints : DkmDataItem
  {
    public Guid SquirrelOpenBreakpoint;
    public Guid SquirrelCloseBreakpoint;
    public Guid SquirrelLoadFileBreakpoint;

    public Guid SquirrelHelperBreakpointHit;
    public Guid SquirrelHelperStepComplete;
    public Guid SquirrelHelperStepInto;
    public Guid SquirrelHelperStepOut;
    public Guid SquirrelHelperAsyncBreak;
    public Guid SquirrelHelperInitialized;

    public Guid SquirrelHelperFunctionCall;
    public Guid SquirrelHelperFunctionReturn;
    
    public ulong WorkingDirectoryAddress;

    public ulong SquirrelHitBreakpointIndexAddress;
    public ulong SquirrelActiveBreakpointsCountAddress;
    public ulong SquirrelActiveBreakpointsAddress;
    public ulong SquirrelBreakpointsBufferAddress;

    public ulong SquirrelStackInfoAddress;

    public byte[] Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(SquirrelOpenBreakpoint.ToByteArray());
          Writer.Write(SquirrelCloseBreakpoint.ToByteArray());
          Writer.Write(SquirrelLoadFileBreakpoint.ToByteArray());
          Writer.Write(SquirrelHelperBreakpointHit.ToByteArray());
          Writer.Write(SquirrelHelperStepComplete.ToByteArray());
          Writer.Write(SquirrelHelperStepInto.ToByteArray());
          Writer.Write(SquirrelHelperStepOut.ToByteArray());
          Writer.Write(SquirrelHelperAsyncBreak.ToByteArray());
          Writer.Write(SquirrelHelperInitialized.ToByteArray());

          Writer.Write(SquirrelHelperFunctionCall.ToByteArray());
          Writer.Write(SquirrelHelperFunctionReturn.ToByteArray());

          Writer.Write(WorkingDirectoryAddress);
          Writer.Write(SquirrelHitBreakpointIndexAddress);
          Writer.Write(SquirrelActiveBreakpointsCountAddress);
          Writer.Write(SquirrelActiveBreakpointsAddress);
          Writer.Write(SquirrelBreakpointsBufferAddress);

          Writer.Write(SquirrelStackInfoAddress);

          Writer.Flush();

          return Stream.ToArray();
        }
      }
    }

    public bool ReadFrom(byte[] data)
    {
      using (var stream = new MemoryStream(data))
      {
        using (var Reader = new BinaryReader(stream))
        {
          SquirrelOpenBreakpoint      = new Guid(Reader.ReadBytes(16));
          SquirrelCloseBreakpoint     = new Guid(Reader.ReadBytes(16));
          SquirrelLoadFileBreakpoint  = new Guid(Reader.ReadBytes(16));
          SquirrelHelperBreakpointHit = new Guid(Reader.ReadBytes(16));
          SquirrelHelperStepComplete  = new Guid(Reader.ReadBytes(16));
          SquirrelHelperStepInto      = new Guid(Reader.ReadBytes(16));
          SquirrelHelperStepOut       = new Guid(Reader.ReadBytes(16));
          SquirrelHelperAsyncBreak    = new Guid(Reader.ReadBytes(16));
          SquirrelHelperInitialized   = new Guid(Reader.ReadBytes(16));

          SquirrelHelperFunctionCall   = new Guid(Reader.ReadBytes(16));
          SquirrelHelperFunctionReturn = new Guid(Reader.ReadBytes(16));

          WorkingDirectoryAddress               = Reader.ReadUInt64();
          SquirrelHitBreakpointIndexAddress     = Reader.ReadUInt64();
          SquirrelActiveBreakpointsCountAddress = Reader.ReadUInt64();
          SquirrelActiveBreakpointsAddress = Reader.ReadUInt64();
          SquirrelBreakpointsBufferAddress = Reader.ReadUInt64();

          SquirrelStackInfoAddress      = Reader.ReadUInt64();
        }
      }

      return true;
    }
  }
}
