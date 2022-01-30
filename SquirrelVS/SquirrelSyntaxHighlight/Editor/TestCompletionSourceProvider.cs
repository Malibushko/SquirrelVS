using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType("Squirrel")]
  [Name("token completion")]
  internal class TestCompletionSourceProvider : ICompletionSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
    {
      return new TestCompletionSource(this, textBuffer);
    }
  }
}
