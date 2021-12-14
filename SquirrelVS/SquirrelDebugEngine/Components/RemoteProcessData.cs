using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Symbols;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.Breakpoints;
using Microsoft.VisualStudio.Debugger.Stepping;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelDebugEngine
{
  public class BreakpointData
  {
    public string               SourceName;
    public ulong                Line;
    public DkmRuntimeBreakpoint Breakpoint = null;

    public ReadOnlyCollection<byte> Encode()
    {
      using (var stream = new MemoryStream())
      {
        using (var writer = new BinaryWriter(stream))
        {
          writer.Write(SourceName);
          writer.Write(Line);
          
          writer.Flush();

          return new ReadOnlyCollection<byte>(stream.ToArray());
        }
      }
    }

    public bool ReadFrom(byte[] data)
    {
      using (var stream = new MemoryStream(data))
      {
        using (var reader = new BinaryReader(stream))
        { 
          SourceName = reader.ReadString();
          Line       = reader.ReadUInt64();
        }
      }

      return true;
    }
  }

  public class RemoteProcessData : DkmDataItem
  {
    public DkmLanguage   Language = null;
    public DkmCompilerId CompilerID;

    public DkmCustomRuntimeInstance RuntimeInstance = null;
    public DkmCustomModuleInstance  ModuleInstance  = null;
    public DkmModule                Module          = null;
    public DkmStepper               ActiveStepper   = null;

    public SquirrelLocations        Locations       = null;
    public List<BreakpointData>     ActiveBreakpoints = new List<BreakpointData>();
  }
}
