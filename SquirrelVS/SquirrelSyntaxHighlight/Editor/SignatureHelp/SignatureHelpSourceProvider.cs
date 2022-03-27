using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Language.Intellisense;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;

namespace SquirrelSyntaxHighlight.Editor.SignatureHelp
{
  [Export(typeof(ISignatureHelpSourceProvider))]
  [Name("Signature Help source")]
  [Order(Before = "default")]
  [ContentType(SquirrelConstants.SquirrelContentType)]
  internal class SignatureHelpSourceProvider : ISignatureHelpSourceProvider
  {
    [Import(typeof(ICodeDatabaseService))]
    internal CodeDatabaseService CodeDatabase { get; set; }

    public ISignatureHelpSource TryCreateSignatureHelpSource(
        ITextBuffer _TextBuffer
      )
    {
      return new SignatureHelpSource(_TextBuffer, CodeDatabase);
    }
  }
}
