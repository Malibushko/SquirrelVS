using System.Linq;
using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System.IO;
using SquirrelDebugEngine.Proxy;
using System;

namespace SquirrelDebugEngine
{
  internal class CallstackFrame
  {
    private CallInfo  NativeFrame;
    
    private string    CachedFunctionName;
    private string    CachedSourceName;
    private long      CachedLine;

    internal CallstackFrame(
        CallInfo _NativeFrame
      )
    {
      NativeFrame = _NativeFrame;
      CachedLine  = 0;
    }

    internal string SourceName
    {
      get
      {
        if (CachedSourceName == null)
        {
          if (FunctionProto == null)
            CachedSourceName = "<failed to get source name>";
          else
            CachedSourceName = (FunctionProto.SourceName as SQString).Read();
        }

        return CachedSourceName;
      }
    }
    internal string FunctionName
    {
      get
      {
        if (CachedFunctionName == null)
        {
          if (FunctionProto == null)
            CachedFunctionName = "<failed to get function name>";
          else
            CachedFunctionName = (FunctionProto.Name as SQString).Read();
        }

        return CachedFunctionName;
      }
    }

    internal string FrameName
    {
      get
      {
        if (FunctionProto == null)
          return "<failed to get frame name>";

        return $"{FunctionName}({String.Join(",", FunctionProto.GetInputParameterNames())}), line {Line}";
      }
    }
    internal long Line
    {
      get
      {
        if (CachedLine == 0)
        {
          if (FunctionProto == null)
            CachedLine = 1;
          else
            CachedLine = FunctionProto.GetLine(NativeFrame.InstructionPointer);
        }

        return CachedLine;
      }
    }

    private SQFunctionProto FunctionProto
    {
      get
      {
        if (NativeFrame?.Closure == null)
          return null;

        var FunctionObject = NativeFrame.Closure.Function;

        if (FunctionObject?.Type.Read() != SquirrelVariableInfo.Type.FuncProto)
          return null;

        return FunctionObject.Value as SQFunctionProto;
      }
    }
  }
}
