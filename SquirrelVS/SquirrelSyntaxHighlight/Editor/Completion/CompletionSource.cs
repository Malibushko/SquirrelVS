using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Operations;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;
using SquirrelSyntaxHighlight.Editor.CompletionConsts;

namespace SquirrelSyntaxHighlight.Editor
{
  internal class CompletionSource : IAsyncCompletionSource
  {
    private CompletionSourceProvider SourceProvider;
    private ITextView                TextView;
    private List<CompletionItem>     CompletionList;
    private CodeDatabaseService      CodeDatabaseService;

    private ITextStructureNavigatorSelectorService NavigatorService;

    private readonly char[]          TriggerCharacters = new char[] { '(' };

    private readonly string[]        DelimiterCharacters = new[] { " ", "\r", "\n", "\t" };
    private readonly char[]          TypicalDimissChars  = new[] { ';', ' ' };

    private bool                     IsDisposed;

    public CompletionSource(
        CompletionSourceProvider               _SourceProvider,
        ITextStructureNavigatorSelectorService _NavigatorService,
        ITextView                              _TextView,
        CodeDatabaseService                    _CodeDatabaseService
      )
    {
      SourceProvider      = _SourceProvider;
      TextView            = _TextView;
      NavigatorService    = _NavigatorService;
      CodeDatabaseService = _CodeDatabaseService;

      CompletionList = new List<CompletionItem>();
      
      foreach (var KeyValue in CompletionFunctions.Keywords)
        CompletionList.Add(new CompletionItem(KeyValue.Key, this, new ImageElement(KnownMonikers.KeywordSnippet.ToImageId()), ImmutableArray<CompletionFilter>.Empty));
      
      foreach(var FunctionInfo in _CodeDatabaseService.GetBuiltinFunctionsInfo())
        CompletionList.Add(new CompletionItem(FunctionInfo.Name, this, new ImageElement(KnownMonikers.Method.ToImageId()), ImmutableArray<CompletionFilter>.Empty));
      
      foreach (var VariableInfo in _CodeDatabaseService.GetBuiltinVariables())
        CompletionList.Add(new CompletionItem(VariableInfo.Name, this, new ImageElement(KnownMonikers.Constant.ToImageId()), ImmutableArray<CompletionFilter>.Empty));
    }

    public void Dispose()
    {
      if (!IsDisposed)
      {
        GC.SuppressFinalize(this);

        IsDisposed = true;
      }
    }

    public Task<CompletionContext> GetCompletionContextAsync(
        IAsyncCompletionSession _Session, 
        CompletionTrigger       _Trigger, 
        SnapshotPoint           _TriggerLocation, 
        SnapshotSpan            _ApplicableToSpan, 
        CancellationToken       _Token
      )
    {
      // See whether we are in the key or value portion of the pair
      var LineStart              = _TriggerLocation.GetContainingLine().Start;
      var SpanBeforeCaret        = new SnapshotSpan(LineStart, _TriggerLocation);
      var TextBeforeCaret        = _TriggerLocation.Snapshot.GetText(SpanBeforeCaret);

      return Task.Run(() => GetContextForValue(TextBeforeCaret));
    }

    private CompletionContext GetContextForValue(string key)
    {
      return new CompletionContext(CompletionList.ToImmutableArray(), null);
    }

    public async Task<object> GetDescriptionAsync(
        IAsyncCompletionSession _Session, 
        CompletionItem          _Item, 
        CancellationToken       _Token
      )
    {
      if (_Token.IsCancellationRequested)
        return null;

      return await Task.Run(() => CodeDatabaseService.GetItemDocumentation(_Item.DisplayText));
    }

    private SnapshotSpan FindTokenSpanAtPosition(
        SnapshotPoint _TriggerLocation
      )
    {
      ITextStructureNavigator Navigator = NavigatorService.GetTextStructureNavigator(_TriggerLocation.Snapshot.TextBuffer);
      TextExtent              Extent    = Navigator.GetExtentOfWord(_TriggerLocation);

      if (_TriggerLocation.Position > 0 && (!Extent.IsSignificant || !Extent.Span.GetText().Any(c => char.IsLetterOrDigit(c))))
        Extent = Navigator.GetExtentOfWord(_TriggerLocation - 1);

      var TokenSpan = _TriggerLocation.Snapshot.CreateTrackingSpan(Extent.Span, SpanTrackingMode.EdgeInclusive);

      var Snapshot = _TriggerLocation.Snapshot;
      var TokenText = TokenSpan.GetText(Snapshot);
      
      if (string.IsNullOrWhiteSpace(TokenText))
      {
        // The token at this location is empty. Return an empty span, which will grow as user types.
        return new SnapshotSpan(_TriggerLocation, 0);
      }

      // Trim quotes and new line characters.
      int StartOffset = 0;
      int EndOffset = 0;

      if (TokenText.Length > 0)
      {
        if (TokenText.StartsWith("\""))
          StartOffset = 1;
      }

      if (TokenText.Length - StartOffset > 0)
      {
        if (TokenText.EndsWith("\"\r\n"))
          EndOffset = 3;
        else if (TokenText.EndsWith("\r\n"))
          EndOffset = 2;
        else if (TokenText.EndsWith("\"\n"))
          EndOffset = 2;
        else if (TokenText.EndsWith("\n"))
          EndOffset = 1;
        else if (TokenText.EndsWith("\""))
          EndOffset = 1;
      }

      return new SnapshotSpan(TokenSpan.GetStartPoint(Snapshot) + StartOffset, TokenSpan.GetEndPoint(Snapshot) - EndOffset);
    }

    public CompletionStartData InitializeCompletion(
        CompletionTrigger _Trigger, 
        SnapshotPoint     _TriggerLocation, 
        CancellationToken _Token
      )
    {
      // We don't trigger completion when user typed
      if (char.IsNumber(_Trigger.Character)         // a number
          || char.IsPunctuation(_Trigger.Character) // punctuation
          || _Trigger.Character == '\n'             // new line
          || _Trigger.Reason == CompletionTriggerReason.Backspace
          || _Trigger.Reason == CompletionTriggerReason.Deletion)
      {
        return CompletionStartData.DoesNotParticipateInCompletion;
      }

      return new CompletionStartData(CompletionParticipation.ProvidesItems, FindTokenSpanAtPosition(_TriggerLocation));
    }
  }
}
