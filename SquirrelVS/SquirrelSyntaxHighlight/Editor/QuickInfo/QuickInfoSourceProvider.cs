using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;

namespace SquirrelSyntaxHighlight.Editor.QuickInfo
{
  [Export(typeof(IQuickInfoSourceProvider))]
  [Name("ToolTip QuickInfo Source")]
  [Order(Before = "Default Quick Info Presenter")]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal class QuickInfoSourceProvider : IQuickInfoSourceProvider
  {
    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    [Import]
    internal ITextBufferFactoryService TextBufferFactoryService { get; set; }

    [Import(typeof(ICodeDatabaseService))]
    internal CodeDatabaseService CodeDatabaseService { get; set; }

    public IQuickInfoSource TryCreateQuickInfoSource(
        ITextBuffer _TextBuffer
      )
    {
      return new QuickInfoSource(this, _TextBuffer, CodeDatabaseService);
    }
  }
}
