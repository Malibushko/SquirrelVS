using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class SquirrelLocations
  {
    public ulong OpenStartLocation; // sq_open
    public ulong OpenEndLocation;

    public ulong CloseStartLocation; // sq_close
    public ulong CloseEndLocation;

    public ulong LoadFileStartLocation; // sqstd_loadfile
    public ulong LoadFileEndLocation;

    public ulong StackObjectStartLocation;
    public ulong StackObjectEndLocation;

    public ulong CallStartLocation; // sq_call
    public ulong CallEndLocation;

    public ulong HelperStartAddress;
    public ulong HelperEndAddress;

    public ulong HelperSQUnicodeFlagAddress;
    public ulong StringBufferAddress;

    public ulong CallstackAddress;
    public ulong CallstackSizeAddress;

    public byte[] Encode()
    {
      using (var Stream = new MemoryStream())
      {
        using (var Writer = new BinaryWriter(Stream))
        {
          Writer.Write(OpenStartLocation);
          Writer.Write(OpenEndLocation);

          Writer.Write(CloseStartLocation);
          Writer.Write(CloseEndLocation);

          Writer.Write(LoadFileStartLocation);
          Writer.Write(LoadFileEndLocation);

          Writer.Write(StackObjectStartLocation);
          Writer.Write(StackObjectEndLocation);

          Writer.Write(HelperStartAddress);
          Writer.Write(HelperEndAddress);

          Writer.Write(CallStartLocation);
          Writer.Write(CallEndLocation);

          Writer.Write(HelperSQUnicodeFlagAddress);
          Writer.Write(StringBufferAddress);

          Writer.Write(CallstackAddress);
          Writer.Write(CallstackSizeAddress);

          Writer.Flush();

          return Stream.ToArray();
        }
      }
    }

    public bool ReadFrom(byte[] _Data)
    {
      using (var Stream = new MemoryStream(_Data))
      {
        using (var Reader = new BinaryReader(Stream))
        {
          OpenStartLocation = Reader.ReadUInt64();
          OpenEndLocation   = Reader.ReadUInt64();

          CloseStartLocation = Reader.ReadUInt64();
          CloseEndLocation   = Reader.ReadUInt64();

          LoadFileStartLocation = Reader.ReadUInt64();
          LoadFileEndLocation   = Reader.ReadUInt64();

          StackObjectStartLocation = Reader.ReadUInt64();
          StackObjectEndLocation   = Reader.ReadUInt64();

          HelperStartAddress = Reader.ReadUInt64();
          HelperEndAddress   = Reader.ReadUInt64();

          CallStartLocation = Reader.ReadUInt64();
          CallEndLocation   = Reader.ReadUInt64();

          HelperSQUnicodeFlagAddress = Reader.ReadUInt64();
          StringBufferAddress        = Reader.ReadUInt64();

          CallstackAddress     = Reader.ReadUInt64();
          CallstackSizeAddress = Reader.ReadUInt64();
        }
      }

      return true;
    }
  }
}
