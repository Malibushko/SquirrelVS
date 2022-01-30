using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(ITaggerProvider))]
  [TagType(typeof(IOutliningRegionTag))]
  [ContentType("Squirrel")]
  internal class OutliningTaggerProvider : ITaggerProvider
  {
    private readonly IServiceProvider Site;

    [ImportingConstructor]
    public OutliningTaggerProvider(
        [Import(typeof(SVsServiceProvider))] IServiceProvider _Site
      )
    {
      Site = _Site;
    }

    public ITagger<T> CreateTagger<T>(
        ITextBuffer _Buffer
      ) where T : ITag
    {
      return new OutliningTagger() as ITagger<T>;
    }
  }
}
