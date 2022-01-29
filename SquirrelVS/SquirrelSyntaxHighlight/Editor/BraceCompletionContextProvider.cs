using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(IBraceCompletionContextProvider))]
  [BracePair('(', ')')]
  [BracePair('[', ']')]
  [BracePair('{', '}')]
  [BracePair('"', '"')]
  [BracePair('\'', '\'')]
  [ContentType("Squirrel")]
  internal class BraceCompletionContextProvider : IBraceCompletionContextProvider
  {
    [Import(typeof(SVsServiceProvider))]
    internal IServiceProvider Site = null;

    public bool TryCreateContext(
         ITextView                   _TextView, 
         SnapshotPoint               _OpeningPoint, 
         char                        _OpeningBrace, 
         char                        _ClosingBrace, 
         out IBraceCompletionContext _Context
      )
    {
      _Context = new BraceCompletionContext();

      return true;
    }
  }
}
