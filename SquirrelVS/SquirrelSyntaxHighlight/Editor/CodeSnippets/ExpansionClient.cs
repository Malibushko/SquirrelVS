using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using SquirrelSyntaxHighlight.Infrastructure;

namespace SquirrelSyntaxHighlight.Editor.CodeSnippets
{
  internal class ExpansionClient : IVsExpansionClient
  {
    private readonly IVsTextLines Lines;
    private readonly IVsExpansion Expansion;
    private readonly IVsTextView  View;
    private readonly ITextView    TextView;

    private IVsExpansionSession Session;
    
    private bool SessionEnded;
    private bool SelectEndSpan;

    private ITrackingPoint SelectionStart;
    private ITrackingPoint SelectionEnd;

    public const string SurroundsWithString          = "SurroundsWith";
    public const string SurroundsWithStatementString = "SurroundsWithStatement";
    public const string ExpansionString              = "Expansion";

    public ExpansionClient(
        ITextView                       _TextView, 
        IVsEditorAdaptersFactoryService _EditorAdaptersFactoryService
      )
    {
      if (_EditorAdaptersFactoryService == null)
        throw new ArgumentNullException(nameof(_EditorAdaptersFactoryService));

      TextView  = _TextView ?? throw new ArgumentNullException(nameof(_TextView));
      View      = _EditorAdaptersFactoryService.GetViewAdapter(TextView);
      Lines     = (IVsTextLines)_EditorAdaptersFactoryService.GetBufferAdapter(TextView.TextBuffer);
      Expansion = Lines as IVsExpansion;

      if (Expansion == null)
        throw new ArgumentException("TextBuffer does not support expansions");
    }

    public bool InSession
    {
      get
      {
        return Session != null;
      }
    }

    public int EndExpansion()
    {
      SessionEnded   = true;
      Session        = null;
      SelectionStart = null;
      SelectionEnd   = null;

      return VSConstants.S_OK;
    }

    private static int TryGetXmlNodes(
        IVsExpansionSession _Session, 
        out XElement        _Header, 
        out XElement        _Snippet
      )
    {
      MSXML.IXMLDOMNode HeaderNode;
      MSXML.IXMLDOMNode SnippetNode;

      int Result;
      
      _Header = null;
      _Snippet = null;

      if (ErrorHandler.Failed(Result = _Session.GetHeaderNode(null, out HeaderNode)))
        return Result;
    
      if (ErrorHandler.Failed(Result = _Session.GetSnippetNode(null, out SnippetNode)))
        return Result;
      
      _Header  = XElement.Parse(HeaderNode.xml);
      _Snippet = XElement.Parse(SnippetNode.xml);

      return VSConstants.S_OK;
    }

