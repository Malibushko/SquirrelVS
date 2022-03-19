// Python Tools for Visual Studio
// Copyright(c) Microsoft Corporation
// All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the License); you may not use
// this file except in compliance with the License. You may obtain a copy of the
// License at http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY
// IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
// MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache Version 2.0 License for specific language governing
// permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using SquirrelSyntaxHighlight.Parsing;

namespace SquirrelSyntaxHighlight.Editor
{
  [DebuggerDisplay("{GetDebugView(),nq}")]
  internal struct LineTokenization : ITag
  {
    public readonly LineToken[]   Tokens;
    public readonly object        State;
    public readonly ITrackingSpan Line;

    public LineTokenization(
        IEnumerable<TokenInfo> _Tokens,
        object                 _State,
        ITextSnapshotLine      _Line
      )
    {
      Tokens = _Tokens.Select(Token => new LineToken(Token, _Line.EndIncludingLineBreak)).ToArray();
      State  = _State;
      Line   = _Line.Snapshot.CreateTrackingSpan(_Line.ExtentIncludingLineBreak, SpanTrackingMode.EdgeNegative);
    }

    internal string GetDebugView()
    {
      var StringBuilder = new StringBuilder();
      
      if (State != null)
        StringBuilder.Append(State != null ? "S " : "  ");
      
      if (Tokens != null)
      {
        for (var i = 0; i < Tokens.Length; i++)
        {
          StringBuilder.Append('[');
          StringBuilder.Append(Tokens[i].Category);
          StringBuilder.Append(']');
        }
      }

      return StringBuilder.ToString();
    }
  }

  internal struct LineToken
  {
    public LineToken(
        TokenInfo _Token, 
        int       _LineLength
      )
    {
      Category = _Token.Category;
      Trigger  = _Token.Trigger;
      Column   = _Token.SourceSpan.Start.Column - 1;

      if (_Token.SourceSpan.Start.Line == _Token.SourceSpan.End.Line)
      {
        // Token on the same line is easy
        Length = _Token.SourceSpan.End.Column - _Token.SourceSpan.Start.Column;
      }
      else if (_Token.SourceSpan.End.Line == _Token.SourceSpan.Start.Line + 1 && _Token.SourceSpan.End.Column == 1)
      {
        // Token ending at the start of the next line is a known special case
        Length = _LineLength - Column;
      }
      else
      {
        // Tokens spanning lines should not be added to a LineTokenization
        throw new ArgumentException("Cannot cache multiline token");
      }
    }

    public TokenCategory Category;
    public TokenTriggers Trigger;
    /// <summary>
    /// 0-based index where the token starts on the line.
    /// </summary>
    public int Column;
    /// <summary>
    /// Number of characters included in the token.
    /// </summary>
    public int Length;
  }

  struct TrackingTokenInfo
  {
    internal TrackingTokenInfo(
        LineToken     _Token, 
        int           _LineNumber, 
        ITrackingSpan _LineSpan
      )
    {
      if (_LineNumber < 0)
        throw new ArgumentOutOfRangeException(nameof(_LineNumber));
      
      LineToken  = _Token;
      LineNumber = _LineNumber;
      LineSpan   = _LineSpan;
    }

    private readonly LineToken     LineToken;
    public  readonly int           LineNumber;
    private readonly ITrackingSpan LineSpan;

    public TokenCategory Category => LineToken.Category;
    public TokenTriggers Trigger  => LineToken.Trigger;

    /// <summary>
    /// Returns true if the location is on the same line and between either end. If
    /// <see cref="IsAdjacent(SourceLocation)"/> is true, this will also be true.
    /// </summary>
    /// <param name="_Location"></param>
    /// <returns></returns>
    public bool Contains(
        SourceLocation _Location
      )
    {
      var Column = _Location.Column - 1;

      return _Location.Line - 1 == LineNumber &&
          (Column >= LineToken.Column && Column <= LineToken.Column + LineToken.Length);
    }

    /// <summary>
    /// Returns true if the location is on the same line and at either end of this token.
    /// </summary>
    public bool IsAdjacent(
        SourceLocation _Location
      )
    {
      var Column = _Location.Column - 1;

      return _Location.Line - 1 == LineNumber &&
          (Column == LineToken.Column || Column == LineToken.Column + LineToken.Length);
    }

    public bool IsAtStart(
        SourceLocation _Location
      )
    {
      return _Location.Line - 1 == LineNumber && _Location.Column - 1 == LineToken.Column;
    }

    public bool IsAtEnd(
        SourceLocation _Location
      )
    {
      return _Location.Line - 1 == LineNumber && _Location.Column - 1 == LineToken.Column + LineToken.Length;
    }

