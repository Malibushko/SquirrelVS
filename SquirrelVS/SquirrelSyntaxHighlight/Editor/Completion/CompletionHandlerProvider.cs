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
  [Name("Token Completion Handler")]
  [ContentType("Squirrel")]
  [TextViewRole(PredefinedTextViewRoles.Editable)]
  internal class CompletionHandlerProvider : IVsTextViewCreationListener
  {
    [Import]
    internal IVsEditorAdaptersFactoryService AdapterService = null;

    [Import]
    internal ICompletionBroker CompletionBroker { get; set; }

    [Import]
    internal SVsServiceProvider ServiceProvider { get; set; }

    public void VsTextViewCreated(IVsTextView _TextViewAdapter)
    {
      ITextView TextView = AdapterService.GetWpfTextView(_TextViewAdapter);

      if (TextView == null)
        return;

      Func<CompletionCommandHandler> CreateCommandHandler = delegate () 
      { 
        return new CompletionCommandHandler(_TextViewAdapter, TextView, this);
      };
      
      TextView.Properties.GetOrCreateSingletonProperty(CreateCommandHandler);
    }
  }
}
