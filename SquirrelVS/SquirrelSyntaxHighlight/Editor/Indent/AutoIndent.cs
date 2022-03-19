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

      /*
      int Indentation = GetIndentation(_BaseLine, _Options.GetTabSize());
      int TabSize     = _Options.GetIndentSize();
      var Tokens      = _Buffer.GetTokens(_Line).ToList();

      while (Tokens.Count > 0 && IsWhitespace(Tokens[Tokens.Count - 1].Category))
        Tokens.RemoveAt(Tokens.Count - 1);

      if (Tokens.Count == 0 || IsUnterminatedStringToken(Tokens[Tokens.Count - 1], Snapshot))
        return Indentation;

      if (HasExplicitLineJoin(Tokens, Snapshot))
      {
        // explicit line continuation, we indent 1 level for the continued line unless
        // we're already indented because of multiple line continuation characters.
        Indentation = GetIndentation(_Line.GetText(), _Options.GetTabSize());

        var JoinedLine = _Line.LineNumber - 1;
        if (JoinedLine >= 0)
        {
          var PreviousLineTokens = _Buffer.GetTokens(Snapshot.GetLineFromLineNumber(JoinedLine)).ToList();
         
          if (PreviousLineTokens.Count == 0 || !HasExplicitLineJoin(PreviousLineTokens, Snapshot))
            Indentation += TabSize;
        }
        else
          Indentation += TabSize;

        return Indentation;
      }

      var TokenStack = new Stack<TrackingTokenInfo?>();
      
      TokenStack.Push(null);  // end with an implicit newline
      
      int EndAtLine = -1, CurrentLine = Tokens.Last().LineNumber;

      foreach (var Token in _Buffer.GetTokensInReverseFromPoint(Tokens.Last().ToSnapshotSpan(Snapshot).Start))
      {
        if (Token.LineNumber == CurrentLine)
          TokenStack.Push(Token);
        else
          TokenStack.Push(null);

        if (Token.LineNumber == EndAtLine)
          break;
        else if (Token.Category == TokenCategory.Keyword && PythonKeywords.IsOnlyStatementKeyword(Token.GetText(Snapshot), _Buffer.LanguageVersion))
          EndAtLine = Token.LineNumber - 1;

        if (Token.LineNumber != CurrentLine)
        {
          CurrentLine = Token.LineNumber;
          
          if (Token.Category != TokenCategory.WhiteSpace && Token.Category != TokenCategory.Comment && Token.Category != TokenCategory.LineComment)
            TokenStack.Push(Token);
        }
      }

      var IndentStack = new Stack<LineInfo>();
      var Current     = LineInfo.Empty;

      while (TokenStack.Count > 0)
      {
        var Token = TokenStack.Pop();
        
        if (Token == null)
        {
          Current.NeedsUpdate = true;
          continue;
        }

        var TokenLine = new Lazy<string>(() => Snapshot.GetLineFromLineNumber(Token.Value.LineNumber).GetText());

        if (IsOpenGrouping(Token.Value, Snapshot))
        {
          IndentStack.Push(Current);

          var Next = TokenStack.Count > 0 ? TokenStack.Peek() : null;
          if (Next != null && Next.Value.LineNumber == Token.Value.LineNumber)
          {
            // Put indent at same depth as grouping
            Current = new LineInfo
            {
              Indentation = Token.Value.ToSourceSpan().End.Column - 1
            };
          }
          else
          {
            // Put indent at one indent deeper than this line
            Current = new LineInfo
            {
              Indentation = GetIndentation(TokenLine.Value, TabSize) + TabSize
            };
          }
        }
        else if (IsCloseGrouping(Token.Value, Snapshot))
        {
          if (IndentStack.Count > 0)
            Current = IndentStack.Pop();
          else
            Current.NeedsUpdate = true;
        }
        else if (IsExplicitLineJoin(Token.Value, Snapshot))
        {
          while (Token != null && TokenStack.Count > 0)
            Token = TokenStack.Pop();
          
          if (!Token.HasValue)
            continue;
        }
        else if (Current.NeedsUpdate == true)
        {
          Current = new LineInfo
          {
            Indentation = GetIndentation(TokenLine.Value, TabSize)
          };
        }

        if (ShouldDedentAfterKeyword(Token.Value, Snapshot))
        {    // dedent after some statements
          Current.ShouldDedentAfter = true;
        }

        if (IsColon(Token.Value, Snapshot) &&       // indent after a colon
            IndentStack.Count == 0)
        {           // except in a grouping
          Current.ShouldIndentAfter = true;
          // If the colon isn't at the end of the line, cancel it out.
          // If the following is a ShouldDedentAfterKeyword, only one dedent will occur.
          Current.ShouldDedentAfter = (TokenStack.Count != 0 && TokenStack.Peek() != null);
        }
      }

      Indentation = Current.Indentation +
          (Current.ShouldIndentAfter ? TabSize : 0) -
          (Current.ShouldDedentAfter ? TabSize : 0);
      */

      return 0;
    }
    /*
    private static bool IsOpenGrouping(TrackingTokenInfo token, ITextSnapshot snapshot)
    {
      if (token.Category != TokenCategory.Grouping)
        return false;
      
      var span = token.ToSnapshotSpan(snapshot);
      return span.Length == 1 && "([{".Contains(span.GetText());
    }

    private static bool IsCloseGrouping(
        TrackingTokenInfo _Token, 
        ITextSnapshot     _Snapshot
      )
    {
      if (_Token.Category != TokenCategory.Grouping)
        return false;
      
      var Span = _Token.ToSnapshotSpan(_Snapshot);
      
      return Span.Length == 1 && ")]}".Contains(Span.GetText());
    }

    private static bool IsUnterminatedStringToken(
        TrackingTokenInfo _Token, 
        ITextSnapshot     _Snapshot
      )
    {
      if (_Token.Category == TokenCategory.IncompleteMultiLineStringLiteral)
        return true;

      if (_Token.Category != TokenCategory.StringLiteral)
        return false;
      
      try
      {
        var Text = _Token.GetText(_Snapshot);

        return Text == "\"" || Text == "'";
      }
      catch (ArgumentException)
      {
        return false;
      }
    }

    private static bool ShouldDedentAfterKeyword(
        TrackingTokenInfo _Token, 
        ITextSnapshot     _Snapshot
      )
    {
      if (_Token.Category != TokenCategory.Keyword)
        return false;
      
      var keyword = _Token.GetText(_Snapshot);

      return keyword == "pass" || keyword == "return" || keyword == "break" || keyword == "continue" || keyword == "raise";
    }

    private static bool IsColon(
        TrackingTokenInfo _Token, 
        ITextSnapshot     _Snapshot
      )
    {
      if (_Token.Category != TokenCategory.Delimiter)
        return false;
      
      var Span = _Token.ToSnapshotSpan(_Snapshot);
      
      return Span.Length == 1 && Span.GetText() == ":";
    }

    private static bool IsWhitespace(TokenCategory category)
    {
      return category == TokenCategory.Comment ||
          category == TokenCategory.DocComment ||
          category == TokenCategory.LineComment ||
          category == TokenCategory.WhiteSpace;
    }

    private static bool IsExplicitLineJoin(
        TrackingTokenInfo _Token, 
        ITextSnapshot     _Snapshot
      )
    {
      if (_Token.Category != TokenCategory.Operator)
        return false;
      
      var Token = _Token.GetText(_Snapshot);

      return Token == "\\" || Token.TrimEnd('\r', '\n') == "\\";
    }

    private static bool HasExplicitLineJoin(
        IReadOnlyList<TrackingTokenInfo> _Tokens, 
        ITextSnapshot                    _Snapshot
      )
    {
      foreach (var t in _Tokens.Reverse())
      {
        if (IsExplicitLineJoin(t, _Snapshot))
          return true;
        
        if (t.Category != TokenCategory.WhiteSpace)
          return false;
      }
      return false;
    }
    */
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
