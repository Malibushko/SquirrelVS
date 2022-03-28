using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Text.Editor;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(IAsyncCompletionSourceProvider))]
  [ContentType("Squirrel")]
  [Name("Token Completion")]
  internal class CompletionSourceProvider : IAsyncCompletionSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    [Import(typeof(ICodeDatabaseService))]
    internal CodeDatabaseService CodeDatabaseService;

    public IAsyncCompletionSource GetOrCreate(
        ITextView _TextView
      )
    {
      return new CompletionSource(this, NavigatorService, _TextView, CodeDatabaseService);
    }
  }
}
