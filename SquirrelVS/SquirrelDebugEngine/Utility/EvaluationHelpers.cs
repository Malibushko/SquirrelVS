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
          BreakpointHitData _Data,
          out DkmStackWalkFrame _Frame
        )
    {
      const int CV_ALLREG_VFRAME = 0x00007536;
      var FrameRegister = DkmUnwoundRegister.Create(CV_ALLREG_VFRAME, new ReadOnlyCollection<byte>(BitConverter.GetBytes(_Data.vFrame)));
      var Registers = _Thread.GetCurrentRegisters(new[] { FrameRegister });
      var InstructionRegister = _Process.CreateNativeInstructionAddress(Registers.GetInstructionPointer());

      _Frame = DkmStackWalkFrame.Create(
          _Thread,
          InstructionRegister,
          _Data.FrameBase,
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
          DkmEvaluationFlags _Flags
        )
    {
      if (ExecuteExpression(_Expression, _Session, _Thread, _Input, _Flags, true, out ulong address) != null)
        return address;

      return null;
    }
    internal static long? TryEvaluateNumberExpression(string expression, DkmInspectionSession inspectionSession, DkmThread thread, DkmStackWalkFrame input, DkmEvaluationFlags flags)
    {
      string result = ExecuteExpression(expression, inspectionSession, thread, input, flags, true, out ulong Address);

      if (result == null)
        return null;

      if (long.TryParse(result, out long value))
        return value;
      else
        return (long)Address;
    }

    internal static string TryEvaluateStringExpression(string expression, DkmInspectionSession inspectionSession, DkmThread thread, DkmStackWalkFrame input, DkmEvaluationFlags flags)
    {
      return ExecuteExpression(expression + ",sb", inspectionSession, thread, input, flags, false, out _);
    }

    internal static string ExecuteExpression(
          string _Expression,
          DkmInspectionSession _Session,
          DkmThread _Thread,
          DkmStackWalkFrame _Input,
          DkmEvaluationFlags _Flags,
          bool _AllowZero,
          out ulong _Address
        )
    {
      var CompilerID = new DkmCompilerId(DkmVendorId.Microsoft, DkmLanguageId.Cpp);
      var CppLanguage = DkmLanguage.Create("C++", CompilerID);
      var LanguageExpression = DkmLanguageExpression.Create(CppLanguage, DkmEvaluationFlags.None, _Expression, null);

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
      catch (OperationCanceledException)
      {
        _Address = 0;
        return null;
      }
    }
  }
}
