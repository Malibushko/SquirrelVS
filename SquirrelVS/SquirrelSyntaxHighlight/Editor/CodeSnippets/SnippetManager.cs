using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Editor.CodeSnippets
{
  [Export(typeof(SnippetManager))]
  internal class SnippetManager
  {
    private readonly IServiceProvider                Site;
    private readonly IVsEditorAdaptersFactoryService EditorAdaptersFactory;
    private readonly IVsTextManager2                 TextManager;
    private readonly IVsExpansionManager             VSExpansionManager;
    private readonly IExpansionManager               ExpansionManager;

    private static readonly string[] StandardSnippetTypes     = { ExpansionClient.ExpansionString, ExpansionClient.SurroundsWithString };
    private static readonly string[] SurroundWithSnippetTypes = { ExpansionClient.SurroundsWithString, ExpansionClient.SurroundsWithStatementString };

    [ImportingConstructor]
    public SnippetManager(
        [Import(typeof(SVsServiceProvider))] IServiceProvider _Site,
        [Import] IVsEditorAdaptersFactoryService              _EditorAdaptersFactoryService
      )
    {
      Site                  = _Site ?? throw new ArgumentNullException(nameof(_Site));
      EditorAdaptersFactory = _EditorAdaptersFactoryService ?? throw new ArgumentNullException(nameof(_EditorAdaptersFactoryService));
      TextManager           = _Site.GetService<SVsTextManager, IVsTextManager2>();
      
      TextManager.GetExpansionManager(out VSExpansionManager);
      ExpansionManager = VSExpansionManager as IExpansionManager;
    }

    public bool IsInSession(ITextView textView) => TryGetExpansionClient(textView)?.InSession ?? false;

    public bool ShowInsertionUI(
        ITextView _TextView, 
        bool      _IsSurroundsWith
      )
    {
      if (_TextView == null)
        throw new ArgumentNullException(nameof(_TextView));

      if (VSExpansionManager == null)
        return false;

      string   Prompt;
      string[] SnippetTypes;

      if (_IsSurroundsWith)
      {
        Prompt       = "SurroundWith";
        SnippetTypes = SurroundWithSnippetTypes;
      }
      else
      {
        Prompt       = "InsertSnippet";
        SnippetTypes = StandardSnippetTypes;
      }

      var Client = GetOrCreateExpansionClient(_TextView);

      var Handler = VSExpansionManager.InvokeInsertionUI(
          EditorAdaptersFactory.GetViewAdapter(_TextView),
          Client,
          SnippetUtilities.LanguageServiceGuid,
          SnippetTypes,
          SnippetTypes.Length,
          0,
          null,
          0,
          0,
          Prompt,
          ">"
        );

      return ErrorHandler.Succeeded(Handler);
    }

    public bool EndSession(
        ITextView _TextView, 
        bool      _LeaveCaret
      )
    {
      if (_TextView == null)
        throw new ArgumentNullException(nameof(_TextView));

      var Client = TryGetExpansionClient(_TextView);

      return Client != null && ErrorHandler.Succeeded(Client.EndCurrentExpansion(_LeaveCaret));
    }

    public bool MoveToNextField(
        ITextView _TextView
      )
    {
      if (_TextView == null)
        throw new ArgumentNullException(nameof(_TextView));

      var Client = TryGetExpansionClient(_TextView);

      return Client != null && ErrorHandler.Succeeded(Client.NextField());
    }

    public bool MoveToPreviousField(
        ITextView _TextView
      )
    {
      if (_TextView == null)
        throw new ArgumentNullException(nameof(_TextView));

      var Client = TryGetExpansionClient(_TextView);

      return Client != null && ErrorHandler.Succeeded(Client.PreviousField());
    }

    public bool TryTriggerExpansion(
        ITextView _TextView
      )
    {
      if (_TextView == null)
        throw new ArgumentNullException(nameof(_TextView));
    
      if (VSExpansionManager == null)
        return false;
      
      if (!_TextView.Selection.IsEmpty || _TextView.Caret.Position.BufferPosition <= 0)
        return false;
      
      var Snapshot  = _TextView.TextBuffer.CurrentSnapshot;
      var CaretSpan = new SnapshotSpan(Snapshot, new Span(_TextView.Caret.Position.BufferPosition.Position - 1, 1));

      var BufferInfo = SquirrelTextBufferInfo.ForBuffer(Site, _TextView.TextBuffer);

      var Node = BufferInfo.GetNodeAt(CaretSpan);

      var Start = api.TsNodeStartByte(Node);
      var End   = api.TsNodeEndByte(Node);
      
      if (End != CaretSpan.End.Position)
      {
        // Match C# behavior and only trigger snippet
        // if caret is at the end of an identifier. Otherwise,
        // a TAB should be inserted even if the token matches
        // a snippet shortcut.
        return false;
      }

      var StartPoint = api.TsNodeStartPoint(Node);
      var EndPoint   = api.TsNodeEndPoint(Node);

      var TextBuffer = new char[End - Start];
      
      _TextView.TextBuffer.CurrentSnapshot.CopyTo((int)Start, TextBuffer, 0, (int)(End - Start));

      var TextSpan = new TextSpan[1];
      TextSpan[0].iStartLine  = (int)StartPoint.Row;
      TextSpan[0].iStartIndex = (int)StartPoint.Column;
      TextSpan[0].iEndLine    = (int)EndPoint.Row;
      TextSpan[0].iEndIndex   = (int)EndPoint.Column;

      var Client  = GetOrCreateExpansionClient(_TextView);
      int Handler = VSExpansionManager.GetExpansionByShortcut(
          Client,
          SnippetUtilities.LanguageServiceGuid,
          new string(TextBuffer),
          EditorAdaptersFactory.GetViewAdapter(_TextView),
          TextSpan,
          1,
          out string expansionPath,
          out string title
        );

      if (ErrorHandler.Succeeded(Handler))
      {
        // hr may be S_FALSE if there are multiple expansions,
        // so we don't want to InsertNamedExpansion yet. VS will
        // pop up a selection dialog in this case.
        if (Handler == VSConstants.S_OK)
          return ErrorHandler.Succeeded(Client.InsertNamedExpansion(title, expansionPath, TextSpan[0]));
        
        return true;
      }

      return false;
    }

    private ExpansionClient GetOrCreateExpansionClient(
        ITextView _TextView
      )
    {
      if (!_TextView.Properties.TryGetProperty(typeof(ExpansionClient), out ExpansionClient _Client))
      {
        _Client = new ExpansionClient(_TextView, EditorAdaptersFactory);

        _TextView.Properties.AddProperty(typeof(ExpansionClient), _Client);
      }

      return _Client;
    }

    private ExpansionClient TryGetExpansionClient(ITextView _TextView)
    {
      if (_TextView.Properties.TryGetProperty(typeof(ExpansionClient), out ExpansionClient _Client))
        return _Client;

      return null;
    }
  }
}
