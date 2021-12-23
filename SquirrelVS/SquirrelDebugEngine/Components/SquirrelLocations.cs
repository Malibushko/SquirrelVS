using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System;
using System.IO;

namespace SquirrelDebugEngine
{
  internal class SquirrelLocations
  {
    public AddressRange SquirrelOpen;     // sq_open 
    public AddressRange SquirrelClose;    // sq_close
    public AddressRange SquirrelLoadFile; // sqstd_loadfile
    public AddressRange SquirrelCall;     // sq_call

    public UInt64Proxy IsSquirrelUnicode;
    
    public ulong       CallstackAddress;
    public UInt64Proxy CallstackSize;
  }
}
