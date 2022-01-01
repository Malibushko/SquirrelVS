using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQInstruction")]
  internal class SQInstruction : StructProxy
  {
    public SQInstruction(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      // Empty
    }
  }
}
