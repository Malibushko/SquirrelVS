using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace SquirrelSyntaxHighlight.Common
{
  static class EditorExtensions
  {
    /// <summary>
    /// Maps down to the buffer using positive point tracking and successor position affinity
    /// </summary>
    public static SnapshotPoint? MapDownToBuffer(
        this ITextView _TextBuffer, 
        int            _Position, 
        ITextBuffer    _Buffer
      )
    {
      if (_TextBuffer.BufferGraph == null)
      {
        if (_Position <= _Buffer.CurrentSnapshot.Length)
         return new SnapshotPoint(_Buffer.CurrentSnapshot, _Position);
       
        return null;
      }

      if (_Position <= _TextBuffer.TextBuffer.CurrentSnapshot.Length)
      {
        return _TextBuffer.BufferGraph.MapDownToBuffer(
            new SnapshotPoint(_TextBuffer.TextBuffer.CurrentSnapshot, _Position),
            PointTrackingMode.Positive,
            _Buffer,
            PositionAffinity.Successor
          );
      }

      return null;
    }

    public static SnapshotSpan Trim(
        this SnapshotSpan _Snapshot
      )
    {
      try
      {
        int SnapStartPosition = _Snapshot.Start.Position;

        for (; SnapStartPosition < _Snapshot.End.Position; ++SnapStartPosition)
        {
          if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapStartPosition]))
            break;
        }

        int SnapEndPosition = _Snapshot.End.Position - 1;

        for (; SnapEndPosition != _Snapshot.Start.Position; --SnapEndPosition)
        {
          if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapEndPosition]))
            break;
        }

        return new SnapshotSpan(_Snapshot.Snapshot, new Span(SnapStartPosition, SnapEndPosition - SnapStartPosition));
      }
      catch (Exception)
      {
        return _Snapshot;
      }
    }
  }
}
