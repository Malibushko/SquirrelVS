using System.Linq;
using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelDebugEngine
{
  public class SquirrelBreakpointData
  {
    public ulong  Type;
    public string SourceName;
    public ulong  Line;
    public string FunctionName;

    public SquirrelBreakpointData()
    {
    }
    public SquirrelBreakpointData(
        DkmProcess _Process,
        ulong      _BreakpointDataAddress
      )
    {
      ulong Offset = 0;

      Type = Utility.ReadUlongVariable(_Process, _BreakpointDataAddress + Offset).GetValueOrDefault(0);
      
      Offset += sizeof(ulong);

      var SourceNameAddress = Utility.ReadPointerVariable(_Process, _BreakpointDataAddress + Offset);
      if (SourceNameAddress.HasValue)
        SourceName = Utility.ReadStringVariable(_Process, SourceNameAddress.Value, 256);

      Offset += sizeof(ulong);

      Line = Utility.ReadUlongVariable(_Process, _BreakpointDataAddress + Offset).GetValueOrDefault(0);

      Offset += sizeof(ulong);

      var FunctionNameAddress = Utility.ReadPointerVariable(_Process, _BreakpointDataAddress + Offset);

      if (FunctionNameAddress.HasValue)
        FunctionName = Utility.ReadStringVariable(_Process, FunctionNameAddress.Value, 256);
    }
    public ReadOnlyCollection<byte> Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(Type);
          Writer.Write(SourceName);
          Writer.Write(Line);
          Writer.Write(FunctionName);

          Writer.Flush();

          return new ReadOnlyCollection<byte>(Stream.ToArray());
        }
      }
    }

    public bool ReadFrom(byte[] _Data)
    {
      using (var Stream = new MemoryStream(_Data))
      {
        using (var Reader = new BinaryReader(Stream))
        {
          Type         = Reader.ReadUInt64();
          SourceName   = Reader.ReadString();
          Line         = Reader.ReadUInt64();
          FunctionName = Reader.ReadString();
        }
      }

      return true;
    }
  }
}
