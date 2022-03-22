using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using SquirrelSyntaxHighlight.Editor;
using Microsoft.VisualStudio.Shell;
using tree_sitter;
using SquirrelSyntaxHighlight.Infrastructure.Syntax;

using Task = System.Threading.Tasks.Task;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClasifier : IClassifier, ISquirrelTextBufferInfoEventSink
  {
    [Import(typeof(SVsServiceProvider))]
    internal IServiceProvider Site = null;

    private readonly object Key = new object();

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
        { "delete",     "Squirrel.Keyword" },
        { "instanceof", "Squirrel.Keyword" },
        { "typeof",     "Squirrel.Keyword" },
        { "extends",    "Squirrel.Keyword" },
        { "constructor", "Squirrel.Keyword" },
        { "while",      "Squirrel.Keyword" },
        { "enum",       "Squirrel.Keyword" },
        { "throw",      "Squirrel.Keyword" }
     };

    public static SortedSet<string> ColoredNodes = new SortedSet<string>
    {
       "comment",
       "string",
       "if",
       "switch",
       "else",
       "for",
       "foreach",
       "return",
       "null",
       "const",
       "break",
       "static",
       "var",
       "class",
       "try",
       "catch",
       "case",
       "local",
       "function",
       "delete",
       "instanceof",
       "typeof",
       "extends",
       "constructor",
       "while",
       "enum",
       "throw"
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

      int SnapEndPosition = _Snapshot.End.Position - 1;

      for (; SnapEndPosition != _Snapshot.Start.Position; --SnapEndPosition)
      {
        if (!char.IsWhiteSpace(_Snapshot.Snapshot[SnapEndPosition]))
          break;
      }

      if (SnapStartPosition == SnapEndPosition)
        return new List<ClassificationSpan>();

      return TryGetNodeSpans(_Snapshot);
    }

    private List<ClassificationSpan> TryGetNodeSpans(
        SnapshotSpan _Snapshot
      )
    {
      List<ClassificationSpan> Spans = new List<ClassificationSpan>();

      foreach (Tuple<TSNode, string> Node in BufferInfo.GetNodeWithSymbols(ColoredNodes, _Snapshot.Span))
      {
        int Start  = (int)api.TsNodeStartByte(Node.Item1);
        int Length = (int)api.TsNodeEndByte(Node.Item1) - Start;

        Spans.Add(new ClassificationSpan(
                     new SnapshotSpan(
                        _Snapshot.Snapshot,
                        new Span(Start, Length)
                     ),
                     ClassificationTypeRegistry.GetClassificationType(NodeClassificator[Node.Item2])
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
