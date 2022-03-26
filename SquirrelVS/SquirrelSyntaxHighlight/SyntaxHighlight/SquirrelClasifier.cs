using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SquirrelSyntaxHighlight.Editor;
using Microsoft.VisualStudio.Shell;
using tree_sitter;
using SquirrelSyntaxHighlight.Infrastructure.Syntax;
using SquirrelSyntaxHighlight.Queries;
using Task = System.Threading.Tasks.Task;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClasifier : IClassifier, ISquirrelTextBufferInfoEventSink
  {
    [Import(typeof(SVsServiceProvider))]
    internal IServiceProvider Site = null;

    private readonly object Key = new object();

    private static Dictionary<string, string> QueryCache = new Dictionary<string, string>
    {
      // Empty
    };

    IClassificationTypeRegistryService ClassificationTypeRegistry;    
    ITextBuffer                        Buffer;
    SquirrelTextBufferInfo             BufferInfo;

    internal SquirrelClasifier(
        IClassificationTypeRegistryService _Registry,
        ITextBuffer                        _Buffer
      )
    {
      ClassificationTypeRegistry = _Registry;
      Buffer                     = _Buffer;
      BufferInfo                 = SquirrelTextBufferInfo.ForBuffer(Site, _Buffer);

      if (!QueryCache.ContainsKey(SyntaxTreeQueries.HIGHLIGHTS_QUERY))
        QueryCache.Add(SyntaxTreeQueries.HIGHLIGHTS_QUERY, File.ReadAllText(SyntaxTreeQueries.HIGHLIGHTS_QUERY));

      BufferInfo.AddSink(Key, this);
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

      int SnapEndPosition = _Snapshot.End.Position;

      for (; SnapEndPosition > SnapStartPosition && SnapEndPosition < _Snapshot.Snapshot.Length; --SnapEndPosition)
      {
        if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapEndPosition]))
          break;
      }

      if (SnapStartPosition == SnapEndPosition)
        return new List<ClassificationSpan>();

      return TryGetNodeSpans(new SnapshotSpan(_Snapshot.Snapshot, new Span(SnapStartPosition, SnapEndPosition - SnapStartPosition)));
    }

    private List<ClassificationSpan> TryGetNodeSpans(
        SnapshotSpan _Snapshot
      )
    {
      List<ClassificationSpan> Spans = new List<ClassificationSpan>();
      
      var Root = BufferInfo.GetNodeAt(_Snapshot.Span);
      
      foreach (Tuple<string, Span> CaptureSpan in  BufferInfo.ExecuteQuery(Root, QueryCache[SyntaxTreeQueries.HIGHLIGHTS_QUERY]))
      {
        var Type = ClassificationTypeRegistry.GetClassificationType($"Squirrel.{CaptureSpan.Item1}");

        if (Type == null)
          continue;

        Spans.Add(new ClassificationSpan(
                     new SnapshotSpan(
                        _Snapshot.Snapshot,
                        CaptureSpan.Item2
                     ),
                     Type
                 )
          );
      }

      return Spans;
    }

    public Task SquirrelTextBufferEventAsync(
        SquirrelTextBufferInfo          _Sender, 
        SquirrelTextBufferInfoEventArgs _Args
      )
    {
      return Task.Run( () =>
      {
        switch (_Args.Event)
        {
          case SquirrelTextBufferInfoEvents.ParseTreeChanged:
          {
            var Args = _Args as SquirrelTreeChangedArgs;

            foreach (var Span in Args.ChangedSpans)
              ClassificationChanged(this, new ClassificationChangedEventArgs(new SnapshotSpan(Buffer.CurrentSnapshot, Span)));

            break;
          }
        }
      });
    }

    #endregion

    #region Public Events
#pragma warning disable 67

    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67
    #endregion
  }
}
