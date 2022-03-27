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

namespace SquirrelSyntaxHighlight.Editor.QuickInfo
{
  internal class QuickInfoController : IIntellisenseController
  {
    private ITextView                   TextView;
    private IList<ITextBuffer>          SubjectBuffers;
    private QuickInfoControllerProvider Provider;
    private IQuickInfoSession           Session;

    internal QuickInfoController(
        ITextView                   _TextView, 
        IList<ITextBuffer>          _SubjectBuffers, 
        QuickInfoControllerProvider _Provider
      )
    {
      TextView       = _TextView;
      SubjectBuffers = _SubjectBuffers;
      Provider       = _Provider;

      TextView.MouseHover += this.OnTextViewMouseHover;
    }

    private void OnTextViewMouseHover(
        object              _Sender, 
        MouseHoverEventArgs _Args
      )
    {
      SnapshotPoint? Point = TextView.BufferGraph.MapDownToFirstMatch(
          new SnapshotPoint(TextView.TextSnapshot, _Args.Position),
          PointTrackingMode.Positive,
          Snapshot => SubjectBuffers.Contains(Snapshot.TextBuffer),
          PositionAffinity.Predecessor
        );

      if (Point != null)
      {
        ITrackingPoint TriggerPoint = Point.Value.Snapshot.CreateTrackingPoint(Point.Value.Position, PointTrackingMode.Positive);

        if (!Provider.QuickInfoBroker.IsQuickInfoActive(TextView))
          Session = Provider.QuickInfoBroker.TriggerQuickInfo(TextView, TriggerPoint, true);
      }
    }

    public void Detach(
        ITextView _TextView
      )
    {
      if (TextView == _TextView)
      {
        TextView.MouseHover -= this.OnTextViewMouseHover;

        TextView = null;
      }
    }

    public void ConnectSubjectBuffer(
        ITextBuffer _SubjectBuffer
      )
    {
      // Empty
    }

    public void DisconnectSubjectBuffer(
        ITextBuffer _SubjectBuffer
      )
    {
      // Empty
    }
  }
}
