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
  internal class QuickInfoSource : IQuickInfoSource
  {
    private QuickInfoSourceProvider    Provider;
    private ITextBuffer                SubjectBuffer;
    private Dictionary<string, string> Dictionary;
    private CodeDatabaseService        CodeDatabase;

    public QuickInfoSource(
        QuickInfoSourceProvider _Provider, 
        ITextBuffer             _SubjectBuffer,
        CodeDatabaseService     _CodeDatabase
      )
    {
      Provider      = _Provider;
      SubjectBuffer = _SubjectBuffer;
      CodeDatabase  = _CodeDatabase;

      Dictionary = new Dictionary<string, string>();

      foreach (var Function in CodeDatabase.GetBuiltinFunctionsInfo())
        Dictionary.Add(Function.Name, Function.ToString() + "\n" + Function.Documentation);

      foreach (var Variable in CodeDatabase.GetBuiltinVariables())
        Dictionary.Add(Variable.Name, Variable.ToString() + "\n" + Variable.Documentation);
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
