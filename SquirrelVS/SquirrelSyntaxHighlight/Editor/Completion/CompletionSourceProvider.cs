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

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(ICompletionSourceProvider))]
  [ContentType("Squirrel")]
  [Name("Token Completion")]
  internal class CompletionSourceProvider : ICompletionSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    [Import(typeof(ICodeDatabaseService))]
    internal CodeDatabaseService CodeDatabaseService;

    public ICompletionSource TryCreateCompletionSource(
        ITextBuffer _TextBuffer
      )
    {
      return new CompletionSource(this, _TextBuffer, CodeDatabaseService);
    }
  }
}
