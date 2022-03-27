using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TextManager.Interop;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;

namespace SquirrelSyntaxHighlight.Editor.SignatureHelp
{
  [Export(typeof(IVsTextViewCreationListener))]
  [Name("Signature Help controller")]
  [TextViewRole(PredefinedTextViewRoles.Editable)]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal class SignatureHelpCommandProvider : IVsTextViewCreationListener
  {
    [Import]
    internal IVsEditorAdaptersFactoryService        AdapterService;

    [Import]
    internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

    [Import]
    internal ISignatureHelpBroker                   SignatureHelpBroker;

    [Import(typeof(ICodeDatabaseService))]
    internal CodeDatabaseService                    CodeDatabaseService;

    public void VsTextViewCreated(
        IVsTextView _TextViewAdapter
      )
    {
      ITextView TextView = AdapterService.GetWpfTextView(_TextViewAdapter);
      
      if (TextView == null)
        return;

      TextView.Properties.GetOrCreateSingletonProperty(
           () => new SignatureHelpCommandHandler(_TextViewAdapter,
              TextView,
              NavigatorService.GetTextStructureNavigator(TextView.TextBuffer),
              SignatureHelpBroker,
              CodeDatabaseService));
    }
  }
}