    public TokenInfo ToTokenInfo()
    {
      return new TokenInfo
      {
        Category   = Category,
        Trigger    = Trigger,
        SourceSpan = ToSourceSpan()
      };
    }

    public SourceSpan ToSourceSpan()
    {
      return new SourceSpan(
          new SourceLocation(LineNumber + 1, LineToken.Column + 1),
          new SourceLocation(LineNumber + 1, LineToken.Column + LineToken.Length + 1)
        );
    }

    public SnapshotSpan ToSnapshotSpan(
        ITextSnapshot _Snapshot
      )
    {
      // Note that this assumes the content of the line has not changed
      // since the tokenization was created. Lines can move up and down
      // within the file, and this will handle it correctly, but when a
      // line is edited the span returned here may not be valid.
      var Line = LineSpan.GetSpan(_Snapshot);

      var StartColumn = Math.Min(LineToken.Column, Line.Length);
      var EndColumn   = Math.Min(LineToken.Column + LineToken.Length, Line.Length);

      return new SnapshotSpan(Line.Start + StartColumn, Line.Start + EndColumn);
    }

    public string GetText(ITextSnapshot _Snapshot) => ToSnapshotSpan(_Snapshot).GetText();
  }

  /// <summary>
  /// Represents cached information on line tokens from a given snapshot.
  /// </summary>
  /// <remarks>The tokenization snapshot is immutable in relation 
  /// to the text buffer snapshot. If text buffer is updated, 
  /// tokenization snapshot continues using buffer snapshot it was
  /// created on.</remarks>
  internal interface ILineTokenizationSnapshot : IDisposable
  {
    /// <summary>
    /// Gets the tokenization for the specified line.
    /// </summary>
    /// <param name="_Line">The line to get tokenization for.</param>
    /// <param name="_LazyTokenizer">Tokenizer factory</param>
    LineTokenization GetLineTokenization(
        ITextSnapshotLine _Line, 
        Lazy<Tokenizer>   _LazyTokenizer
      );
  }

  internal class TokenCache
  {
    private readonly object     Lock = new object();
    private LineTokenizationMap Map = new LineTokenizationMap();

    // Controls 'copy on write' when buffer changes from a background thread
    private int UseCount;

    /// <summary>
    /// Obtains tokenization snapshot that is immutable 
    /// in relation to the text buffer snapshot.
    /// </summary>
    /// <returns></returns>
    internal ILineTokenizationSnapshot GetSnapshot()
    {
      lock (Lock)
      {
        UseCount++;

        return new LineTokenizationSnapshot(this, Map);
      }
    }

    /// <summary>
    /// Releases tokenization snapshot
    /// </summary>
    internal void Release(
        LineTokenizationMap _Map
      )
    {
      lock (Lock)
      {
        if (Map == _Map)
        {
          if (UseCount == 0)
            throw new InvalidOperationException("Line tokenization map is not in use");
          
          UseCount--;
        }
      }
    }

    internal void Clear()
    {
      lock (Lock)
      {
        Map = new LineTokenizationMap();
      }
    }

    internal void Update(
        TextContentChangedEventArgs _Args, 
        Lazy<Tokenizer>             _LazyTokenizer
      )
    {
      lock (Lock)
      {
        // Copy on write. No need to copy if no one is using the map.
        if (UseCount > 0)
        {
          Map      = Map.Clone();
          UseCount = 0;
        }

        Map.Update(_Args, _LazyTokenizer);
      }
    }

    internal class LineTokenizationSnapshot : ILineTokenizationSnapshot
    {
      private readonly TokenCache          Cache;
      private readonly LineTokenizationMap Map;

      internal LineTokenizationSnapshot(
          TokenCache          _Cache, 
          LineTokenizationMap _Map
        )
      {
        Cache = _Cache;
        Map   = _Map;
      }

      void IDisposable.Dispose() => Cache.Release(Map);

      LineTokenization ILineTokenizationSnapshot.GetLineTokenization(ITextSnapshotLine _Line, Lazy<Tokenizer> _LazyTokenizer)
          => Map.GetLineTokenization(_Line, _LazyTokenizer);
    }

    internal class LineTokenizationMap
    {
      private readonly object    Lock = new object();
      private LineTokenization[] Map;

      internal LineTokenizationMap() { }

      private LineTokenizationMap(
          LineTokenization[] _Map
        )
      {
        Map = _Map;
      }

      internal LineTokenizationMap Clone()
      {
        lock (Lock)
        {
          LineTokenization[] TempMap = null;

          if (Map != null)
          {
            TempMap = new LineTokenization[Map.Length];
            Array.Copy(TempMap, Map, TempMap.Length);
          }

          return new LineTokenizationMap(TempMap);
        }
      }

