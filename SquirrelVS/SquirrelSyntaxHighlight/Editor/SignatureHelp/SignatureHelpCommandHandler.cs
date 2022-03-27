using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;

namespace SquirrelSyntaxHighlight.Editor.SignatureHelp
{
  internal class SignatureHelpCommandHandler : IOleCommandTarget
  {
    IOleCommandTarget       NextCommandHandler;
    ITextView               TextView;
    ISignatureHelpBroker    Broker;
    ISignatureHelpSession   Session;
    ITextStructureNavigator Navigator;
    CodeDatabaseService     CodeDatabaseService;

    internal SignatureHelpCommandHandler(
        IVsTextView             _TextViewAdapter, 
        ITextView               _TextView, 
        ITextStructureNavigator _Navigator, 
        ISignatureHelpBroker    _Broker,
        CodeDatabaseService     _CodeDatabaseService
      )
    {
      TextView  = _TextView;
      Broker    = _Broker;
      Navigator = _Navigator;
      
      CodeDatabaseService = _CodeDatabaseService;

      //add this to the filter chain
      _TextViewAdapter.AddCommandFilter(this, out NextCommandHandler);
    }

    public int Exec(
        ref Guid pguidCmdGroup, 
        uint     nCmdID, 
        uint     nCmdexecopt, 
        IntPtr   pvaIn, 
        IntPtr   pvaOut
      )
    {
      char TypedChar = char.MinValue;

      if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
      {
        TypedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

        if (TypedChar.Equals('('))
        {
          //move the point back so it's in the preceding word
          SnapshotPoint Point  = TextView.Caret.Position.BufferPosition - 1;
          TextExtent    Extent = Navigator.GetExtentOfWord(Point);
          string        Word   = Extent.Span.GetText();

          if (CodeDatabaseService.HasFunctionInfo(Word))
            Session = Broker.TriggerSignatureHelp(TextView);
        }
        else if (TypedChar.Equals(')') && Session != null)
        {
          Session.Dismiss();
          Session = null;
        }
      }

      return NextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
    }

    public int QueryStatus(
        ref Guid pguidCmdGroup, 
        uint     cCmds, 
        OLECMD[] prgCmds, 
        IntPtr   pCmdText
      )
    {
      return NextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }
  }
}
