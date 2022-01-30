using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight.Editor
{
  internal class CompletionSource : ICompletionSource
  {
    private CompletionSourceProvider SourceProvider;
    private ITextBuffer              TextBuffer;
    private List<Completion>         CompletionList;

    private bool                     IsDisposed;

    public CompletionSource(
        CompletionSourceProvider _SourceProvider, 
        ITextBuffer              _TextBuffer
      )
    {
      SourceProvider = _SourceProvider;
      TextBuffer     = _TextBuffer;
      CompletionList = new List<Completion>();

      foreach (var Token in SquirrelClasifier.NodeClassificator)
        CompletionList.Add(new Completion(Token.Key, Token.Key, Token.Key, null, null));
    }

    void ICompletionSource.AugmentCompletionSession(
        ICompletionSession   _Session, 
        IList<CompletionSet> _CompletionSets
      )
    {
      _CompletionSets.Add(new CompletionSet(
          "Suggested completions",
          "Suggested completions",
          FindTokenSpanAtPosition(_Session.GetTriggerPoint(TextBuffer), _Session),
          CompletionList,
          null)
        );
    }

    private ITrackingSpan FindTokenSpanAtPosition(
        ITrackingPoint     _Point, 
        ICompletionSession _Session
      )
    {
      SnapshotPoint           CurrentPoint = _Session.TextView.Caret.Position.BufferPosition - 1;
      ITextStructureNavigator Navigator    = SourceProvider.NavigatorService.GetTextStructureNavigator(TextBuffer);
      
      TextExtent Extent = Navigator.GetExtentOfWord(CurrentPoint);
      
      return CurrentPoint.Snapshot.CreateTrackingSpan(Extent.Span, SpanTrackingMode.EdgeInclusive);
    }

    public void Dispose()
    {
      if (!IsDisposed)
      {
        GC.SuppressFinalize(this);

        IsDisposed = true;
      }
    }
  }
}
