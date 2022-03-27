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
  internal class QuickInfoSource : IQuickInfoSource
  {
    private QuickInfoSourceProvider    Provider;
    private ITextBuffer                SubjectBuffer;
    private Dictionary<string, string> Dictionary;

    public QuickInfoSource(
        QuickInfoSourceProvider _Provider, 
        ITextBuffer             _SubjectBuffer
      )
    {
      Provider      = _Provider;
      SubjectBuffer = _SubjectBuffer;

      Dictionary = new Dictionary<string, string>();
      Dictionary.Add("add", "int add(int firstInt, int secondInt)\nAdds one integer to another.");
      Dictionary.Add("subtract", "int subtract(int firstInt, int secondInt)\nSubtracts one integer from another.");
      Dictionary.Add("multiply", "int multiply(int firstInt, int secondInt)\nMultiplies one integer by another.");
      Dictionary.Add("divide", "int divide(int firstInt, int secondInt)\nDivides one integer by another.");
    }

    public void AugmentQuickInfoSession(
        IQuickInfoSession _Session, 
        IList<object>     _QuickInfoContent, 
        out ITrackingSpan _ApplicableToSpan
      )
    {
      SnapshotPoint? SubjectTriggerPoint = _Session.GetTriggerPoint(SubjectBuffer.CurrentSnapshot);
      
      if (!SubjectTriggerPoint.HasValue)
      {
        _ApplicableToSpan = null;

        return;
      }

      ITextSnapshot CurrentSnapshot = SubjectTriggerPoint.Value.Snapshot;
      SnapshotSpan  QuerySpan       = new SnapshotSpan(SubjectTriggerPoint.Value, 0);

      ITextStructureNavigator Navigator  = Provider.NavigatorService.GetTextStructureNavigator(SubjectBuffer);
      TextExtent              Extent     = Navigator.GetExtentOfWord(SubjectTriggerPoint.Value);
      string                  SearchText = Extent.Span.GetText();

      foreach (string Key in Dictionary.Keys)
      {
        int FoundIndex = SearchText.IndexOf(Key, StringComparison.CurrentCultureIgnoreCase);
        
        if (FoundIndex > -1)
        {
          _ApplicableToSpan = CurrentSnapshot.CreateTrackingSpan(
              Extent.Span.Start + FoundIndex, Key.Length, SpanTrackingMode.EdgeInclusive
            );

          string Value;

          Dictionary.TryGetValue(Key, out Value);
          
          if (Value != null)
            _QuickInfoContent.Add(Value);
          else
            _QuickInfoContent.Add("");

          return;
        }
      }

      _ApplicableToSpan = null;
    }

    private bool IsDisposed;

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
