using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using SquirrelSyntaxHighlight.Common;
using SquirrelSyntaxHighlight.Editor.Common;
using SquirrelSyntaxHighlight.Parsing;
using SquirrelSyntaxHighlight.Infrastructure.Syntax;
using tree_sitter;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Editor.Errors
{
  internal class DiagnosticsSquiggleTagger : ITagger<ErrorTag>, ISquirrelTextBufferInfoEventSink
  {
    private readonly object           Key = new object();
    private readonly IServiceProvider Site;
    private readonly ITextBuffer      Buffer;


    public DiagnosticsSquiggleTagger(
        IServiceProvider _Site,
        ITextBuffer      _Buffer
      )
    {
      Site   = _Site;
      Buffer = _Buffer;

      SquirrelTextBufferInfo.ForBuffer(Site, Buffer).AddSink(Key, this);
    }

    public IEnumerable<ITagSpan<ErrorTag>> GetTags(
        NormalizedSnapshotSpanCollection _Spans
      )
    {
      SquirrelTextBufferInfo Info = SquirrelTextBufferInfo.ForBuffer(Site, Buffer);

      if (Info == null)
        throw new Exception("Failed to get/create buffer");

      foreach (var Span in _Spans)
      {
        int SnapStartPosition = Span.Start.Position;

        for (; SnapStartPosition < Span.End.Position; ++SnapStartPosition)
        {
          if (!char.IsWhiteSpace(Span.Snapshot[SnapStartPosition]))
            break;
        }

        int SnapEndPosition = Span.End.Position;

        for (; SnapEndPosition < Span.Start.Position; --SnapEndPosition)
        {
          if (!char.IsWhiteSpace(Span.Snapshot[SnapEndPosition]))
            break;
        }

        if (SnapStartPosition == SnapEndPosition)
          continue;

        TSNode       Root   = Info.GetNodeAt(new Span(SnapStartPosition, SnapEndPosition - SnapStartPosition));
        TSTreeCursor Walker = api.TsTreeCursorNew(Root);

        foreach (TSNode Node in SyntaxTreeWalker.Traverse(Walker))
        {
          if (api.TsNodeHasError(Node) && api.TsNodeType(Node) == "ERROR")
          {
            int Start = (int)api.TsNodeStartByte(Node);
            int End   = (int)api.TsNodeEndByte(Node);

            if (End - Start <= 0)
              continue;
            
            yield return new TagSpan<ErrorTag>(new SnapshotSpan(Span.Snapshot, new Span(Start, End - Start)), new ErrorTag("Error", "Parsing error"));
          }
        }
      }
    }

    public Task SquirrelTextBufferEventAsync(
        SquirrelTextBufferInfo          _Sender, 
        SquirrelTextBufferInfoEventArgs _Args
      )
    {
      return Task.Run(() =>
      {
        switch (_Args.Event)
        {
          case SquirrelTextBufferInfoEvents.ParseTreeChanged:
          {
            var Args = _Args as SquirrelTreeChangedArgs;

            foreach (var Span in Args.ChangedSpans)
              TagsChanged(this, new SnapshotSpanEventArgs(new SnapshotSpan(Buffer.CurrentSnapshot, Span)));

            break;
          }
        }
      });
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
  }
}
