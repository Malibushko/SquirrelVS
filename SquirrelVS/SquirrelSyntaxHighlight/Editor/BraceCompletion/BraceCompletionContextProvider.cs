using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.BraceCompletion;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Python.Parsing;

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

    public bool TryCreateContext(ITextView textView, SnapshotPoint openingPoint, char openingBrace, char closingBrace, out IBraceCompletionContext context)
    {
      var bi = SquirrelTextBufferInfo.ForBuffer(Site, openingPoint.Snapshot.TextBuffer);
      if (IsValidBraceCompletionContext(bi, openingPoint, openingBrace))
      {
        context = new BraceCompletionContext();
        return true;
      }
      else
      {
        context = null;
        return false;
      }
    }

    private static bool IsValidBraceCompletionContext(SquirrelTextBufferInfo buffer, SnapshotPoint openingPoint, char openingBrace)
    {
      if (buffer == null)
      {
        return false;
      }

      Debug.Assert(openingPoint.Position >= 0, "SnapshotPoint.Position should always be zero or positive.");
      if (openingPoint.Position < 0)
      {
        return false;
      }

      switch (openingBrace)
      {
        case '(':
        case '[':
        case '{':
        {
          // Valid anywhere, including comments / strings
          return true;
        }

        case '"':
        case '\'':
        {
          // Not valid in comment / strings, so user can easily type triple-quotes
          var category = buffer.GetTokenAtPoint(openingPoint)?.Category ?? TokenCategory.None;
          return !(
              category == TokenCategory.Comment ||
              category == TokenCategory.LineComment ||
              category == TokenCategory.DocComment ||
              category == TokenCategory.StringLiteral ||
              category == TokenCategory.IncompleteMultiLineStringLiteral
          );
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
