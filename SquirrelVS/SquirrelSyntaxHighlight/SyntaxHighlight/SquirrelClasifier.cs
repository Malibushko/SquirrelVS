using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClasifier : IClassifier
  {
    internal static object ColorKey = new object { };

    IClassificationTypeRegistryService ClassificationTypeRegistry;
    ITextBuffer                        Buffer;

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
      List<NodeSpan> Spans;
      
      if (!Buffer.Properties.TryGetProperty(ColorKey, out Spans))
      {
        var Parser = new SquirrelParser(_Snapshot, ClassificationTypeRegistry);

        Parser.Parse();

        Buffer.Properties.AddProperty(ColorKey, Parser.SyntaxTree.GetSpans());

        Spans = Buffer.Properties.GetProperty(ColorKey) as List<NodeSpan>;
      }

      List<ClassificationSpan> ColoredSpans = new List<ClassificationSpan>();

      foreach (var Span in Spans)
      {
        if (Span == null || !Span.Span.OverlapsWith(_Snapshot.Span))
          continue;

        ColoredSpans.Add(
            new ClassificationSpan(
              new SnapshotSpan(_Snapshot.Snapshot, Span.Span), 
              ClassificationTypeRegistry.GetClassificationType(Span.Type)
              )
           );
      }

      return ColoredSpans;
    }

    #endregion

    #region Public Events
#pragma warning disable 67

    public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67
    #endregion
  }
}
