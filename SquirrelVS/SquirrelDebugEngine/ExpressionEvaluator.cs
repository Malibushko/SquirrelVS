using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;

namespace SquirrelDebugEngine
{
  internal class ExpressionEvaluator : DkmDataItem
  {
    public void EvaluateExpression(
         DkmInspectionContext                                   _InspectionContext,
         DkmWorkList                                            _WorkList,
         DkmLanguageExpression                                  _Expression,
         DkmStackWalkFrame                                      _StackFrame,
         DkmCompletionRoutine<DkmEvaluateExpressionAsyncResult> _CompletionRoutine
       )
    {
      _InspectionContext.EvaluateExpression(_WorkList, _Expression, _StackFrame, _CompletionRoutine);
    }

    public void GetChildren(
        DkmEvaluationResult                             _Result,
        DkmWorkList                                     _WorkList,
        int                                             _InitialRequestChild,
        DkmInspectionContext                            _InspectionContext,
        DkmCompletionRoutine<DkmGetChildrenAsyncResult> _CompletionRoutine
      )
    {
      _Result.GetChildren(_WorkList, _InitialRequestChild, _InspectionContext, _CompletionRoutine);
    }

    public void GetFrameLocals(
        DkmInspectionContext                               _InspectionContext,
        DkmWorkList                                        _WorkList,
        DkmStackWalkFrame                                  _StackFrame,
        DkmCompletionRoutine<DkmGetFrameLocalsAsyncResult> _CompletionRoutine
      )
    {
      var FrameData = _StackFrame.Data.GetDataItem<SquirrelStackFrameData>();

      SquirrelFunctionVariables Variables = new SquirrelFunctionVariables()
      {
        Process   = _StackFrame.Process,
        Variables = FrameData.NativeFrame.GetFrameLocals()
      };

      if (Variables != null)
        _CompletionRoutine(new DkmGetFrameLocalsAsyncResult(DkmEvaluationResultEnumContext.Create(Variables.Variables.Count, _StackFrame, _InspectionContext, Variables)));
      else
        _InspectionContext.GetFrameLocals(_WorkList, _StackFrame, _CompletionRoutine);
    }

    public void GetFrameArguments(
        DkmInspectionContext                                  _InspectionContext,
        DkmWorkList                                           _WorkList,
        DkmStackWalkFrame                                     _Frame,
        DkmCompletionRoutine<DkmGetFrameArgumentsAsyncResult> _CompletionRoutine
      )
    {
      _CompletionRoutine(new DkmGetFrameArgumentsAsyncResult(new DkmEvaluationResult[0]));
    }

    public void GetItems(
        DkmEvaluationResultEnumContext                     _EnumContext,
        DkmWorkList                                        _WorkList,
        int                                                _StartIndex,
        int                                                _Count,
        DkmCompletionRoutine<DkmEvaluationEnumAsyncResult> _CompletionRoutine
      )
    {
      SquirrelFunctionVariables Variables = _EnumContext.GetDataItem<SquirrelFunctionVariables>();

      if (Variables != null)
      {
        LocalProcessData Data = Utility.GetOrCreateDataItem<LocalProcessData>(Variables.Process);

        int ActualCount = Math.Min(Variables.Variables.Count, _Count);

        DkmEvaluationResult[] Results = new DkmEvaluationResult[ActualCount];

        for (int i = _StartIndex; i < _StartIndex + ActualCount; ++i)
        {
          var Variable = Variables.Variables[i];

          DkmDataAddress dataAddress = DkmDataAddress.Create(Data.RuntimeInstance, 0, null);

          Results[i] = DkmSuccessEvaluationResult.Create(
              _EnumContext.InspectionContext,
              _EnumContext.StackFrame,
              Variable.Name,
              Variable.Name,
              DkmEvaluationResultFlags.ReadOnly,
              Variable.Value == null ? "<undefined>" : Variable.Value.ReadValue().ToString(),
              null,
              Variable.Value == null ? "<undefined>" : Variable.Value.Type.Read().ToString(),
              DkmEvaluationResultCategory.Data,
              DkmEvaluationResultAccessType.Public,
              DkmEvaluationResultStorageType.None,
              DkmEvaluationResultTypeModifierFlags.None,
              dataAddress,
              null,
              null,
              null
            );
        }

        _CompletionRoutine(new DkmEvaluationEnumAsyncResult(Results));

        return;
      }
      _EnumContext.GetItems(_WorkList, _StartIndex, _Count, _CompletionRoutine);
    }

    public void SetValueAsString(
        DkmEvaluationResult _Result,
        string              _Value,
        int                 _Timeout,
        out string          _ErrorText
      )
    {
      _Result.SetValueAsString(_Value, _Timeout, out _ErrorText);
    }

    public string GetUnderlyingString(
        DkmEvaluationResult _Result
      )
    {
      return _Result.GetUnderlyingString();
    }
  }
}
