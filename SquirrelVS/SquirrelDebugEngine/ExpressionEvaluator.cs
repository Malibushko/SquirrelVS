using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.CallStack;
using Microsoft.VisualStudio.Debugger.CustomRuntimes;
using Microsoft.VisualStudio.Debugger.Evaluation;
using Microsoft.VisualStudio.Debugger.Native;
using SquirrelDebugEngine.Proxy;

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
      // First check if expression is a squirrel local variable
      var StackData = _StackFrame.Data.GetDataItem<SquirrelStackFrameData>();

      if (StackData != null)
      {
        var Locals = StackData.NativeFrame.GetFrameLocals();

        foreach (var LocalVariable in Locals)
        {
          if (LocalVariable.Name == _Expression.Text.Trim())
          {
            var SuccessEvaluationResult = CreateSquirrelSuccessEvaluationResult(
                  _StackFrame.RuntimeInstance,
                  _InspectionContext,
                  _StackFrame,
                  LocalVariable
                );

            _CompletionRoutine(new DkmEvaluateExpressionAsyncResult(SuccessEvaluationResult));

            return;
          }
        }
      }

      // Otherwise try evaluate cpp expression
      var CompilerID = new DkmCompilerId(DkmVendorId.Microsoft, DkmLanguageId.Cpp);
      var CppLanguage = DkmLanguage.Create("C++", CompilerID);

      var Result = DkmIntermediateEvaluationResult.Create(
            _InspectionContext,
            _StackFrame,
            _Expression.Text,
            _Expression.Text,
            _Expression.Text,
            CppLanguage,
            _StackFrame.Process.GetNativeRuntimeInstance(),
            null
          );

      if (Result != null)
      {
        _CompletionRoutine(new DkmEvaluateExpressionAsyncResult(Result));

        return;
      }

      // Give up
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
      var Result = _Result as DkmSuccessEvaluationResult;

      if (Result == null)
      {
        _Result.GetChildren(_WorkList, _InitialRequestChild, _InspectionContext, _CompletionRoutine);

        return;
      }

      var LocalData     = Utility.GetOrCreateDataItem<LocalProcessData>(_InspectionContext.Thread.Process);
      var VariableData  = _Result.GetDataItem<FieldDataItem>();
      var Results       = new List<DkmEvaluationResult>();

      if (VariableData.Object == null)
        return;

      /*
       * Evaluate Native view
       */

      var CompilerID  = new DkmCompilerId(DkmVendorId.Microsoft, DkmLanguageId.Cpp);
      var CppLanguage = DkmLanguage.Create("C++", CompilerID);

      if (VariableData.Object is ISQObject)
      {
        var NativeAddress = (VariableData.Object is SQObject) ?
                                (VariableData.Object as SQObject).NativeValueAddress :
                                (VariableData.Object as ISQObject).Address;

        string CppExpression = EvaluationHelpers.GetExpressionForObject(
            LocalData.SquirrelModule.Name,
            VariableData.Object.GetDisplayNativeType(),
            "0x" + NativeAddress.ToString("x"),
            ",!"
          );
        
        var EvaluationResult = DkmIntermediateEvaluationResult.Create(
              _InspectionContext,
              _Result.StackFrame,
              "[Native View]",
              "{C++}" + CppExpression,
              CppExpression,
              CppLanguage,
              _Result.StackFrame.Process.GetNativeRuntimeInstance(),
              null
            );
        
        Results.Add(EvaluationResult);
      }

      /*
       * Evaluate Squirrel View
       */
      if (VariableData.Object is IVisualizableObject)
      {
        foreach (var Child in VariableData.Object.GetChildren())
        {
          var EvaluationFlags = Child.Object.GetEvaluationFlags();

          if (Child.Object.IsNativeExpression())
          {
            string CppExpression = EvaluationHelpers.GetExpressionForObject(
             LocalData.SquirrelModule.Name,
             Child.Object.GetDisplayNativeType(),
             Child.Object.GetDisplayValue(),
             ",!"
           );

           var EvaluationResult = DkmIntermediateEvaluationResult.Create(
               _InspectionContext,
               _Result.StackFrame,
               Child.Name,
               "{C++}" + CppExpression,
               CppExpression,
               CppLanguage,
               _Result.StackFrame.Process.GetNativeRuntimeInstance(),
               null
             );

            Results.Add(EvaluationResult);
          }
          else
          {
            Results.Add(DkmSuccessEvaluationResult.Create(
                _InspectionContext,
                _Result.StackFrame,
                Child.Name,
                Child.Name,
                EvaluationFlags.Flags,
                Child.Object.GetDisplayValue(),
                null,
                Child.Object.GetDisplayType(),
                EvaluationFlags.Category,
                EvaluationFlags.AccessType,
                EvaluationFlags.StorageType,
                EvaluationFlags.TypeModifierFlags,
                DkmDataAddress.Create(_InspectionContext.RuntimeInstance, 0, null),
                null,
                null,
                new FieldDataItem { Object = Child.Object, Name = Child.Name }
              ));
          }
          }
      }

      _CompletionRoutine(
          new DkmGetChildrenAsyncResult(
            Results.ToArray(),
            DkmEvaluationResultEnumContext.Create(
            Results.Count,
            _Result.StackFrame,
            _InspectionContext,
            null
          )
         )
       );
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

      if (Variables?.Variables != null)
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
          Results[i] = CreateSquirrelSuccessEvaluationResult(
              Data.RuntimeInstance,
              _EnumContext.InspectionContext,
              _EnumContext.StackFrame,
              Variables.Variables[i]
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

    #region Service

    DkmSuccessEvaluationResult CreateSquirrelSuccessEvaluationResult(
        DkmRuntimeInstance   _RuntimeInstance,
        DkmInspectionContext _InspectionContext,
        DkmStackWalkFrame    _StackFrame,
        SquirrelVariableInfo _VariableInfo
      )
    {
      DkmDataAddress DataAddress    = DkmDataAddress.Create(_RuntimeInstance, 0, null);
      SQObject       SquirrelObject = _VariableInfo.Value;

      var EvaluationFlags = SquirrelObject.GetEvaluationFlags();

      return DkmSuccessEvaluationResult.Create(
            _InspectionContext,
            _StackFrame,
            _VariableInfo.Name,
            _VariableInfo.Name,
            EvaluationFlags.Flags,
            SquirrelObject.GetDisplayValue(),
            null,
            SquirrelObject.GetDisplayType(),
            EvaluationFlags.Category,
            EvaluationFlags.AccessType,
            EvaluationFlags.StorageType,
            EvaluationFlags.TypeModifierFlags,
            DataAddress,
            null,
            null,
            new FieldDataItem { Name = _VariableInfo.Name, Object = SquirrelObject }
          );
    }

    #endregion
  }
}
