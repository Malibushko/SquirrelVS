using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using SquirrelSyntaxHighlight.Editor.CodeDatabase;

namespace SquirrelSyntaxHighlight.Editor.SignatureHelp
{
  internal class SignatureHelpSource : ISignatureHelpSource
  {
    private ITextBuffer         TextBuffer;
    private CodeDatabaseService Database;

    public SignatureHelpSource(
        ITextBuffer         _TextBuffer,
        CodeDatabaseService _CodeDatabaseService
      )
    {
      this.TextBuffer = _TextBuffer;
      this.Database   = _CodeDatabaseService;
    }

    public void AugmentSignatureHelpSession(
        ISignatureHelpSession _Session, 
        IList<ISignature>     _Signatures
      )
    {
      ITextSnapshot Snapshot = TextBuffer.CurrentSnapshot;

      int Position = _Session.GetTriggerPoint(TextBuffer).GetPosition(Snapshot);

      ITrackingSpan ApplicableToSpan = Snapshot.CreateTrackingSpan(
          new Span(Position, 0), 
          SpanTrackingMode.EdgeInclusive,
          0
        );

      string FunctionName = TryGetFunctionName(Snapshot, Position);

      foreach (var Function in Database.GetBuiltinFunctionsInfo())
      {
        if (Function.Name.Contains(FunctionName) || FunctionName.Contains(Function.Name))
          _Signatures.Add(CreateSignature(TextBuffer, Function.Name, Function.ToString(), Function.Parameters, Function.Documentation, ApplicableToSpan));
      }
    }

    private string TryGetFunctionName(
        ITextSnapshot _Snapshot,
        int           _TriggerPosition
      )
    {
      string FunctionName = string.Empty;

      for (int i = _TriggerPosition - 1; i >= 0; i--)
      {
        if (char.IsWhiteSpace(_Snapshot[i]) || char.IsPunctuation(_Snapshot[i]))
          break;

        FunctionName += _Snapshot[i];
      }

      return new string(FunctionName.Reverse().ToArray());
    }

    private Signature CreateSignature(
        ITextBuffer             _TextBuffer, 
        string                  _MethodName,
        string                  _MethodSignature,
        List<ParameterDataItem> _Parameters,
        string                  _MethodDocumentation, 
        ITrackingSpan           _Span
      )
    {
      Signature Signature = new Signature(_TextBuffer, _MethodSignature, _MethodName, _MethodDocumentation, null);
      
      _TextBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(Signature.OnSubjectBufferChanged);

      //find the parameters in the method signature (expect methodname(one, two)
      string[] Pairs                 = _MethodSignature.Split(new char[] { '(', ',', ')' });
      List<IParameter> ParameterList = new List<IParameter>();

      int LocusSearchStart = 0;

      for (int i = 1; i < Pairs.Length; i++)
      {
        string Parameter = Pairs[i].Trim();

        if (string.IsNullOrEmpty(Parameter))
          continue;

        //find where this parameter is located in the method signature
        int LocusStart = _MethodSignature.IndexOf(Parameter, LocusSearchStart);
        
        if (LocusStart >= 0)
        {
          Span Locus = new Span(LocusStart, Parameter.Length);
          
          LocusSearchStart = LocusStart + Parameter.Length;
          
          ParameterList.Add(new SignatureParameter(_Parameters[i - 1].Documentation, Locus, Parameter, Signature));
        }
      }

      Signature.Parameters       = new ReadOnlyCollection<IParameter>(ParameterList);
      Signature.ApplicableToSpan = _Span;

      Signature.ComputeCurrentParameter();

      return Signature;
    }

    public ISignature GetBestMatch(
        ISignatureHelpSession _Session
      )
    {
      return null;
    }

    private bool IsDisposed;

    public void Dispose()
    {
      if (!IsDisposed)
      {
        GC.SuppressFinalize(this);
        IsDisposed = true;
      }
    }
  }
}