      internal LineTokenization GetLineTokenization(
          ITextSnapshotLine _Line, 
          Lazy<Tokenizer>   _LazyTokenizer
        )
      {
        var LineNumber = _Line.LineNumber;

        lock (Lock)
        {
          EnsureCapacity(_Line.Snapshot.LineCount);
          
          var Start = IndexOfPreviousTokenization(LineNumber + 1, 0, out var LineTokenization);

          while (++Start <= LineNumber)
          {
            if (!TryGetTokenization(Start, out LineTokenization))
              Map[Start] = LineTokenization = _LazyTokenizer.Value.TokenizeLine(_Line.Snapshot.GetLineFromLineNumber(Start));
          }
          return LineTokenization;
        }
      }

      internal void Update(
          TextContentChangedEventArgs _Args, 
          Lazy<Tokenizer>             _LazyTokenizer
        )
      {
        var _Snapshot = _Args.After;

        lock (Lock)
        {
          EnsureCapacity(_Snapshot.LineCount);

          foreach (var Change in _Args.Changes)
          {
            var Line = _Snapshot.GetLineNumberFromPosition(Change.NewPosition) + 1;
            
            if (Change.LineCountDelta > 0)
              InsertLines(Line, Change.LineCountDelta);
            else if (Change.LineCountDelta < 0)
              DeleteLines(Line, Math.Min(-Change.LineCountDelta, Map.Length - Line));

            ApplyChanges(new SnapshotSpan(_Snapshot, Change.NewSpan), _LazyTokenizer);
          }
        }
      }

      private void ApplyChanges(
          SnapshotSpan    _Span, 
          Lazy<Tokenizer> _LazyTokenizer
        )
      {
        var FirstLine = _Span.Start.GetContainingLine().LineNumber;
        var LastLine  = _Span.End.GetContainingLine().LineNumber;

        AssertCapacity(FirstLine);

        // find the closest line preceding firstLine for which we know tokenizer state
        FirstLine = IndexOfPreviousTokenization(FirstLine, 0, out var LineTokenization) + 1;

        for (var LineNumber = FirstLine; LineNumber < _Span.Snapshot.LineCount; ++LineNumber)
        {
          var Line = _Span.Snapshot.GetLineFromLineNumber(LineNumber);

          Map[LineNumber] = LineTokenization = _LazyTokenizer.Value.TokenizeLine(Line);
          
          // stop if we visited all affected lines and the current line has no tokenization state
          // or its previous state is the same as the new state.
          if (LineNumber > LastLine)
            break;
        }
      }

      /// <summary>
      /// Looks for the first cached tokenization preceding the given line.
      /// Returns the line we have a tokenization for or minLine - 1 if there is none.
      /// </summary>
      private int IndexOfPreviousTokenization(
          int                  _Line, 
          int                  _MinLine, 
          out LineTokenization _Tokenization
        )
      {
        _Line--;

        while (_Line >= _MinLine)
        {
          if (Map[_Line].Tokens != null)
          {
            _Tokenization = Map[_Line];
            return _Line;
          }
          _Line--;
        }

        _Tokenization = default(LineTokenization);
        
        return _MinLine - 1;
      }

      private bool TryGetTokenization(
          int                  _Line, 
          out LineTokenization _Tokenization
        )
      {
        _Tokenization = Map[_Line];

        if (_Tokenization.Tokens != null)
          return true;
        
        _Tokenization = default(LineTokenization);
        
        return false;
      }

      private void EnsureCapacity(
          int _Capacity
        )
      {
        if (Map == null)
        {
          Map = new LineTokenization[_Capacity];

          return;
        }

        if (_Capacity > Map.Length)
          Array.Resize(ref Map, Math.Max(_Capacity, (Map.Length + 1) * 2));
      }

      [Conditional("DEBUG")]
      private void AssertCapacity(
          int _Capacity
        )
      {
        Debug.Assert(Map != null);
        Debug.Assert(Map.Length > _Capacity);
      }

      private void DeleteLines(
          int _Index, 
          int _Count
        )
      {
        if (_Index > Map.Length - _Count)
          throw new ArgumentOutOfRangeException(nameof(_Index), "Must be 'count' less than the size of the cache");
        
        Array.Copy(Map, _Index + _Count, Map, _Index, Map.Length - _Index - _Count);

        for (var i = 0; i < _Count; i++)
          Map[Map.Length - i - 1] = default(LineTokenization);
      }

      private void InsertLines(
          int _Index, 
          int _Count
        )
      {
        Array.Copy(Map, _Index, Map, _Index + _Count, Map.Length - _Index - _Count);

        for (var i = 0; i < _Count; i++)
          Map[_Index + i] = default(LineTokenization);
      }
    }
  }
}
