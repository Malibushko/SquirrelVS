using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.DefaultPort;
using Microsoft.VisualStudio.Debugger.Evaluation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SquirrelDebugEngine
{
  internal class EvaluationHelpers
  {
    internal static DkmInspectionSession CreateInspectionSession(
          DkmProcess _Process,
          DkmThread _Thread,
          ulong     _FrameBase,
          ulong     _vFrame,
          out DkmStackWalkFrame _Frame
        )
    {
      const int CV_ALLREG_VFRAME = 0x00007536;
      var FrameRegister = DkmUnwoundRegister.Create(CV_ALLREG_VFRAME, new ReadOnlyCollection<byte>(BitConverter.GetBytes(_vFrame)));
      var Registers = _Thread.GetCurrentRegisters(new[] { FrameRegister });
      var InstructionRegister = _Process.CreateNativeInstructionAddress(Registers.GetInstructionPointer());

      _Frame = DkmStackWalkFrame.Create(
          _Thread,
          InstructionRegister,
          _FrameBase,
          0,
          DkmStackWalkFrameFlags.None,
          null,
          Registers,
          null
        );

      return DkmInspectionSession.Create(_Process, null);
    }

    internal static ulong? TryEvaluateAddressExpression(
          string _Expression,
          DkmInspectionSession _Session,
          DkmThread _Thread,
          DkmStackWalkFrame _Input,
          Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags _Flags
        )
    {
      if (ExecuteExpression(_Expression, _Session, _Thread, _Input, _Flags, true, out ulong address) != null)
        return address;

      return null;
    }
    internal static long? TryEvaluateNumberExpression(string expression, DkmInspectionSession inspectionSession, DkmThread thread, DkmStackWalkFrame input, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags flags)
    {
      string result = ExecuteExpression(expression, inspectionSession, thread, input, flags, true, out ulong Address);

      if (result == null)
        return null;

      if (long.TryParse(result, out long value))
        return value;
      else
        return (long)Address;
    }

    internal static int GetPointerSize(
       DkmProcess _Process
     )
    {
      return Is64Bit(_Process) ? 8 : 4;
    }

    internal static bool Is64Bit(
        DkmProcess _Process
      )
    {
      // x32 not supported yet
      return true;
    }

    internal static string TryEvaluateStringExpression(string expression, DkmInspectionSession inspectionSession, DkmThread thread, DkmStackWalkFrame input, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags flags)
    {
      return ExecuteExpression(expression + ",sb", inspectionSession, thread, input, flags, false, out _);
    }

    internal static DkmEvaluationResult EvaluateCppExpression(
          string               _Expression,
          DkmInspectionSession _Session,
          DkmThread            _Thread,
          DkmStackWalkFrame    _Input,
          Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags   _Flags,
          uint                 _Timeout
        )
    {
      var CompilerID         = new DkmCompilerId(DkmVendorId.Microsoft, DkmLanguageId.Cpp);
      var CppLanguage        = DkmLanguage.Create("C++", CompilerID);
      var LanguageExpression = DkmLanguageExpression.Create(CppLanguage, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.None, _Expression, null);

      DkmInspectionContext InspectionContext = DkmInspectionContext.Create(
          _Session,
          _Input.RuntimeInstance,
          _Thread,
          _Timeout,
          _Flags,
          DkmFuncEvalFlags.None,
          10,
          CppLanguage,
          null
        );

      var WorkList = DkmWorkList.Create(null);

      try
      {
        DkmEvaluationResult Result = null;

        InspectionContext.EvaluateExpression(WorkList, LanguageExpression, _Input, AsyncResult =>
        {
          if (AsyncResult.ErrorCode == 0)
          {
            Result = AsyncResult.ResultObject as DkmSuccessEvaluationResult;

            AsyncResult.ResultObject.Close();
          }
        });

        WorkList.Execute();

        return Result;
      }
      catch (Exception _Ex)
      {
        return null;
      }
    }

    internal static string ExecuteExpression(
          string _Expression,
          DkmInspectionSession _Session,
          DkmThread _Thread,
          DkmStackWalkFrame _Input,
          Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags _Flags,
          bool _AllowZero,
          out ulong _Address
        )
    {
      var CompilerID = new DkmCompilerId(DkmVendorId.Microsoft, DkmLanguageId.Cpp);
      var CppLanguage = DkmLanguage.Create("C++", CompilerID);
      var LanguageExpression = DkmLanguageExpression.Create(CppLanguage, Microsoft.VisualStudio.Debugger.Evaluation.DkmEvaluationFlags.None, _Expression, null);

      DkmInspectionContext InspectionContext = DkmInspectionContext.Create(
          _Session,
          _Input.RuntimeInstance,
          _Thread,
          200,
          _Flags,
          DkmFuncEvalFlags.None,
          10,
          CppLanguage,
          null
        );

      var WorkList = DkmWorkList.Create(null);

      try
      {
        string TextResult = null;
        ulong ResultAddress = 0;

        InspectionContext.EvaluateExpression(WorkList, LanguageExpression, _Input, AsyncResult =>
        {
          if (AsyncResult.ErrorCode == 0)
          {
            var Result = AsyncResult.ResultObject as DkmSuccessEvaluationResult;

            if (Result != null && Result.Address == null) // void methods
            {
              TextResult   = Result.Value;
              ResultAddress = 0;
            }
            else
            if (Result != null && Result.TagValue == DkmEvaluationResult.Tag.SuccessResult &&
               (_AllowZero || Result.Address.Value != 0))
            {
              TextResult = Result.Value;
              ResultAddress = Result.Address.Value;
            }

            AsyncResult.ResultObject.Close();
          }
        });

        WorkList.Execute();

        _Address = ResultAddress;

        return TextResult;
      }
      catch (Exception _Ex)
      {
        _Address = 0;
        return _Ex.StackTrace;
      }
    }

    internal static string GetExpressionForObject(string moduleName, string typeName, string address, string tail = "")
    {
      string expr = string.Format("(*(::{0}*){1}ULL){2}", typeName, address, tail);
      if (moduleName != null)
      {
        expr = "{,," + moduleName + "}" + expr;
      }
      return expr;
    }
  }
}
