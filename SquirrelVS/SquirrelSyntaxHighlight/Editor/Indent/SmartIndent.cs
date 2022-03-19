using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace SquirrelSyntaxHighlight.Editor.Indent
{
  internal class SmartIndent : ISmartIndent
  {
    private readonly IServiceProvider Site;
    private readonly ITextView        TextView;

    public SmartIndent(
        IServiceProvider _Site, 
        ITextView        _View
      )
    {
      Site     = _Site ?? throw new ArgumentNullException(nameof(_Site));
      TextView = _View ?? throw new ArgumentNullException(nameof(_View));
    }

    public int? GetDesiredIndentation(
        ITextSnapshotLine _Line
      )
    {
      if (/*Site.LangPrefs.IndentMode == vsIndentStyle.vsIndentStyleSmart*/ true)
        return AutoIndent.GetLineIndentation(SquirrelTextBufferInfo.ForBuffer(Site, _Line.Snapshot.TextBuffer), _Line, TextView);
      else
        return null;
    }

    public void Dispose()
    {
      // Empty
    }
  }
}
