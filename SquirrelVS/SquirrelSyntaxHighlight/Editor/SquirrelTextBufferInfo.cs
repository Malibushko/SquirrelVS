using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SquirrelSyntaxHighlight.Infrastructure;
using SquirrelSyntaxHighlight.Parsing;
using SquirrelSyntaxHighlight.Infrastructure.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Editor
{

  internal sealed class SquirrelTextBufferInfo
  {
    private static readonly object SquirrelTextBufferInfoKey = new { Id = "SquirrelTextBufferInfo" };

    public static SquirrelTextBufferInfo ForBuffer(
        IServiceProvider _Site, 
        ITextBuffer      _Buffer
      )
    {
      var BufferInfo = (_Buffer ?? throw new ArgumentNullException(nameof(_Buffer))).Properties.GetOrCreateSingletonProperty(
          SquirrelTextBufferInfoKey,
          () => new SquirrelTextBufferInfo(_Site, _Buffer)
        );

      if (BufferInfo.ReplaceRequested)
        BufferInfo = BufferInfo.ReplaceBufferInfo();
      
      return BufferInfo;
    }

    public static SquirrelTextBufferInfo TryGetForBuffer(
        ITextBuffer _Buffer
      )
    {
      SquirrelTextBufferInfo BufferInfo;

      if (_Buffer == null)
        return null;
      
      if (!_Buffer.Properties.TryGetProperty(SquirrelTextBufferInfoKey, out BufferInfo) || BufferInfo == null)
        return null;
      
      if (BufferInfo.ReplaceRequested)
        BufferInfo = BufferInfo.ReplaceBufferInfo();
      
      return BufferInfo;
    }

    /// <summary>
    /// Calling this function marks the buffer to be replaced next
    /// time it is retrieved.
    /// </summary>
    public static void MarkForReplacement(
        ITextBuffer _Buffer
      )
    {
      var BufferInfo = TryGetForBuffer(_Buffer);

      if (BufferInfo != null)
        BufferInfo.ReplaceRequested = true;
    }

    public static IEnumerable<SquirrelTextBufferInfo> GetAllFromView(
        ITextView _View
      )
    {
      return _View.BufferGraph.GetTextBuffers(_ => true)
                              .Select(b => TryGetForBuffer(b))
                              .Where(b => b != null);
    }

    private readonly ConcurrentDictionary<object, ISquirrelTextBufferInfoEventSink> EventSinks;
    private Lazy<TSTree>                                                   Tree;
    private readonly TSParser                                                       Parser;

    private readonly bool HasChangedOnBackground;
    private bool          ReplaceRequested;

    private SquirrelTextBufferInfo(
        IServiceProvider _Site, 
        ITextBuffer      _Buffer
      )
    {
      Site        = _Site;
      Buffer      = _Buffer;
      EventSinks  = new ConcurrentDictionary<object, ISquirrelTextBufferInfoEventSink>();
      Parser      = api.TsParserNew();

      if (!api.TsParserSetLanguage(Parser, squirrel.TreeSitterSquirrel()))
        throw new Exception("failed to initialize parser");

      Tree = new Lazy<TSTree>(() => api.TsParserParseString(Parser, null, _Buffer.CurrentSnapshot.GetText(), (uint)_Buffer.CurrentSnapshot.Length));

      if (Buffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument Document))
      {
        this.Document                  = Document;
        this.Document.EncodingChanged += Document_EncodingChanged;
      }

      Buffer.ContentTypeChanged += Buffer_ContentTypeChanged;
      Buffer.Changed            += Buffer_TextContentChanged;
      Buffer.ChangedLowPriority += Buffer_TextContentChangedLowPriority;

      if (Buffer is ITextBuffer2 Buffer2)
      {
        HasChangedOnBackground       = true;
        Buffer2.ChangedOnBackground += Buffer_TextContentChangedOnBackground;
      }
    }

    private SquirrelTextBufferInfo ReplaceBufferInfo()
    {
      var NewInfo = new SquirrelTextBufferInfo(Site, Buffer);
      
      foreach (var Sink in EventSinks)  
        NewInfo.EventSinks[Sink.Key] = Sink.Value;
    
      Buffer.Properties[SquirrelTextBufferInfoKey] = NewInfo;

      Buffer.ContentTypeChanged -= Buffer_ContentTypeChanged;
      Buffer.Changed            -= Buffer_TextContentChanged;
      Buffer.ChangedLowPriority -= Buffer_TextContentChangedLowPriority;

      if (Buffer is ITextBuffer2 Buffer2)
        Buffer2.ChangedOnBackground -= Buffer_TextContentChangedOnBackground;

      InvokeSinks(new SquirrelNewTextBufferInfoEventArgs(SquirrelTextBufferInfoEvents.NewTextBufferInfo, NewInfo));

      return NewInfo;
    }

    public ITextBuffer   Buffer { get; }
    public ITextDocument Document { get; }
    public ITextSnapshot CurrentSnapshot => Buffer.CurrentSnapshot;
    public IContentType  ContentType     => Buffer.ContentType;

    public IServiceProvider Site { get; }

    #region Events

    private void Buffer_TextContentChanged(
        object                      _Sender, 
        TextContentChangedEventArgs _Args
      )
    {
      if (!HasChangedOnBackground)
        UpdateTree(_Args);
      
      InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.TextContentChanged, _Args));
      
      if (!HasChangedOnBackground)
        InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.TextContentChangedOnBackgroundThread, _Args));
    }

    private void Buffer_TextContentChangedLowPriority(
        object                      _Sender, 
        TextContentChangedEventArgs _Args
      )
    {
      InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.TextContentChangedLowPriority, _Args));
    }

    private void Buffer_ContentTypeChanged(
        object                      _Sender, 
        ContentTypeChangedEventArgs _Args
      )
    {
      ClearTree();

      InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.ContentTypeChanged, _Args));
    }

    private void Buffer_TextContentChangedOnBackground(
        object                      _Sender, 
        TextContentChangedEventArgs _Args
      )
    {
      if (!HasChangedOnBackground)
      {
        Debug.Fail("Received TextContentChangedOnBackground unexpectedly");
        return;
      }

      UpdateTree(_Args);
      
      InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.TextContentChangedOnBackgroundThread, _Args));
    }

    private void Document_EncodingChanged(
        object                   _Sender, 
        EncodingChangedEventArgs _Args
      )
    {
      InvokeSinks(new SquirrelTextBufferInfoNestedEventArgs(SquirrelTextBufferInfoEvents.DocumentEncodingChanged, _Args));
    }

    #endregion

    #region Sink management

    public void AddSink(
        object                           _Key, 
        ISquirrelTextBufferInfoEventSink _Sink
      )
    {
      if (!EventSinks.TryAdd(_Key, _Sink))
      {
        if (EventSinks[_Key] != _Sink)
        {
          throw new InvalidOperationException("cannot replace existing sink");
        }
      }
    }

    public T GetOrCreateSink<T>(
        object                          _Key, 
        Func<SquirrelTextBufferInfo, T> _Creator
      ) where T : class, ISquirrelTextBufferInfoEventSink
    {
      ISquirrelTextBufferInfoEventSink Sink;

      if (EventSinks.TryGetValue(_Key, out Sink))
        return Sink as T;

      Sink = _Creator(this);

      if (!EventSinks.TryAdd(_Key, Sink))
        Sink = EventSinks[_Key];

      return Sink as T;
    }

    public ISquirrelTextBufferInfoEventSink TryGetSink(
        object _Key
      )
    {
      ISquirrelTextBufferInfoEventSink Sink;

      return EventSinks.TryGetValue(_Key, out Sink) ? Sink : null;
    }

    public bool RemoveSink(
        object _Key
      )
    {
      return EventSinks.TryRemove(_Key, out _);
    }

    private void InvokeSinks(
        SquirrelTextBufferInfoEventArgs _Args
      )
    {
      foreach (var Sink in EventSinks.Values)
      {
        Sink.SquirrelTextBufferEventAsync(this, _Args)
            .HandleAllExceptions(Site, GetType())
            .DoNotWait();
      }
    }

    #endregion

    #region Tree Management

    void UpdateTree(
        TextContentChangedEventArgs _Args
      )
    {
      if (!Tree.IsValueCreated)
        return;

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
            Row = (uint)Line.LineNumber,
            Column = (uint)(Change.OldPosition - Line.Start.Position)
          },

          NewEndPoint = new TSPoint
          {
            Row = (uint)NewLine.LineNumber,
            Column = (uint)(Change.NewEnd - NewLine.Start.Position)
          },

          OldEndPoint = new TSPoint
          {
            Row = (uint)Line.LineNumber,
            Column = (uint)(Change.OldEnd - Line.Start.Position)
          }
        };

        api.TsTreeEdit(Tree.Value, Edit);
      }

      var Text = _Args.After.GetText();

      var EditedTree = api.TsParserParseString(Parser, Tree.Value, Text, (uint)Text.Length);

      uint ChangesCount = 0;
      TSRange[] Changes = api.TsTreeGetChangedRanges(Tree.Value, EditedTree, ref ChangesCount);

      Tree = new Lazy<TSTree>(() => EditedTree);
      
      var Root = api.TsTreeRootNode(Tree.Value);

      List<SnapshotSpan> ChangedSpans = new List<SnapshotSpan>();

      for (uint i = 0; i < ChangesCount; i++)
      {
        ChangedSpans.Add(
            new SnapshotSpan(
              _Args.After,
              new Span(
                (int)Changes[i].StartByte,
                (int)Changes[i].EndByte - (int)Changes[i].StartByte
                )
              )
          );
      }

      InvokeSinks(new SquirrelTreeChanged(SquirrelTextBufferInfoEvents.ParseTreeChanged, ChangedSpans));
    }

    void ClearTree()
    {
      if (Tree.IsValueCreated)
        api.TsTreeDelete(Tree.Value);
    }

    public TSNode GetNodeAt(
        Span _Span
      )
    {
      TSNode Root       = api.TsTreeRootNode(Tree.Value);
      TSNode Descendant = api.TsNodeDescendantForByteRange(Root, (uint)_Span.Start, (uint)_Span.End);
      
      return Descendant;
    }

    public IEnumerable<TSNode> GetNodeWithSymbols(
        SortedSet<string> _Symbols,
        Span              _Span
      )
    {
      TSNode       Root       = api.TsTreeRootNode(Tree.Value);
      TSNode       Descendant = api.TsNodeDescendantForByteRange(Root, (uint)_Span.Start, (uint)_Span.End);
      TSTreeCursor Walker     = api.TsTreeCursorNew(Descendant);
      TSLanguage   Language   = api.TsParserLanguage(Parser);

      foreach (TSNode Node in SyntaxTreeWalker.Traverse(Walker))
      {
        ushort Symbol = api.TsNodeSymbol(Node);

        string Name = api.TsLanguageSymbolName(Language, Symbol);

        if (_Symbols.Contains(Name))
          yield return Node;
      }
    }
    #endregion
  }
}
