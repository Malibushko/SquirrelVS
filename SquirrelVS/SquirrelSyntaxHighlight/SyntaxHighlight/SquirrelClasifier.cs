using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using tree_sitter;
using System.Linq;
using System.Text.RegularExpressions;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClasifier : IClassifier
  {
    public static readonly Dictionary<string, string> NodeClassificator = new Dictionary<string, string>
     {
        { "comment",    "Squirrel.Comment" },
        { "string",     "Squirrel.String"  },
        { "if",         "Squirrel.Keyword" },
        { "switch",     "Squirrel.Keyword" },
        { "else",       "Squirrel.Keyword" },
        { "for",        "Squirrel.Keyword" },
        { "foreach",    "Squirrel.Keyword" },
        { "return",     "Squirrel.Keyword" },
        { "null",       "Squirrel.Keyword" },
        { "const",      "Squirrel.Keyword" },
        { "break",      "Squirrel.Keyword" },
        { "static",     "Squirrel.Keyword" },
        { "var",        "Squirrel.Keyword" },
        { "class",      "Squirrel.Keyword" },
        { "try",        "Squirrel.Keyword" },
        { "catch",      "Squirrel.Keyword" },
        { "case",       "Squirrel.Keyword" },
        { "local",      "Squirrel.Keyword" },
        { "function",   "Squirrel.Keyword" },
        { "delete",     "Squirrel.Keyword" }
     };

    IClassificationTypeRegistryService ClassificationTypeRegistry;

    TSParser                           Parser;
    TSTree                             SyntaxTree;
    TSLanguage                         Language;
    TSTreeCursor                       Walker;
    TSNode                             Root;

    internal SquirrelClasifier(
        IClassificationTypeRegistryService _Registry,
        ITextBuffer                        _Buffer
      )
    {
      ClassificationTypeRegistry = _Registry;
      Parser                     = api.TsParserNew();
      Language                   = squirrel.TreeSitterSquirrel();

      if (api.TsParserSetLanguage(Parser, Language))
      {
        var Text = _Buffer.CurrentSnapshot.GetText();

        SyntaxTree = api.TsParserParseString(Parser, null, Text, (uint)Text.Length);
        Root       = api.TsTreeRootNode(SyntaxTree);
        Walker     = api.TsTreeCursorNew(Root);
      }

      _Buffer.Changed += BufferChanged;
    }

    private void BufferChanged(
        object                      _Sender,
        TextContentChangedEventArgs _Args)
    {
      var Buffer = (ITextBuffer)_Sender;

      foreach (ITextChange Change in _Args.Changes)
      {
        var Line    = Buffer.CurrentSnapshot.GetLineFromPosition(Change.OldPosition);
        var NewLine = Buffer.CurrentSnapshot.GetLineFromPosition(Change.NewPosition);
        
        TSInputEdit Edit = new TSInputEdit()
        {
          StartByte  = (uint)Change.OldPosition,
          NewEndByte = (uint)Change.NewEnd,
          OldEndByte = (uint)Change.OldEnd,

          StartPoint = new TSPoint()
          {
            Row    = (uint)Line.LineNumber,
            Column = (uint)(Change.OldPosition - Line.Start.Position)
          },

          NewEndPoint = new TSPoint
          {
            Row    = (uint)NewLine.LineNumber,
            Column = (uint)(Change.NewEnd - NewLine.Start.Position)
          },

          OldEndPoint = new TSPoint
          {
            Row    = (uint)Line.LineNumber,
            Column = (uint)(Change.OldEnd - Line.Start.Position)
          }
        };

        api.TsTreeEdit(SyntaxTree, Edit);
      }
      
      var Text = _Args.After.GetText();

      var EditedTree = api.TsParserParseString(Parser, SyntaxTree, Text, (uint)Text.Length);

      uint      ChangesCount = 0;
      TSRange[] Changes      = api.TsTreeGetChangedRanges(SyntaxTree, EditedTree, ref ChangesCount);

      SyntaxTree = EditedTree;
      Root       = api.TsTreeRootNode(SyntaxTree);

      for (uint i = 0; i < ChangesCount; i++)
      {
        try
        {
          ClassificationChanged.Invoke(
            this,
            new ClassificationChangedEventArgs(
              new SnapshotSpan(
                _Args.After,
                new Span(
                  (int)Changes[i].StartByte,
                  (int)Changes[i].EndByte - (int)Changes[i].StartByte
                  )
                )
              )
            );
        } catch (Exception Ex)
        {

        }
      }
    }

    #region Public Methods
    public IList<ClassificationSpan> GetClassificationSpans(
        SnapshotSpan _Snapshot
      )
    {
      int SnapStartPosition = _Snapshot.Start.Position;

      for (; SnapStartPosition < _Snapshot.End.Position; ++SnapStartPosition)
      {
        if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapStartPosition]))
          break;
      }

      int SnapEndPosition = _Snapshot.End.Position - 1;

      for (; SnapEndPosition != _Snapshot.Start.Position; --SnapEndPosition)
      {
        if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapEndPosition]))
          break;
      }

      api.TsTreeCursorReset(Walker, api.TsNodeDescendantForByteRange(Root, (uint)SnapStartPosition, (uint)SnapEndPosition));

      return TryGetNodeSpans(_Snapshot, Walker, Language);
    }

    private List<ClassificationSpan> TryGetNodeSpans(
        SnapshotSpan _Snapshot,
        TSTreeCursor _Cursor,
        TSLanguage   _Language
      )
    {
      List<ClassificationSpan> Spans = new List<ClassificationSpan>();

      bool ReachedRoot = false;
      bool Retracing   = false;

      while (!ReachedRoot)
      {
        var Span = TryGetClassificationSpan(_Snapshot, _Language, api.TsTreeCursorCurrentNode(ref _Cursor), out Retracing);
        
        if (Span != null)
          Spans.Add(Span);

        if (!Retracing)
        {
          if (api.TsTreeCursorGotoFirstChild(ref _Cursor))
            continue;

          if (api.TsTreeCursorGotoNextSibling(ref _Cursor))
            continue;
        }

        Retracing = true;

        while (Retracing)
        {
          if (!api.TsTreeCursorGotoParent(ref _Cursor))
          {
            Retracing   = false;
            ReachedRoot = true;
          }

          if (api.TsTreeCursorGotoNextSibling(ref _Cursor))
            Retracing = false;
        }
      }

      return Spans;
    }

    private ClassificationSpan TryGetClassificationSpan(
        SnapshotSpan _Snapshot,
        TSLanguage   _Language,
        TSNode       _Node,
        out bool     _IsFallthough
      )
    {
      var StartPosition = api.TsNodeStartByte(_Node);

      if (StartPosition >= _Snapshot.Span.End)
      {
        _IsFallthough = true;

        _Node.Dispose();

        return null;
      }
      else
      {
        _IsFallthough = false;
      }

      var EndPosition   = api.TsNodeEndByte(_Node);

      ushort Symbol = api.TsNodeSymbol(_Node);

      string Name = api.TsLanguageSymbolName(_Language, Symbol);

      _Node.Dispose();

      if (NodeClassificator.TryGetValue(Name, out string _ClassificationType))
      {
        return new ClassificationSpan(
                      new SnapshotSpan(
                        _Snapshot.Snapshot,
                        new Span((int)StartPosition, (int)(EndPosition - StartPosition)
                       )
                      ),
                      ClassificationTypeRegistry.GetClassificationType(_ClassificationType));
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
