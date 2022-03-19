using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor.Indent
{
  [Export(typeof(ISmartIndentProvider))]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal class SmartIndentProvider : ISmartIndentProvider
  {
    private readonly IServiceProvider Site;

    [ImportingConstructor]
    internal SmartIndentProvider(
        [Import(typeof(SVsServiceProvider))] IServiceProvider _ServiceProvider
      )
    {
      Site = _ServiceProvider;
    }

    public ISmartIndent CreateSmartIndent(
        ITextView _TextView
      )
    {
      return new SmartIndent(Site, _TextView);
    }
  }
}
