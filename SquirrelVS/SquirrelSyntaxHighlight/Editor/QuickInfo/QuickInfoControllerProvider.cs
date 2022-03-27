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

namespace SquirrelSyntaxHighlight.Editor.QuickInfo
{
  [Export(typeof(IIntellisenseControllerProvider))]
  [Name("ToolTip QuickInfo Controller")]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
  {
    [Import]
    internal IQuickInfoBroker QuickInfoBroker { get; set; }

    public IIntellisenseController TryCreateIntellisenseController(
        ITextView          _TextView, 
        IList<ITextBuffer> _SubjectBuffers
      )
    {
      return new QuickInfoController(_TextView, _SubjectBuffers, this);
    }
  }
}
