using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Tvl.VisualStudio.Text.Tagging;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(IViewTaggerProvider))]
  [TagType(typeof(TextMarkerTag))]
  [ContentType("Squirrel")]
  public sealed class BraceMatchingTaggerProvider : IViewTaggerProvider
  {
    [Import]
    private IClassifierAggregatorService AggregatorService
    {
      get;
      set;
    }

    public ITagger<T> CreateTagger<T>(
        ITextView   _TextView,
        ITextBuffer _Buffer
      ) where T : ITag
    {
      if (_TextView == null)
        return null;

      var Aggregator = AggregatorService.GetClassifier(_Buffer);
      var Pairs      = new KeyValuePair<char, char>[]
      {
         new KeyValuePair<char, char>('(', ')'),
         new KeyValuePair<char, char>('{', '}'),
         new KeyValuePair<char, char>('[', ']'),
         new KeyValuePair<char, char>('<', '>')
       };
      
      return new BraceMatchingTagger(_TextView, _Buffer, Aggregator, Pairs) as ITagger<T>;
    }
  }
}
