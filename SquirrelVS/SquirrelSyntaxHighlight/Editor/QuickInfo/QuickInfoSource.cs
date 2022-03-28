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
    private QuickInfoSourceProvider     Provider;
    private ITextBuffer                 SubjectBuffer;
    private List<Tuple<string, string>> KnownItems;
    private CodeDatabaseService         CodeDatabase;

    public QuickInfoSource(
        QuickInfoSourceProvider _Provider, 
        ITextBuffer             _SubjectBuffer,
        CodeDatabaseService     _CodeDatabase
      )
    {
      Provider      = _Provider;
      SubjectBuffer = _SubjectBuffer;
      CodeDatabase  = _CodeDatabase;

      KnownItems = new List<Tuple<string, string>>();

      foreach (var Function in CodeDatabase.GetBuiltinFunctionsInfo())
        KnownItems.Add(new Tuple<string, string>(Function.Name, Function.ToString() + "\n" + Function.Documentation));

      foreach (var Variable in CodeDatabase.GetBuiltinVariables())
        KnownItems.Add(new Tuple<string, string>(Variable.Name, Variable.ToString() + "\n" + Variable.Documentation));
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

      foreach (Tuple<string, string> Item in KnownItems)
      {
        int FoundIndex = SearchText.IndexOf(Item.Item1, StringComparison.CurrentCultureIgnoreCase);
        
        if (FoundIndex > -1)
        {
          _ApplicableToSpan = CurrentSnapshot.CreateTrackingSpan(
              Extent.Span.Start + FoundIndex, Item.Item1.Length, SpanTrackingMode.EdgeInclusive
            );

          _QuickInfoContent.Add(Item.Item2 ?? "");

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
