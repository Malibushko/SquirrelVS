using System;
using System.Collections.Generic;
using System.Linq;
using SquirrelSyntaxHighlight.Infrastructure;
using SquirrelSyntaxHighlight.Parsing;
using SquirrelSyntaxHighlight.Common;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

namespace SquirrelSyntaxHighlight.Editor.Indent
{

  internal static class AutoIndent
  {
    internal static int GetIndentation(
        string _Line, 
        int    _TabSize
      )
    {
      int Indent = 0;

      for (int i = 0; i < _Line.Length; i++)
      {
        if (_Line[i] == ' ')
          Indent++;
        else if (_Line[i] == '\t')
          Indent += _TabSize;
        else
          break;
      }

      return Indent;
    }

    private struct LineInfo
    {
      public static readonly LineInfo Empty = new LineInfo();

      public bool NeedsUpdate;
      public int  Indentation;
      public bool ShouldIndentAfter;
      public bool ShouldDedentAfter;
    }

    private static int CalculateIndentation(
        string                 _BaseLine,
        ITextSnapshotLine      _Line,
        IEditorOptions         _Options,
        SquirrelTextBufferInfo _Buffer
      )
    {
      var Snapshot = _Line.Snapshot;
      
      if (Snapshot.TextBuffer != _Buffer.Buffer)
        throw new ArgumentException("Buffer mismatch");

      if (_BaseLine.Length <= 1)
        return 0;

      int Indentation = GetIndentation(_BaseLine, _Options.GetTabSize());

      return Indentation;
    }
    
    private static bool IsBlankLine(
        string _LineText
      )
    {
      foreach (char Char in _LineText)
      {
        if (!Char.IsWhiteSpace(Char))
          return false;
      }

      return true;
    }
    private static void SkipPreceedingBlankLines(
        ITextSnapshotLine     _Line, 
        out string            _BaseLineText, 
        out ITextSnapshotLine _BaseLine
      )
    {
      string Text;

      while (_Line.LineNumber > 0)
      {
        _Line = _Line.Snapshot.GetLineFromLineNumber(_Line.LineNumber - 1);
        
        Text = _Line.GetText();
        
        if (!IsBlankLine(Text))
        {
          _BaseLine    = _Line;
          _BaseLineText = Text;
          
          return;
        }
      }

      _BaseLineText = _Line.GetText();
      _BaseLine     = _Line;
    }

    internal static int? GetLineIndentation(
        SquirrelTextBufferInfo _Buffer, 
        ITextSnapshotLine      _Line, 
        ITextView              _TextView
      )
    {
      if (_Buffer == null)
        return 0;
      
      var Options = _TextView.Options;

      ITextSnapshotLine BaseLine;
      string            BaseLineText;

      SkipPreceedingBlankLines(_Line, out BaseLineText, out BaseLine);

      var LineStart = _Line.Start;

      if (!LineStart.Snapshot.TextBuffer.ContentType.IsOfType(SquirrelConstants.SquirrelContentType))
        return null;

      var DesiredIndentation = CalculateIndentation(BaseLineText, BaseLine, Options, _Buffer);
      
      if (DesiredIndentation < 0)
        DesiredIndentation = 0;

      var CaretPosition = _TextView.MapDownToBuffer(_TextView.Caret.Position.BufferPosition, LineStart.Snapshot.TextBuffer);
      var CaretLine     = CaretPosition?.GetContainingLine();

      // VS will get the white space when the user is moving the cursor or when the user is doing an edit which
      // introduces a new line.  When the user is moving the cursor the caret line differs from the line
      // we're querying.  When editing the lines are the same and so we want to account for the white space of
      // non-blank lines.  An alternate strategy here would be to watch for the edit and fix things up after
      // the fact which is what would happen pre-Dev10 when the language would not get queried for non-blank lines
      // (and is therefore what C# and other languages are doing).
      if (CaretLine != null && CaretLine.LineNumber == _Line.LineNumber)
      {
        var LineText          = CaretLine.GetText();
        int IndentationUpdate = 0;

        for (int i = CaretPosition.Value.Position - CaretLine.Start; i < LineText.Length; i++)
        {
          if (LineText[i] == ' ')
            IndentationUpdate++;
          else if (LineText[i] == '\t')
            IndentationUpdate += _TextView.Options.GetTabSize();
          else
          {
            if (IndentationUpdate > DesiredIndentation)
            {
              // we would dedent this line (e.g. there's a return on the previous line) but the user is
              // hitting enter with a statement to the right of the caret and they're in the middle of white space.
              // So we need to instead just maintain the existing indentation level.
              DesiredIndentation = Math.Max(GetIndentation(BaseLineText, Options.GetTabSize()) - IndentationUpdate, 0);
            }
            else
              DesiredIndentation -= IndentationUpdate;
            
            break;
          }
        }
      }

      // Map indentation back to the view's text buffer.
      if (_TextView.TextBuffer != LineStart.Snapshot.TextBuffer)
      {
        var ViewLineStart = _TextView.BufferGraph.MapUpToSnapshot(
            LineStart,
            PointTrackingMode.Positive,
            PositionAffinity.Successor,
            _TextView.TextSnapshot
          );

        if (ViewLineStart.HasValue)
          DesiredIndentation += ViewLineStart.Value - ViewLineStart.Value.GetContainingLine().Start;
      }

      return DesiredIndentation;
    }
  }
}
