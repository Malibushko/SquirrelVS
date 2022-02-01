using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace SquirrelSyntaxHighlight.Editor
{
  internal class BraceHighlightTagger : ITagger<TextMarkerTag>
  {
    private static TextMarkerTag Tag = new TextMarkerTag("Brace Matching (Rectangle)");
    private ITextView      View { get; set; }
    private ITextBuffer    SourceBuffer { get; set; }
    private SnapshotPoint? CurrentChar { get; set; }
    
    private Dictionary<char, char> BraceList;

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public BraceHighlightTagger(
        ITextView   _View,
        ITextBuffer _Buffer
      )
    {
      BraceList = new Dictionary<char, char>();

      BraceList.Add('{', '}');
      BraceList.Add('[', ']');
      BraceList.Add('(', ')');

      View         = _View;
      SourceBuffer = _Buffer;
      CurrentChar  = null;

      View.Caret.PositionChanged += CaretPositionChanged;
      View.LayoutChanged         += ViewLayoutChanged;
    }

    void ViewLayoutChanged(
        object                         _Sender, 
        TextViewLayoutChangedEventArgs _Args
      )
    {
      if (_Args.NewSnapshot != _Args.OldSnapshot)
        UpdateAtCaretPosition(View.Caret.Position);
    }

    void CaretPositionChanged(
        object                        _Sender, 
        CaretPositionChangedEventArgs _Args
      )
    {
      UpdateAtCaretPosition(_Args.NewPosition);
    }

    void UpdateAtCaretPosition(
        CaretPosition _CaretPosition
      )
    {
      CurrentChar = _CaretPosition.Point.GetPoint(SourceBuffer, _CaretPosition.Affinity);

      if (!CurrentChar.HasValue)
        return;

      var TempEvent = TagsChanged;

      if (TempEvent != null)
      {
        TempEvent(this, new SnapshotSpanEventArgs(new SnapshotSpan(SourceBuffer.CurrentSnapshot, 0,
            SourceBuffer.CurrentSnapshot.Length)));
      }
    }

    public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(
        NormalizedSnapshotSpanCollection _Spans
      )
    {
      if (_Spans.Count == 0)
        yield break;

      if (!CurrentChar.HasValue || CurrentChar.Value.Position >= CurrentChar.Value.Snapshot.Length)
        yield break;

      SnapshotPoint Char = CurrentChar.Value;

      if (_Spans[0].Snapshot != Char.Snapshot)
        Char = Char.TranslateTo(_Spans[0].Snapshot, PointTrackingMode.Positive);

      char          CurrentText = Char.GetChar();
      SnapshotPoint LastChar    = Char == 0 ? Char : Char - 1;
      char          LastText    = LastChar.GetChar();
      SnapshotSpan  PairSpan    = new SnapshotSpan();

      if (BraceList.ContainsKey(CurrentText))
      {
        BraceList.TryGetValue(CurrentText, out char _CloseChar);

        if (FindMatchingCloseChar(Char, CurrentText, _CloseChar, View.TextSnapshot.LineCount, out PairSpan) == true)
        {
          yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(Char, 1), Tag);
          yield return new TagSpan<TextMarkerTag>(PairSpan, Tag);
        }
      }
      else if (BraceList.ContainsValue(LastText))
      {
        var Open = from n in BraceList
                   where n.Value.Equals(LastText)
                   select n.Key;

        if (FindMatchingOpenChar(LastChar, (char)Open.ElementAt<char>(0), LastText, View.TextSnapshot.LineCount, out PairSpan) == true)
        {
          yield return new TagSpan<TextMarkerTag>(new SnapshotSpan(LastChar, 1), Tag);
          yield return new TagSpan<TextMarkerTag>(PairSpan, Tag);
        }
      }
    }

    private static bool FindMatchingCloseChar(
        SnapshotPoint    _StartPoint, 
        char             _Open, 
        char             _Close, 
        int              _MaxLines, 
        out SnapshotSpan _PairSpan
      )
    {
      _PairSpan                    = new SnapshotSpan(_StartPoint.Snapshot, 1, 1);

      ITextSnapshotLine Line       = _StartPoint.GetContainingLine();
      string            LineText   = Line.GetText();
      int               LineNumber = Line.LineNumber;
      int               Offset     = _StartPoint.Position - Line.Start.Position + 1;

      int StopLineNumber = _StartPoint.Snapshot.LineCount - 1;

      if (_MaxLines > 0)
        StopLineNumber = Math.Min(StopLineNumber, LineNumber + _MaxLines);

      int OpenCount = 0;

      while (true)
      {
        while (Offset < Line.Length)
        {
          char CurrentChar = LineText[Offset];

          if (CurrentChar == _Close)
          {
            if (OpenCount > 0)
            {
              OpenCount--;
            }
            else
            {
              _PairSpan = new SnapshotSpan(_StartPoint.Snapshot, Line.Start + Offset, 1);
              return true;
            }
          }
          else if (CurrentChar == _Open)
          {
            OpenCount++;
          }
          Offset++;
        }

        if (++LineNumber > StopLineNumber)
          break;

        Line     = Line.Snapshot.GetLineFromLineNumber(LineNumber);
        LineText = Line.GetText();
        Offset   = 0;
      }

      return false;
    }

    private static bool FindMatchingOpenChar(
        SnapshotPoint    _StartPoint, 
        char             _Open, 
        char             _Close, 
        int              _MaxLines, 
        out SnapshotSpan _PairSpan
      )
    {
      _PairSpan = new SnapshotSpan(_StartPoint, _StartPoint);

      ITextSnapshotLine Line = _StartPoint.GetContainingLine();

      int LineNumber = Line.LineNumber;
      int Offset     = _StartPoint - Line.Start - 1; 

      if (Offset < 0)
      {
        Line   = Line.Snapshot.GetLineFromLineNumber(--LineNumber);
        Offset = Line.Length - 1;
      }

      string LineText = Line.GetText();

      int StopLineNumber = 0;

      if (_MaxLines > 0)
        StopLineNumber = Math.Max(StopLineNumber, LineNumber - _MaxLines);

      int CloseCount = 0;

      while (true)
      {
        while (Offset >= 0)
        {
          char CurrentChar = LineText[Offset];

          if (CurrentChar == _Open)
          {
            if (CloseCount > 0)
            {
              CloseCount--;
            }
            else
            {
              _PairSpan = new SnapshotSpan(Line.Start + Offset, 1);

              return true;
            }
          }
          else if (CurrentChar == _Close)
          {
            CloseCount++;
          }
          Offset--;
        }

        if (--LineNumber < StopLineNumber)
          break;

        Line     = Line.Snapshot.GetLineFromLineNumber(LineNumber);
        LineText = Line.GetText();
        Offset   = Line.Length - 1;
      }

      return false;
    }
  }
}
