using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Python.Parsing;
using SquirrelSyntaxHighlight.Infrastructure;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

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
    private readonly TokenCache                                                   TokensCache;

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
      TokensCache = new TokenCache();
      
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
        UpdateTokenCache(_Args);
      
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
      ClearTokenCache();

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

      UpdateTokenCache(_Args);
      
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

    /// <summary>
    /// Returns the first token containing or adjacent to the specified point.
    /// </summary>
    public TrackingTokenInfo? GetTokenAtPoint(
        SnapshotPoint _Point
      )
    {
      return GetTrackingTokens(new SnapshotSpan(_Point, 0))
            .Cast<TrackingTokenInfo?>()
            .FirstOrDefault();
    }

    /// <summary>
    /// Returns tokens for the specified line.
    /// </summary>
    public IEnumerable<TrackingTokenInfo> GetTokens(
        ITextSnapshotLine _Line
      )
    {
      using (var CacheSnapshot = TokensCache.GetSnapshot())
      {
        var LineTokenization = CacheSnapshot.GetLineTokenization(_Line, GetTokenizerLazy());
        var LineNumber       = _Line.LineNumber;
        var LineSpan         = _Line.Snapshot.CreateTrackingSpan(_Line.ExtentIncludingLineBreak, SpanTrackingMode.EdgeNegative);

        return LineTokenization.Tokens.Select(t => new TrackingTokenInfo(t, LineNumber, LineSpan));
      }
    }

    public IEnumerable<TrackingTokenInfo> GetTokens(
        SnapshotSpan _Span
      )
    {
      return GetTrackingTokens(_Span);
    }

    /// <summary>
    /// Iterates forwards through tokens starting from the token at or
    /// adjacent to the specified point.
    /// </summary>
    public IEnumerable<TrackingTokenInfo> GetTokensForwardFromPoint(
        SnapshotPoint _Point
      )
    {
      var Line = _Point.GetContainingLine();

      foreach (var Token in GetTrackingTokens(new SnapshotSpan(_Point, Line.End)))
        yield return Token;

      while (Line.LineNumber < Line.Snapshot.LineCount - 1)
      {
        Line = Line.Snapshot.GetLineFromLineNumber(Line.LineNumber + 1);
        
        // Use line.Extent because GetLineTokens endpoints are inclusive - we
        // will get the line break token because it is adjacent, but no
        // other repetitions.
        foreach (var Token in GetTrackingTokens(Line.Extent))
          yield return Token;
      }
    }

    /// <summary>
    /// Iterates backwards through tokens starting from the token at or
    /// adjacent to the specified point.
    /// </summary>
    public IEnumerable<TrackingTokenInfo> GetTokensInReverseFromPoint(
        SnapshotPoint _Point
      )
    {
      var Line = _Point.GetContainingLine();

      foreach (var Token in GetTrackingTokens(new SnapshotSpan(Line.Start, _Point)).Reverse())
        yield return Token;

      while (Line.LineNumber > 0)
      {
        Line = Line.Snapshot.GetLineFromLineNumber(Line.LineNumber - 1);
        
        // Use line.Extent because GetLineTokens endpoints are inclusive - we
        // will get the line break token because it is adjacent, but no
        // other repetitions.
        foreach (var Token in GetTrackingTokens(Line.Extent).Reverse())
          yield return Token;
      }
    }

    internal IEnumerable<TrackingTokenInfo> GetTrackingTokens(
        SnapshotSpan _Span
      )
    {
      int FirstLine = _Span.Start.GetContainingLine().LineNumber;
      int LastLine  = _Span.End.GetContainingLine().LineNumber;

      int StartColumn = _Span.Start - _Span.Start.GetContainingLine().Start;
      int EndColumn   = _Span.End - _Span.End.GetContainingLine().Start;

      // We need current state of the cache since it can change from a background thread
      using (var CacheSnapshot = TokensCache.GetSnapshot())
      {
        var TokenizerLazy = GetTokenizerLazy();

        for (int Line = FirstLine; Line <= LastLine; ++Line)
        {
          var LineTokenization = CacheSnapshot.GetLineTokenization(_Span.Snapshot.GetLineFromLineNumber(Line), TokenizerLazy);

          foreach (var Token in LineTokenization.Tokens.MaybeEnumerate())
          {
            if (Line == FirstLine && Token.Column + Token.Length < StartColumn)
              continue;
            
            if (Line == LastLine && Token.Column > EndColumn)
              continue;
            
            yield return new TrackingTokenInfo(Token, Line, LineTokenization.Line);
          }
        }
      }
    }

    #region Token Cache Management

    private Lazy<Tokenizer> GetTokenizerLazy()
        => new Lazy<Tokenizer>(() => new Tokenizer(PythonLanguageVersion.V37, options: TokenizerOptions.Verbatim | TokenizerOptions.VerbatimCommentsAndLineJoins));

    private void ClearTokenCache() => TokensCache.Clear();

    private void UpdateTokenCache(
        TextContentChangedEventArgs _Args
      )
    {
      // NOTE: Runs on background thread
      var Snapshot = _Args.After;

      if (Snapshot.TextBuffer != Buffer)
      {
        Debug.Fail("Mismatched buffer");
        return;
      }

//      if (Snapshot.IsReplBufferWithCommand())
//        return;

      // Prevent later updates overwriting our tokenization
      TokensCache.Update(_Args, GetTokenizerLazy());
    }

    #endregion
  }
}
