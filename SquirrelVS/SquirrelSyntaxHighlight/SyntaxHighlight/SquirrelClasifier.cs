using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using tree_sitter;
using System.Linq;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClasifier : IClassifier
  {
    internal static object ColorKey = new object { };

    IClassificationTypeRegistryService ClassificationTypeRegistry;
    ITextBuffer Buffer;

    internal SquirrelClasifier(
        IClassificationTypeRegistryService _Registry,
        ITextBuffer _Buffer
      )
    {
      ClassificationTypeRegistry = _Registry;
      Buffer = _Buffer;

      Buffer.Changed += BufferChanged;
    }

    private void BufferChanged(
        object                      _Sender,
        TextContentChangedEventArgs _Args)
    {
      Buffer.Properties.RemoveProperty(ColorKey);
    }

    #region Public Methods
    public IList<ClassificationSpan> GetClassificationSpans(
        SnapshotSpan _Snapshot
      )
    {
      if (Buffer.Properties.TryGetProperty(ColorKey, out List<ClassificationSpan> _Spans))
      {
        return _Spans.Where(Span => Span.Span.OverlapsWith(_Snapshot.Span)).ToList();
      }

      TSParser   Parse            = api.TsParserNew();
      TSLanguage SquirrelLanguage = squirrel.TreeSitterSquirrel();

      api.TsParserSetLanguage(Parse, SquirrelLanguage);

      TSTree Tree = api.TsParserParseString(Parse, null, _Snapshot.Snapshot.GetText(), (uint)_Snapshot.Snapshot.Length);

      TSNode Root = api.TsTreeRootNode(Tree);

      var Spans = TryGetNodeSpans(_Snapshot, SquirrelLanguage, Root);
      Buffer.Properties.AddProperty(ColorKey, Spans);
      
      api.TsTreeDelete(Tree);
      api.TsParserDelete(Parse);

      return Spans.Where(Span => Span.Span.OverlapsWith(_Snapshot.Span)).ToList();
    }

    private List<ClassificationSpan> TryGetNodeSpans(
        SnapshotSpan _Snapshot,
        TSLanguage   _Language,
        TSNode       _Node
      )
    {
      List<ClassificationSpan> Spans = new List<ClassificationSpan>();

      uint ChildCount = api.TsNodeChildCount(_Node);

      for (uint i = 0; i < ChildCount; i++)
      {
        TSNode Node = api.TsNodeChild(_Node, i);

        var Span = TryGetClassificationSpan(_Snapshot, _Language, _Node);

        if (Span != null)
          Spans.Add(Span);

        Spans.AddRange(TryGetNodeSpans(_Snapshot, _Language, Node));
      }

      return Spans;
    }

    private ClassificationSpan TryGetClassificationSpan(
        SnapshotSpan _Snapshot,
        TSLanguage   _Language,
        TSNode       _Node
      )
    {
      var StartPosition = api.TsNodeStartByte(_Node);
      var EndPosition   = api.TsNodeEndByte(_Node);

      if (api.TsNodeIsNamed(_Node))
      {
        ushort Symbol = api.TsNodeSymbol(_Node);

        string Name = api.TsLanguageSymbolName(_Language, Symbol);

        if (Name == "identifier" || Name == "function_name")
        {
          return new ClassificationSpan(
                        new SnapshotSpan(
                          _Snapshot.Snapshot,
                          new Span((int)StartPosition, (int)(EndPosition - StartPosition)
                         )
                        ),
                        ClassificationTypeRegistry.GetClassificationType("Squirrel.Keyword"));
        }
      }

      return null;
    }
    #endregion

    #region Public Events
#pragma warning disable 67

    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67
    #endregion
  }
}
