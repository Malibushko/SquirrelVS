using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;

namespace SquirrelSyntaxHighlight.Editor
{
  internal class CompletionCommandHandler : IOleCommandTarget
  {
    private IOleCommandTarget         NextCommandHandler;
    private ITextView                 TextView;

    private CompletionHandlerProvider CompletionProvider;
    private ICompletionSession        CompletionSession;

    internal CompletionCommandHandler(
        IVsTextView               _TextViewAdapter, 
        ITextView                 _TextView, 
        CompletionHandlerProvider _Provider
      )
    {
      TextView           = _TextView;
      CompletionProvider = _Provider;

      _TextViewAdapter.AddCommandFilter(this, out NextCommandHandler);
    }

    public int QueryStatus(
        ref Guid pguidCmdGroup, 
        uint     cCmds, 
        OLECMD[] prgCmds, 
        IntPtr   pCmdText
      )
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      return NextCommandHandler.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
    }

    public int Exec(
        ref Guid pguidCmdGroup, 
        uint     nCmdID, 
        uint     nCmdexecopt, 
        IntPtr   pvaIn, 
        IntPtr   pvaOut
      )
    {
      if (VsShellUtilities.IsInAutomationFunction(CompletionProvider.ServiceProvider))
        return NextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      
      uint CommandID = nCmdID;
      char TypeChar  = char.MinValue;
      
      if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
        TypeChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
      
      if (nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN || 
          nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB)
      {
        if (CompletionSession != null && !CompletionSession.IsDismissed)
        {
          if (CompletionSession.SelectedCompletionSet.SelectionStatus.IsSelected)
          {
            CompletionSession.Commit();
           
            return VSConstants.S_OK;
          }
          else
          {
            CompletionSession.Dismiss();
          }
        }
      }

      int  ExecResult = NextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
      bool IsHandled  = false;

      if (!TypeChar.Equals(char.MinValue) && char.IsLetterOrDigit(TypeChar))
      {
        if (CompletionSession == null || CompletionSession.IsDismissed)
        {
          TriggerCompletion();

          CompletionSession.Filter();
        }
        else
        {
          CompletionSession.Filter();
        }
        IsHandled = true;
      }
      else 
      if (CommandID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE || CommandID == (uint)VSConstants.VSStd2KCmdID.DELETE)
      {
        if (CompletionSession != null && !CompletionSession.IsDismissed)
          CompletionSession.Filter();

        IsHandled = true;
      }
      
      if (IsHandled) 
        return VSConstants.S_OK;
      
      return ExecResult;
    }

    private bool TriggerCompletion()
    {
      //the caret must be in a non-projection location 
      SnapshotPoint? caretPoint =
      TextView.Caret.Position.Point.GetPoint(
      textBuffer => (!textBuffer.ContentType.IsOfType("projection")), PositionAffinity.Predecessor);
      if (!caretPoint.HasValue)
      {
        return false;
      }

      CompletionSession = CompletionProvider.CompletionBroker.CreateCompletionSession
          (TextView,
          caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
          true);

      //subscribe to the Dismissed event on the session 
      CompletionSession.Dismissed += this.OnSessionDismissed;
      CompletionSession.Start();

      return true;
    }

    private void OnSessionDismissed(object sender, EventArgs e)
    {
      CompletionSession.Dismissed -= this.OnSessionDismissed;
      CompletionSession = null;
    }
  }
}
