using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor
{
  [Export(typeof(IVsTextViewCreationListener))]
  [Name("token completion handler")]
  [ContentType("Squirrel")]
  [TextViewRole(PredefinedTextViewRoles.Editable)]
  internal class TestCompletionHandlerProvider : IVsTextViewCreationListener
  {
    [Import]
    internal IVsEditorAdaptersFactoryService AdapterService = null;
    [Import]
    internal ICompletionBroker CompletionBroker { get; set; }
    [Import]
    internal SVsServiceProvider ServiceProvider { get; set; }

    public void VsTextViewCreated(IVsTextView textViewAdapter)
    {
      ITextView textView = AdapterService.GetWpfTextView(textViewAdapter);
      if (textView == null)
        return;

      Func<TestCompletionCommandHandler> createCommandHandler = delegate () { return new TestCompletionCommandHandler(textViewAdapter, textView, this); };
      textView.Properties.GetOrCreateSingletonProperty(createCommandHandler);
    }
  }
}
