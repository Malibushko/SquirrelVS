using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class HookData
  {
    public ulong TraceRoutineFlagAddress;
    public ulong TraceRoutine;

    public ulong TraceRoutineAddress;

    public byte[] Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(TraceRoutineFlagAddress);
          Writer.Write(TraceRoutine);
          Writer.Write(TraceRoutineAddress);

          Writer.Flush();

          return Stream.ToArray();
        }
      }
    }

    public bool Decode(byte[] data)
    {
      using (var Stream = new MemoryStream(data))
      {
        using (var Reader = new BinaryReader(Stream))
        {
          TraceRoutineFlagAddress = Reader.ReadUInt64();
          TraceRoutine        = Reader.ReadUInt64();
          TraceRoutineAddress      = Reader.ReadUInt64();
        }
      }

      return true;
    }
  }
}
