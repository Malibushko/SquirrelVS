using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Editor.Outlining
{
  internal class OutliningTagger : ITagger<IOutliningRegionTag>
  {
    private static SortedSet<string> ExpandableNodeTypes = new SortedSet<string>
    {
      "table_expression",
      "switch_body",
      "statement_block",
      "enum_statement_block",
      "class_body",
      "block_statement"
    };

    private IServiceProvider Site;
    private ITextBuffer      TextBuffer;
    
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
        ITextBuffer _TextBuffer
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
        foreach (Tuple<TSNode, string> Node in BufferInfo.GetNodeWithSymbols(ExpandableNodeTypes, Span))
        {
          int Start = (int)api.TsNodeStartByte(Node.Item1);
          int End   = (int)api.TsNodeEndByte(Node.Item1);

          TSNode Parent = api.TsNodeParent(Node.Item1);

          string HoverText = Parent == null ? GetNodeText(Node.Item1) : GetNodeText(Parent);
          
          yield return new TagSpan<IOutliningRegionTag>(
                 new SnapshotSpan(Span.Snapshot, Start, End - Start),
                 new OutliningRegionTag(false, false, "...", HoverText));
        }
      }
    }
    
    #region Events
    
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    #endregion
  }
}
