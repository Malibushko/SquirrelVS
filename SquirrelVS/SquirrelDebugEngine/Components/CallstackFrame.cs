using System.Linq;
using Microsoft.VisualStudio.Debugger;
using System.Collections.ObjectModel;
using System.IO;
using SquirrelDebugEngine.Proxy;
using System;
using System.Collections.Generic;

namespace SquirrelDebugEngine
{
  internal class CallstackFrame
  {
    private CallInfo  NativeFrame;
    
    private string    CachedFunctionName;
    private string    CachedSourceName;
    private long      CachedLine;

    internal Int32 PreviousStackBase { get; set; }

    internal long  StackBase { get; set; }

    internal ulong ParentFrameBase { get; set; }

    internal SQVM Thread { get; set; }

    internal CallstackFrame(
        CallInfo _NativeFrame
      )
    {
      NativeFrame = _NativeFrame;
      
      if (NativeFrame != null)
        PreviousStackBase = NativeFrame.PreviousStackBase;

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
            CachedSourceName = (FunctionProto.SourceName.Value as SQString).Read();
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
            CachedFunctionName = (FunctionProto.Name.Value as SQString).Read();
        }

        return CachedFunctionName;
      }
    }

    internal List<SquirrelVariableInfo> GetFrameLocals()
    {
      return FunctionProto?.GetLocals(Thread, StackBase, NativeFrame.InstructionPointer);
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
        if (NativeFrame?.Closure == null || NativeFrame?.Closure.Type != SquirrelVariableInfo.Type.Closure)
          return null;

        var FunctionObject = (NativeFrame.Closure.Value as SQClosure).Function;

        if (FunctionObject?.Type != SquirrelVariableInfo.Type.FuncProto)
          return null;

        return FunctionObject.Value as SQFunctionProto;
      }
    }

    public bool IsClosure()
    {
      return NativeFrame?.Closure?.Type == SquirrelVariableInfo.Type.Closure;
    }

    public bool IsNativeClosure()
    {
      return NativeFrame?.Closure?.Type == SquirrelVariableInfo.Type.NativeClosure;
    }

    public SQObjectPtr Closure
    {
      get
      {
        return NativeFrame?.Closure;
      }
    }
  }
}
