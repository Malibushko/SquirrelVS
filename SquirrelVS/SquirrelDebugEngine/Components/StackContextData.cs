using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine
{
  internal class SquirrelStackContextData : DkmDataItem
  {
    public ulong HandleAddress   = 0;
    public bool  SeenSquirrelFrames = false;
    public int   SkipFramesCount = 0;
    public int   SeenFramesCount = 0;

    public bool HideTopLibraryFrames      = false;
    public bool HideInternalLibraryFrames = false;

    public bool  HasPreparedStateAddress = false;
    public ulong PreparedStateAddress   = 0;
  }
}
