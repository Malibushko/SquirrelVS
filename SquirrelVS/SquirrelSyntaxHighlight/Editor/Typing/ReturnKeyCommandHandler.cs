using System;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.Typing
{
  [Export(typeof(ICommandHandler))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  [Name(nameof(ReturnKeyCommandHandler))]
  internal class ReturnKeyCommandHandler : ICommandHandler<ReturnKeyCommandArgs>
  {
    public string DisplayName => nameof(ReturnKeyCommandHandler);

    public CommandState GetCommandState(ReturnKeyCommandArgs args) => CommandState.Unspecified;

    public bool ExecuteCommand(
        ReturnKeyCommandArgs    _Args, 
        CommandExecutionContext _ExecutionContext
      )
    {
      var Point = _Args.TextView.Caret.Position.BufferPosition;

      if (Point != null)
      {
        var Line         = Point.GetContainingLine();
        var LineText     = Point.Snapshot.GetText(Line.Start, Point - Line.Start);
        int CommentStart = LineText.IndexOf("//");
        
        if (CommentStart >= 0                                    &&
            Point <= Line.End                                    &&
            Line.Start + CommentStart < Point                    &&
            string.IsNullOrWhiteSpace(LineText.Remove(CommentStart))
          )
        {
          int Extra = LineText.Skip(CommentStart + 2).TakeWhile(char.IsWhiteSpace).Count() + 2;

          using (var Edit = Line.Snapshot.TextBuffer.CreateEdit())
          {
            Edit.Insert(
                Point.Position,
                _Args.TextView.Options.GetNewLineCharacter() + LineText.Substring(0, CommentStart + Extra)
              );

            Edit.Apply();
          }

          return true;
        }
      }

      return false;
    }
  } 
}
