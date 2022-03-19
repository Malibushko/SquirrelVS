using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.BraceHighlight
{
  [Export(typeof(IViewTaggerProvider))]
  [TagType(typeof(TextMarkerTag))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  public class BraceHighlightTaggerProvider : IViewTaggerProvider
  {
    [Import(typeof(SVsServiceProvider))]
    private readonly IServiceProvider _ServiceProvider;

    public ITagger<T> CreateTagger<T>(
        ITextView   _View,
        ITextBuffer _Buffer
      ) where T : ITag
    {
      if (_View == null || _Buffer != _View.TextBuffer)
        return null;

      return new BraceHighlightTagger(_ServiceProvider, _View, _Buffer) as ITagger<T>;
    }
  }
}
