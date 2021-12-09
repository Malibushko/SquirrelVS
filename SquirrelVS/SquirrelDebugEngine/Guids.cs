﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine
{
  static public class Guids
  {
    public static readonly Guid SquirrelLocalComponentID  = new Guid("8557CDDA-FC8B-4BB3-A47B-BBBC86F36402");
    public static readonly Guid SquirrelRemoteComponentID = new Guid("8086DEE7-4FCD-4C9E-9F6F-8C4B8237E7A3");

    public static readonly Guid SquirrelCompilerID        = new Guid("5E92C4F0-A9CA-49BB-8858-1EADF11B65BB");
    public static readonly Guid SquirrelLanguageID        = new Guid("00F5E22A-3249-4481-9081-0E88AF62672E");
    public static readonly Guid SquirrelRuntimeID         = new Guid("F0BD6D38-5132-4CC1-B97B-BF5FAC380EDC");
    public static readonly Guid SquirrelSymbolProviderID  = new Guid("3AFE0003-855D-48EA-8C56-1024CB473B52");

    public static readonly Guid SquirrelSupportBreakpointID = new Guid("D58A34F5-0573-4A23-B2BE-39486E6DACAA");
  }

  static class MessageToLocal
  {
    public static readonly Guid Guid = new Guid("E98BF0CF-D479-4F2F-9B05-F0A5B2877175");

    public enum MessageType
    {
      BreakpointHit,
      Symbols,
      ComponentException
    };
  }

  static class MessageToRemote
  {
    public static readonly Guid Guid = new Guid("A84822CB-A3B4-41A7-853E-9C83EA0A900F");

    public enum MessageType
    {
      CreateRuntime,
      LocateHelper,
      PauseBreakpoints,
      ResumeBreakpoints,
      BreakpointsInfo,
      RegisterState
    }
  }

  static class MessageToLocalWorker
  {
    public static readonly Guid Guid = new Guid("F59348DD-8760-49C4-B2D7-EF76E8AD51F9");

    public enum MessageType
    {
      FetchSquirrelSymbols
    }
  }
}