using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class BreakpointHitData
  {
    public Guid BreakpointID;
    public Guid ThreadID;

    public ulong ReturnAddress;
    public ulong FrameBase;
    public ulong vFrame;

    public byte[] Encode()
    {
      using (var stream = new MemoryStream())
      {
        using (var writer = new BinaryWriter(stream))
        {
          writer.Write(BreakpointID.ToByteArray());
          writer.Write(ThreadID.ToByteArray());

          writer.Write(ReturnAddress);
          writer.Write(FrameBase);
          writer.Write(vFrame);

          writer.Flush();

          return stream.ToArray();
        }
      }
    }

    public bool Decode(byte[] data)
    {
      using (var stream = new MemoryStream(data))
      {
        using (var reader = new BinaryReader(stream))
        {
          BreakpointID  = new Guid(reader.ReadBytes(16));
          ThreadID      = new Guid(reader.ReadBytes(16));

          ReturnAddress = reader.ReadUInt64();
          FrameBase     = reader.ReadUInt64();
          vFrame        = reader.ReadUInt64();
        }
      }

      return true;
    }
  }
}
