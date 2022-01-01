using System.Collections.ObjectModel;
using System.IO;

namespace SquirrelDebugEngine
{
  public class SourceLocation
  {
    public string Source;
    public long   Line;

    public ReadOnlyCollection<byte> Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(Source);
          Writer.Write(Line);
          
          Writer.Flush();

          return new ReadOnlyCollection<byte>(Stream.ToArray());
        }
      }
    }

    static public SourceLocation Decode(byte[] _Data)
    {
      using (var Stream = new MemoryStream(_Data))
      {
        using (var Reader = new BinaryReader(Stream))
        {
          SourceLocation Location = new SourceLocation
          {
            Source = Reader.ReadString(),
            Line   = Reader.ReadInt64()
          };

          return Location;
        }
      }
    }
  }
}
