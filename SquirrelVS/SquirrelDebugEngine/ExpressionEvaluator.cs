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
      SquirrelFunctionVariables Variables = TryExtraxtFrameVariables(
          _StackFrame.Process,
          _InspectionContext.InspectionSession,
          _StackFrame.Data.GetDataItem<SquirrelStackFrameData>()
        );

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
              Variable.Value,
              null,
              Variable.ItemType.ToString(),
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

    private SquirrelFunctionVariables TryExtraxtFrameVariables(
        DkmProcess             _Process,
        DkmInspectionSession   _Session,
        SquirrelStackFrameData _Frame
     )
    {
      SquirrelFunctionVariables Variables = new SquirrelFunctionVariables()
      {
        Process = _Process
      };

      LocalProcessData  ProcessData = Utility.GetOrCreateDataItem<LocalProcessData>(_Process);
      DkmStackWalkFrame ParentFrame = _Frame.ParentFrame;

      if (ProcessData.SquirrelHandleAddress != 0 && _Frame.IndexFromTop != -1)
      {
        while (true)
        {
          try
          {
            string VariableName = EvaluationHelpers.TryEvaluateStringExpression(
                $"sq_getlocal({ProcessData.SquirrelHandleAddress}, {_Frame.IndexFromTop}, {Variables.Variables.Count})",
                _Session,
                ParentFrame.Thread,
                ParentFrame,
                DkmEvaluationFlags.None
              );

            if (VariableName == null)
              break;

            SquirrelVariableInfo Info = new SquirrelVariableInfo()
            {
              Name = VariableName == "this" ? "[SVQM]" : VariableName
            };

            long? VariableType = EvaluationHelpers.TryEvaluateNumberExpression(
                $"sq_gettype({ProcessData.SquirrelHandleAddress},-1)",
                _Session,
                ParentFrame.Thread,
                ParentFrame,
                DkmEvaluationFlags.None
              );

            if (VariableType.HasValue)
            {
              Info.ItemType = (SquirrelVariableInfo.Type)VariableType.Value;

              switch (Info.ItemType)
              {
                case SquirrelVariableInfo.Type.Null:
                  Info.Value = "null";
                  break;
                case SquirrelVariableInfo.Type.Integer:
                  {
                    /*
                   EvaluationHelpers.TryEvaluateStringExpression(
                      $"sq_getinteger({ProcessData.SquirrelHandleAddress}, -1, &Buffer)",
                      _Session,
                      ParentFrame.Thread,
                      ParentFrame,
                      DkmEvaluationFlags.None
                    );
                    */
                    // Info.Value = Utility.ReadUlongVariable(_Process, BufferLocations.IntegerBufferAddress).GetValueOrDefault(0).ToString();

                    break;
                  }
                case SquirrelVariableInfo.Type.Float:
                  break;
                case SquirrelVariableInfo.Type.UserPointer:
                  break;
                case SquirrelVariableInfo.Type.String:
                  {
                    /*
                   EvaluationHelpers.TryEvaluateStringExpression(
                      $"sq_getstring({ProcessData.SquirrelHandleAddress}, -1, {BufferLocations.StringBufferAddress})",
                      _Session,
                      ParentFrame.Thread,
                      ParentFrame,
                      DkmEvaluationFlags.None
                    );

                    ulong? StringPointer = Utility.ReadPointerVariable(_Process, BufferLocations.StringBufferAddress);

                    if (StringPointer.HasValue)
                      Info.Value = $"\"{Utility.ReadStringVariable(_Process, StringPointer.Value, 1024)}\"";
                    */
                    break;
                  }
                case SquirrelVariableInfo.Type.Closure:
                  break;
                case SquirrelVariableInfo.Type.Array:
                  break;
                case SquirrelVariableInfo.Type.NativeClosure:
                  break;
                case SquirrelVariableInfo.Type.Generator:
                  break;
                case SquirrelVariableInfo.Type.UserData:
                  break;
                case SquirrelVariableInfo.Type.Thread:
                  break;
                case SquirrelVariableInfo.Type.Class:
                  break;
                case SquirrelVariableInfo.Type.Instance:
                  break;
                case SquirrelVariableInfo.Type.WeakRef:
                  break;
                case SquirrelVariableInfo.Type.Bool:
                  break;
              }
            }
            Variables.Variables.Add(Info);

            // Pop result of sq_gettype
            EvaluationHelpers.TryEvaluateAddressExpression(
                $"sq_pop({ProcessData.SquirrelHandleAddress}, 1)",
                _Session,
                ParentFrame.Thread,
                ParentFrame,
                DkmEvaluationFlags.None
              );
          }
          catch (Exception Exception)
          {
            Debug.WriteLine($"Exception caught while getting frame locals: \'{Exception.Message}\'");
          }
        }
      }

      return Variables;
    }
  }
}
