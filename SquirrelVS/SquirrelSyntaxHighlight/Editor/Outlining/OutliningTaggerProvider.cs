using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.Outlining
{
  //[Export(typeof(ITaggerProvider))]
  [TagType(typeof(IOutliningRegionTag))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal sealed class OutliningTaggerProvider : ITaggerProvider
  {
    [Import(typeof(SVsServiceProvider))]
    private readonly IServiceProvider _ServiceProvider;

    public ITagger<T> CreateTagger<T>(
        ITextBuffer _Buffer
      ) where T : ITag
    {
      if (_Buffer == null)
        return null;

      return new OutliningTagger(_ServiceProvider, _Buffer) as ITagger<T>;
    }
  }
}
