using System.Linq;
using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelDebugEngine
{
  public class CallstackFrame
  {
    public string SourceName;
    public string FunctionName;
    public ulong  Line;

    public CallstackFrame()
    {
    }
    public CallstackFrame(
        DkmProcess _Process,
        ulong      _Address
      )
    {
      ulong Offset = 0;

      var SourceNameAddress = Utility.ReadPointerVariable(_Process, _Address + Offset);

      if (SourceNameAddress.HasValue)
        SourceName = Utility.ReadStringVariable(_Process, SourceNameAddress.Value, 4096);

      Offset += sizeof(ulong);

      var FunctionNameAddress = Utility.ReadPointerVariable(_Process, _Address + Offset);

      if (FunctionNameAddress.HasValue)
        FunctionName = Utility.ReadStringVariable(_Process, FunctionNameAddress.Value, 4096);

      Offset += sizeof(ulong);

      Line = Utility.ReadUlongVariable(_Process, _Address + Offset).GetValueOrDefault(0);
    }
    public ReadOnlyCollection<byte> Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
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
          SourceName   = Reader.ReadString();
          Line         = Reader.ReadUInt64();
          FunctionName = Reader.ReadString();
        }
      }

      return true;
    }
  }
}