    public int FormatSpan(
        IVsTextLines _Buffer, 
        TextSpan[]   _Span
      )
    {
      XElement Header;
      XElement Snippet;
      
      int Result = TryGetXmlNodes(Session, out Header, out Snippet);
      
      if (ErrorHandler.Failed(Result))
        return Result;
      
      var Namespace      = Header.Name.NamespaceName;
      bool SurroundsWith = Header
          .Elements(XName.Get("SnippetTypes", Namespace))
          .Elements(XName.Get("SnippetType", Namespace))
          .Any(n => n.Value == SurroundsWithString);

      bool SurroundsWithStatement = Header
          .Elements(XName.Get("SnippetTypes", Namespace))
          .Elements(XName.Get("SnippetType", Namespace))
          .Any(n => n.Value == SurroundsWithStatementString);

      Namespace           = Snippet.Name.NamespaceName;
      var DeclarationList = Snippet
          .Element(XName.Get("Declarations", Namespace))?
          .Elements()
          .Elements(XName.Get("ID", Namespace))
          .Select(n => n.Value)
          .Where(n => !string.IsNullOrEmpty(n))
          .ToList() ?? new List<string>();

      var CodeText = Snippet
          .Element(XName.Get("Code", Namespace))?
          .Value ?? string.Empty;

      // get the indentation of where we're inserting the code...
      string BaseIndentation = GetBaseIndentation(_Span);
      
      int StartPosition;

      _Buffer.GetPositionOfLineIndex(_Span[0].iStartLine, _Span[0].iStartIndex, out StartPosition);
      
      var InsertTrackingPoint = TextView.TextBuffer.CurrentSnapshot.CreateTrackingPoint(StartPosition, PointTrackingMode.Positive);

      TextSpan? EndSpan = null;

      using (var Edit = TextView.TextBuffer.CreateEdit())
      {
        if (SurroundsWith || SurroundsWithStatement)
        {
          // this is super annoyning...  Most languages can do a surround with and $selected$ can be
          // an empty string and everything's the same.  But in Python we can't just have something like
          // "while True: " without a pass statement.  So if we start off with an empty selection we
          // need to insert a pass statement.  This is the purpose of the "SurroundsWithStatement"
          // snippet type.
          //
          // But, to make things even more complicated, we don't have a good indication of what's the 
          // template text vs. what's the selected text.  We do have access to the original template,
          // but all of the values have been replaced with their default values when we get called
          // here.  So we need to go back and re-apply the template, except for the $selected$ part.
          //
          // Also, the text has \n, but the inserted text has been replaced with the appropriate newline
          // character for the buffer.
          var TemplateText = CodeText.Replace("\n", TextView.Options.GetNewLineCharacter());
          
          foreach (var Declaration in DeclarationList)
          {
            string DefaultValue;

            if (ErrorHandler.Succeeded(Session.GetFieldValue(Declaration, out DefaultValue)))
              TemplateText = TemplateText.Replace("$" + Declaration + "$", DefaultValue);
          }

          TemplateText = TemplateText.Replace("$end$", "");

          // we can finally figure out where the selected text began witin the original template...
          int SelectedIndex = TemplateText.IndexOfOrdinal("$selected$", ignoreCase: true);
          if (SelectedIndex != -1)
          {
            var Selection = TextView.Selection;

            // now we need to get the indentation of the $selected$ element within the template,
            // as we'll need to indent the selected code to that level.
            string Indentation = GetTemplateSelectionIndentation(TemplateText, SelectedIndex);

            var Start = SelectionStart.GetPosition(TextView.TextBuffer.CurrentSnapshot);
            var End   = SelectionEnd.GetPosition(TextView.TextBuffer.CurrentSnapshot);
            
            if (End < Start)
            {
              // we didn't actually have a selction, and our negative tracking pushed us
              // back to the start of the buffer...
              End = Start;
            }

            var SelectedSpan = Span.FromBounds(Start, End);

            if (SurroundsWithStatement &&
                String.IsNullOrWhiteSpace(TextView.TextBuffer.CurrentSnapshot.GetText(SelectedSpan)))
            {
              // we require a statement here and the user hasn't selected any code to surround,
              // so we insert a pass statement (and we'll select it after the completion is done)
              Edit.Replace(new Span(StartPosition + SelectedIndex, End - Start), "pass");

              // Surround With can be invoked with no selection, but on a line with some text.
              // In that case we need to inject an extra new line.
              var EndLine = TextView.TextBuffer.CurrentSnapshot.GetLineFromPosition(End);
              var EndText = EndLine.GetText().Substring(End - EndLine.Start);
              
              if (!String.IsNullOrWhiteSpace(EndText))
                Edit.Insert(End, TextView.Options.GetNewLineCharacter());

              // we want to leave the pass statement selected so the user can just
              // continue typing over it...
              var StartLine = TextView.TextBuffer.CurrentSnapshot.GetLineFromPosition(StartPosition + SelectedIndex);
              
              SelectEndSpan = true;
              
              EndSpan = new TextSpan()
              {
                iStartLine  = StartLine.LineNumber,
                iEndLine    = StartLine.LineNumber,
                iStartIndex = BaseIndentation.Length + Indentation.Length,
                iEndIndex   = BaseIndentation.Length + Indentation.Length + 4,
              };
            }

            IndentSpan(
                Edit,
                Indentation,
                TextView.TextBuffer.CurrentSnapshot.GetLineFromPosition(Start).LineNumber + 1, // 1st line is already indented
                TextView.TextBuffer.CurrentSnapshot.GetLineFromPosition(End).LineNumber
              );
          }
        }

        // we now need to update any code which was not selected  that we just inserted.
        IndentSpan(Edit, BaseIndentation, _Span[0].iStartLine + 1, _Span[0].iEndLine);

        Edit.Apply();
      }

      if (EndSpan != null)
        Session.SetEndSpan(EndSpan.Value);

      return Result;
    }

    private static string GetTemplateSelectionIndentation(
        string _TemplateText, 
        int    _SelectedIndex
      )
    {
      string Indentation = "";

      for (int i = _SelectedIndex - 1; i >= 0; i--)
      {
        if (_TemplateText[i] != '\t' && _TemplateText[i] != ' ')
        {
          Indentation = _TemplateText.Substring(i + 1, _SelectedIndex - i - 1);
          break;
        }
      }

      return Indentation;
    }

    private string GetBaseIndentation(
        TextSpan[] _TextSpan
      )
    {
      var    IndentationLine = TextView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(_TextSpan[0].iStartLine).GetText();
      string BaseIndentation = IndentationLine;
      
      for (int i = 0; i < IndentationLine.Length; i++)
      {
        if (IndentationLine[i] != ' ' && IndentationLine[i] != '\t')
        {
          BaseIndentation = IndentationLine.Substring(0, i);
          break;
        }
      }

      return BaseIndentation;
    }

