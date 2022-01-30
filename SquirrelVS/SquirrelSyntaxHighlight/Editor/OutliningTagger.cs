using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight.Editor
{
  internal class OutliningTagger : ITagger<OutliningRegionTag>
  {
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    public IEnumerable<ITagSpan<OutliningRegionTag>> GetTags(
        NormalizedSnapshotSpanCollection _Spans
      )
    {
      return null;
    }
  }
}
