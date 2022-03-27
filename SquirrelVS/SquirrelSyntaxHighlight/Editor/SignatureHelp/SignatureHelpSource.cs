﻿using System;
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
      
      foreach(var Function in Database.GetBuiltinFunctionsInfo())
        _Signatures.Add(CreateSignature(TextBuffer, Function.ToString(), Function.Parameters, Function.Documentation, ApplicableToSpan));
    }

    private Signature CreateSignature(
        ITextBuffer             _TextBuffer, 
        string                  _MethodSignature,
        List<ParameterDataItem> _Parameters,
        string                  _MethodDocumentation, 
        ITrackingSpan           _Span
      )
    {
      Signature Signature = new Signature(_TextBuffer, _MethodSignature, _MethodDocumentation, null);

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
      if (_Session.Signatures.Count > 0)
      {
        ITrackingSpan ApplicableToSpan = _Session.Signatures[0].ApplicableToSpan;
        string        Text             = ApplicableToSpan.GetText(ApplicableToSpan.TextBuffer.CurrentSnapshot);

        if (Text.Trim().Equals("add"))  //get only "add" 
          return _Session.Signatures[0];
      }

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