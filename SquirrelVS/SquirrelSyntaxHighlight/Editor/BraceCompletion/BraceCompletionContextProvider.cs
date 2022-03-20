using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using SquirrelSyntaxHighlight.Parsing;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.BraceCompletion
{
  [Export(typeof(IBraceCompletionContextProvider))]
  [BracePair('(', ')')]
  [BracePair('[', ']')]
  [BracePair('{', '}')]
  [BracePair('"', '"')]
  [BracePair('\'', '\'')]
  [BracePair('<', '>')]
  [ContentType(SquirrelConstants.SquirrelContentType)]
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
      var BufferInfo = SquirrelTextBufferInfo.ForBuffer(Site, _OpeningPoint.Snapshot.TextBuffer);
      
      if (IsValidBraceCompletionContext(BufferInfo, _OpeningPoint, _OpeningBrace))
      {
        _Context = new BraceCompletionContext();
        
        return true;
      }
      else
      {
        _Context = null;

        return false;
      }
    }

    private static bool IsValidBraceCompletionContext(
        SquirrelTextBufferInfo _Buffer, 
        SnapshotPoint          _OpeningPoint, 
        char                   _OpeningBrace
      )
    {
      if (_Buffer == null)
        return false;

      Debug.Assert(_OpeningPoint.Position >= 0, "SnapshotPoint.Position should always be zero or positive.");
      if (_OpeningPoint.Position < 0)
        return false;

      switch (_OpeningBrace)
      {
        case '(':
        case '[':
        case '{':
        case '"':
        case '\'':
        case '<':
        {
          // Valid anywhere, including comments / strings
          return true;
        }

        default:
        {
          Debug.Fail("Unexpected opening brace character.");
          return false;
        }
      }
    }
  }
}
