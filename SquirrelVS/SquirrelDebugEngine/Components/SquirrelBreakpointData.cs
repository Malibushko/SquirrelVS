using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Debugger;

namespace SquirrelDebugEngine
{
  public class SquirrelBreakpointData
  {
    public ulong  Type;
    public string SourceName;
    public ulong  Line;
    public string FunctionName;

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
  }
}
