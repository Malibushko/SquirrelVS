using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.EditorFeatures.GoToDefinition
{
  [Export(typeof(ICommandHandler))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  [Name(nameof(GoToDefinitionCommandHandler))]
  internal class GoToDefinitionCommandHandler :
          ICommandHandler<GoToDefinitionCommandArgs>
  {
    public string DisplayName => "Go To Definition";

    public CommandState GetCommandState(GoToDefinitionCommandArgs args)
    {
      return CommandState.Available;
    }

    public bool ExecuteCommand(GoToDefinitionCommandArgs args, CommandExecutionContext context)
    {
      return false;
    }
  }
}
