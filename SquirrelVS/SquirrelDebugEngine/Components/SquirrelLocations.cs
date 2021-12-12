using Microsoft.VisualStudio.Debugger;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;

namespace SquirrelDebugEngine
{
  public class SquirrelLocations
  {
    public ulong OpenStartLocation;
    public ulong OpenEndLocation;

    public ulong CloseStartLocation;
    public ulong CloseEndLocation;

    public ulong LoadFileStartLocation;
    public ulong LoadFileEndLocation;

    public ulong StackObjectStartLocation;
    public ulong StackObjectEndLocation;
  }
}
