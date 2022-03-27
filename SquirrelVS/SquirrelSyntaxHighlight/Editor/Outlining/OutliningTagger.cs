using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SquirrelSyntaxHighlight.Queries;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Editor.Outlining
{
  internal class OutliningTagger : ITagger<IOutliningRegionTag>
  {
    private IServiceProvider Site;
    private ITextBuffer      TextBuffer;
    
    private SyntaxTreeQuery  BlockQuery;

    private string GetNodeText(
        TSNode _Node
      )
    {
      int Start = (int)api.TsNodeStartByte(_Node);
      int End   = (int)api.TsNodeEndByte(_Node);

      var Buffer = new char[End - Start + 1];

      TextBuffer.CurrentSnapshot.CopyTo(Start, Buffer, 0, End - Start);

      return new string(Buffer);
    }

    public OutliningTagger(
        IServiceProvider _Site,
        ITextBuffer      _TextBuffer
      )
    {
      Site       = _Site;
      TextBuffer = _TextBuffer;
    }

    public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(
        NormalizedSnapshotSpanCollection _Spans
      )
    {
      var BufferInfo = SquirrelTextBufferInfo.ForBuffer(Site, TextBuffer);

      if (BufferInfo == null)
        yield return null;

      foreach (var Span in _Spans)
      {
        foreach (Tuple<string, Span> Node in BufferInfo.ExecuteQueryFromFile(BufferInfo.GetNodeAt(Span), SyntaxTreeQueries.BLOCKS_QUERY))
        {
          int StartLine = TextBuffer.CurrentSnapshot.GetLineNumberFromPosition(Node.Item2.Start);
          int EndLine   = TextBuffer.CurrentSnapshot.GetLineNumberFromPosition(Node.Item2.End);

          if (StartLine == EndLine)
            continue;

          yield return new TagSpan<IOutliningRegionTag>(
                 new SnapshotSpan(Span.Snapshot, Node.Item2),
                 new OutliningRegionTag(false, false, "...", "TODO: add description to hovered blocks"));
        }
      }
    }
    
    #region Events
    
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    #endregion
  }
}
