using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.CodeSnippets
{
  [Export(typeof(ICommandHandler))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  [Name(nameof(InsertSnippetCommandHandler))]
  internal class InsertSnippetCommandHandler :
      ICommandHandler<InsertSnippetCommandArgs>,
      ICommandHandler<SurroundWithCommandArgs>,
      ICommandHandler<TabKeyCommandArgs>,
      ICommandHandler<BackTabKeyCommandArgs>,
      ICommandHandler<ReturnKeyCommandArgs>,
      ICommandHandler<EscapeKeyCommandArgs>
  {
    private readonly SnippetManager SnippetManager;

    [ImportingConstructor]
    public InsertSnippetCommandHandler(
        [Import] SnippetManager _SnippetManager
      )
    {
      SnippetManager = _SnippetManager ?? throw new ArgumentNullException(nameof(_SnippetManager));
    }

    public string DisplayName => nameof(InsertSnippetCommandHandler);

    public CommandState GetCommandState(InsertSnippetCommandArgs args) => CommandState.Available;

    public CommandState GetCommandState(SurroundWithCommandArgs args) => CommandState.Available;

    public CommandState GetCommandState(TabKeyCommandArgs args) => CommandState.Unspecified;

    public CommandState GetCommandState(BackTabKeyCommandArgs args) => CommandState.Unspecified;

    public CommandState GetCommandState(ReturnKeyCommandArgs args) => CommandState.Unspecified;

    public CommandState GetCommandState(EscapeKeyCommandArgs args) => CommandState.Unspecified;

    public bool ExecuteCommand(
        InsertSnippetCommandArgs _Args, 
        CommandExecutionContext  _ExecutionContext
      )
    {
      return SnippetManager.ShowInsertionUI(_Args.TextView, _IsSurroundsWith: false);
    }

    public bool ExecuteCommand(
        SurroundWithCommandArgs _Args,
        CommandExecutionContext _ExecutionContext
      )
    {
      return SnippetManager.ShowInsertionUI(_Args.TextView, _IsSurroundsWith: true);
    }

    public bool ExecuteCommand(
        TabKeyCommandArgs       _Args, 
        CommandExecutionContext _ExecutionContext
      )
    {
      if (SnippetManager.IsInSession(_Args.TextView))
        return SnippetManager.MoveToNextField(_Args.TextView);
      else
        return SnippetManager.TryTriggerExpansion(_Args.TextView);
    }

    public bool ExecuteCommand(
        BackTabKeyCommandArgs   _Args, 
        CommandExecutionContext _ExecutionContext
      )
    {
      if (SnippetManager.IsInSession(_Args.TextView))
        return SnippetManager.MoveToPreviousField(_Args.TextView);

      return false;
    }

    public bool ExecuteCommand(
        ReturnKeyCommandArgs    _Args, 
        CommandExecutionContext _ExecutionContext
      )
    {
      if (SnippetManager.IsInSession(_Args.TextView))
        return SnippetManager.EndSession(_Args.TextView, _LeaveCaret: false);

      return false;
    }

    public bool ExecuteCommand(
        EscapeKeyCommandArgs    _Args, 
        CommandExecutionContext _ExecutionContext
      )
    {
      if (SnippetManager.IsInSession(_Args.TextView))
        return SnippetManager.EndSession(_Args.TextView, _LeaveCaret: true);

      return false;
    }
  }
}
