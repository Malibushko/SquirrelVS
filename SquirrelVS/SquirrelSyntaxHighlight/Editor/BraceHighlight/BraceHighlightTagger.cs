using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using SquirrelSyntaxHighlight.Common;
using SquirrelSyntaxHighlight.Editor.Common;
using SquirrelSyntaxHighlight.Parsing;

namespace SquirrelSyntaxHighlight.Editor.BraceHighlight
{
  internal class BraceHighlightTagger : ITagger<TextMarkerTag>, IDisposable
  {
    private readonly IServiceProvider Site;
    private readonly ITextView        TextView;
    private readonly ITextBuffer      Buffer;
    private readonly DisposableBag    DisposableBag;
    private SnapshotPoint?            CurrentChar;

    private static readonly TextMarkerTag Tag = new TextMarkerTag("Brace Matching (Rectangle)");

    public BraceHighlightTagger(
        IServiceProvider _Site, 
        ITextView        _TextView, 
        ITextBuffer      _Buffer
      )
    {
      Site     = _Site     ?? throw new ArgumentNullException(nameof(_Site));
      TextView = _TextView ?? throw new ArgumentNullException(nameof(_TextView));
      Buffer   = _Buffer   ?? throw new ArgumentNullException(nameof(_Buffer));

      CurrentChar = null;

      TextView.Caret.PositionChanged += CaretPositionChanged;
      TextView.LayoutChanged         += ViewLayoutChanged;

      DisposableBag = new DisposableBag(GetType().Name);
      DisposableBag.Add(() =>
      {
        TextView.Caret.PositionChanged -= CaretPositionChanged;
        TextView.LayoutChanged         -= ViewLayoutChanged;
      });
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public void Dispose()
    {
      DisposableBag.TryDispose();
    }

    public IEnumerable<ITagSpan<TextMarkerTag>> GetTags(
        NormalizedSnapshotSpanCollection _Spans
      )
    {
      if (_Spans.Count == 0)
        yield break;

      if (!CurrentChar.HasValue || CurrentChar.Value.Position > CurrentChar.Value.Snapshot.Length)
        yield break;

      // Hold on to a snapshot of the current character
      SnapshotPoint CurrentPoint = CurrentChar.Value;

      // If the requested snapshot isn't the same as the one the brace is on, translate our spans to the expected snapshot
      if (_Spans[0].Snapshot != CurrentPoint.Snapshot)
        CurrentPoint = CurrentPoint.TranslateTo(_Spans[0].Snapshot, PointTrackingMode.Positive);

      // Look before current position for an opening brace
      if (CurrentPoint != 0)
      {
        var PreviousCharacterText = TextView.TextBuffer.CurrentSnapshot.GetText(CurrentPoint.Position - 1, 1);

        if (PreviousCharacterText == ")" || PreviousCharacterText == "]" || PreviousCharacterText == "}")
        {
          if (FindMatchingPair(GetBraceKind(PreviousCharacterText), CurrentPoint, -1, out var LeftSpan, out var RightSpan))
          {
            yield return new TagSpan<TextMarkerTag>(LeftSpan, Tag);
            yield return new TagSpan<TextMarkerTag>(RightSpan, Tag);
            yield break;
          }
        }
      }

      // Look after current position for a closing brace
      if (CurrentPoint != TextView.TextBuffer.CurrentSnapshot.Length)
      {
        var NextCharText = TextView.TextBuffer.CurrentSnapshot.GetText(CurrentPoint.Position, 1);
        
        if (NextCharText == "(" || NextCharText == "[" || NextCharText == "{")
        {
          if (FindMatchingPair(GetBraceKind(NextCharText), CurrentPoint + 1, 1, out var leftSpan, out var rightSpan))
          {
            yield return new TagSpan<TextMarkerTag>(leftSpan, Tag);
            yield return new TagSpan<TextMarkerTag>(rightSpan, Tag);
            yield break;
          }
        }
      }
    }

    private void ViewLayoutChanged(
        object                         _Sender, 
        TextViewLayoutChangedEventArgs _Args
      )
    {
      if (_Args.NewSnapshot != _Args.OldSnapshot)
        UpdateAtCaretPosition(TextView.Caret.Position);
    }

    private void CaretPositionChanged(
        object                        _Sender, 
        CaretPositionChangedEventArgs _Args
      )
    {
      UpdateAtCaretPosition(_Args.NewPosition);
    }

    private void UpdateAtCaretPosition(
        CaretPosition _CaretPosition
      )
    {
      CurrentChar = _CaretPosition.Point.GetPoint(Buffer, _CaretPosition.Affinity);

      if (!CurrentChar.HasValue)
        return;

      TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(new SnapshotSpan(Buffer.CurrentSnapshot, 0, Buffer.CurrentSnapshot.Length)));
    }

    private enum BraceKind
    {
      None,
      Bracket,
      Paren,
      Brace
    }

    private bool FindMatchingPair(
        BraceKind        _Brace, 
        SnapshotPoint    _Position, 
        int              _Direction, 
        out SnapshotSpan _LeftSpan, 
        out SnapshotSpan _RightSpan
      )
    {
      _LeftSpan  = new SnapshotSpan(_Position, _Position);
      _RightSpan = _LeftSpan;

      var Buffer = SquirrelTextBufferInfo.ForBuffer(Site, _Position.Snapshot.TextBuffer);
      
      if (Buffer == null)
        return false;

      var Snapshot = _Position.Snapshot;
      int Depth    = 0;

      for (int i = _Position.Position - 1; i != (_Direction > 0 ? _Position.Snapshot.Length : 0); i += (_Direction > 0 ? 1 : -1))
      {
        var Kind = GetBraceKind(_Position.Snapshot[i]);
        
        if (Kind == BraceKind.None)
          continue;

        if (Kind == _Brace)
        {
          if (_Position.Snapshot[i].IsCloseGrouping())
            Depth -= _Direction;
          else
            Depth += _Direction;
        }

        if (Depth == 0)
        {
          _LeftSpan  = new SnapshotSpan(Snapshot, i, 1);
          _RightSpan = new SnapshotSpan(Snapshot, _Position - 1, 1);

          return true;
        }
      }

      return false;
    }

    private static BraceKind GetBraceKind(
        string _Brace
      )
    {
      if (string.IsNullOrEmpty(_Brace))
        return BraceKind.None;

      return GetBraceKind(_Brace[0]);
    }

    private static BraceKind GetBraceKind(
        char _Brace
      )
    {
      switch (_Brace)
      {
        case '[':
        case ']':
          return BraceKind.Bracket;
        case '(':
        case ')':
          return BraceKind.Paren;
        case '{':
        case '}':
          return BraceKind.Bracket;
        default:
          return BraceKind.None;
      }
    }
  }
}
