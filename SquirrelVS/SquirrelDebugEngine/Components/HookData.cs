using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class HookData
  {
    public ulong DebugHookNativeAddress;
    public ulong DebugHookNative;

    public ulong HelperHookAddress;

    public byte[] Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(DebugHookNativeAddress);
          Writer.Write(DebugHookNative);
          Writer.Write(HelperHookAddress);

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
          DebugHookNativeAddress = Reader.ReadUInt64();
          DebugHookNative        = Reader.ReadUInt64();
          HelperHookAddress      = Reader.ReadUInt64();
        }
      }

      return true;
    }
  }
}