    private void IndentSpan(
        ITextEdit _Edit, 
        string    _Indentation, 
        int       _StartLine, 
        int       _EndLine
      )
    {
      var Snapshot = TextView.TextBuffer.CurrentSnapshot;

      for (int i = _StartLine; i <= _EndLine; i++)
      {
        var CurrentLine = Snapshot.GetLineFromLineNumber(i);

        _Edit.Insert(CurrentLine.Start, _Indentation);
      }
    }

    public int GetExpansionFunction(
        MSXML.IXMLDOMNode        Node, 
        string                   BSTRFieldName, 
        out IVsExpansionFunction _Func
      )
    {
      _Func = null;
      return VSConstants.S_OK;
    }

    public int IsValidKind(
        IVsTextLines pBuffer, 
        TextSpan[]   ts, 
        string       bstrKind, 
        out int      pfIsValidKind
      )
    {
      pfIsValidKind = 1;

      return VSConstants.S_OK;
    }

    public int IsValidType(
        IVsTextLines pBuffer, 
        TextSpan[]   ts, 
        string[]     rgTypes, 
        int          iCountTypes, 
        out int      pfIsValidType
      )
    {
      pfIsValidType = 1;

      return VSConstants.S_OK;
    }

    public int OnAfterInsertion(
        IVsExpansionSession _Session
      )
    {
      return VSConstants.S_OK;
    }

    public int OnBeforeInsertion(
        IVsExpansionSession _Session
      )
    {
      Session = _Session;

      return VSConstants.S_OK;
    }

    public int OnItemChosen(
        string pszTitle, 
        string pszPath
      )
    {
      int CaretLine, CaretColumn;
      GetCaretPosition(out CaretLine, out CaretColumn);

      var TextSpan = new TextSpan() { iStartLine = CaretLine, iStartIndex = CaretColumn, iEndLine = CaretLine, iEndIndex = CaretColumn };
      return InsertNamedExpansion(pszTitle, pszPath, TextSpan);
    }

    public int InsertNamedExpansion(
        string   pszTitle, 
        string   pszPath, 
        TextSpan _TextSpan
      )
    {
      if (Session != null)
      {
        // if the user starts an expansion session while one is in progress
        // then abort the current expansion session
        Session.EndCurrentExpansion(1);
        Session = null;
      }

      var Selection = TextView.Selection;
      var Snapshot  = Selection.Start.Position.Snapshot;

      SelectionStart = Snapshot.CreateTrackingPoint(Selection.Start.Position, PointTrackingMode.Positive);
      SelectionEnd   = Snapshot.CreateTrackingPoint(Selection.End.Position, PointTrackingMode.Negative);
      SelectEndSpan  = SessionEnded = false;

      int Result = Expansion.InsertNamedExpansion(
          pszTitle,
          pszPath,
          _TextSpan,
          this,
          SnippetUtilities.LanguageServiceGuid,
          0,
          out Session
      );

      if (ErrorHandler.Succeeded(Result))
      {
        if (SessionEnded)
          Session = null;
      }

      return Result;
    }

    public int NextField()
    {
      return Session.GoToNextExpansionField(0);
    }

    public int PreviousField()
    {
      return Session.GoToPreviousExpansionField();
    }

    public int EndCurrentExpansion(
        bool _LeaveCaret
      )
    {
      if (SelectEndSpan)
      {
        TextSpan[] endSpan = new TextSpan[1];
        
        if (ErrorHandler.Succeeded(Session.GetEndSpan(endSpan)))
        {
          var Snapshot  = TextView.TextBuffer.CurrentSnapshot;
          var StartLine = Snapshot.GetLineFromLineNumber(endSpan[0].iStartLine);
          var Span      = new Span(StartLine.Start + endSpan[0].iStartIndex, 4);

          TextView.Caret.MoveTo(new SnapshotPoint(Snapshot, Span.Start));
          TextView.Selection.Select(new SnapshotSpan(TextView.TextBuffer.CurrentSnapshot, Span), false);

          return Session.EndCurrentExpansion(1);
        }
      }
      return Session.EndCurrentExpansion(_LeaveCaret ? 1 : 0);
    }

    public int PositionCaretForEditing(
        IVsTextLines _Buffer, 
        TextSpan[]   _Span
      )
    {
      return VSConstants.S_OK;
    }

    private void GetCaretPosition(
        out int _CaretLine, 
        out int _CaretColumn
      )
    {
      ErrorHandler.ThrowOnFailure(View.GetCaretPos(out _CaretLine, out _CaretColumn));

      // Handle virtual space
      int LineLength;

      ErrorHandler.ThrowOnFailure(Lines.GetLengthOfLine(_CaretLine, out LineLength));

      if (_CaretColumn > LineLength)
        _CaretColumn = LineLength;
    }
  }
}
