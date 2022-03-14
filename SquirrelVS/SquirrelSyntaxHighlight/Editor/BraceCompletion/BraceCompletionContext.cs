using Microsoft.VisualStudio.Text.BraceCompletion;

namespace SquirrelSyntaxHighlight.Editor.BraceCompletion
{
  internal class BraceCompletionContext : IBraceCompletionContext
  {
    public bool AllowOverType(
        IBraceCompletionSession _Session
      )
    {
      return true;
    }

    public void Finish(
        IBraceCompletionSession _Session
      )
    {
      // Empty
    }

    public void OnReturn(
        IBraceCompletionSession _Session
      )
    {
      // Empty
    }

    public void Start(
        IBraceCompletionSession _Session
      )
    {
      // Empty
    }
  }
}
