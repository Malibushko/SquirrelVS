using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.Errors
{
  [Export(typeof(ITaggerProvider))]
  [TagType(typeof(IErrorTag))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal sealed class DiagnosticsSquiggleTaggerProvider : ITaggerProvider
  {
    [Import(typeof(SVsServiceProvider))]
    private readonly IServiceProvider _ServiceProvider;

    public ITagger<T> CreateTagger<T>(
        ITextBuffer _Buffer
      ) where T : ITag
    {
      if (_Buffer == null)
        return null;

      return new DiagnosticsSquiggleTagger(_ServiceProvider, _Buffer) as ITagger<T>;
    }
  }
}
