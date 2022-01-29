using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(IViewTaggerProvider))]
  [ContentType("Squirrel")]
  [TagType(typeof(TextMarkerTag))]
  internal class BraceHighlightTaggerProvider : IViewTaggerProvider
  {
    private readonly IServiceProvider Site;

    [ImportingConstructor]
    public BraceHighlightTaggerProvider(
        [Import(typeof(SVsServiceProvider))] IServiceProvider _Site
      )
    {
      Site = _Site;
    }

    public ITagger<T> CreateTagger<T>(
        ITextView   _TextView, 
        ITextBuffer _Buffer
      ) where T : ITag
    {
      if (_TextView == null)
        return null;

      if (_TextView.TextBuffer != _Buffer)
        return null;

      return new BraceHighlightTagger(_TextView, _Buffer) as ITagger<T>;
    }
  }
}
