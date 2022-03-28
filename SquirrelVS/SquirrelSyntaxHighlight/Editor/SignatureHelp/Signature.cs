using System;
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

namespace SquirrelSyntaxHighlight.Editor.SignatureHelp
{
  internal class Signature : ISignature
  {
    private ITextBuffer                    m_SubjectBuffer;
    private IParameter                     m_CurrentParameter;
    private string                         m_Content;
    private string                         m_Documentation;
    private ITrackingSpan                  m_ApplicableToSpan;
    private ReadOnlyCollection<IParameter> m_Parameters;
    private string                         m_PrintContent;
    private string                         m_Name;

    internal Signature(
        ITextBuffer                    _SubjectBuffer, 
        string                         _Content, 
        string                         _Name,
        string                         _Documentation, 
        ReadOnlyCollection<IParameter> _Parameters
      )
    {
      m_SubjectBuffer = _SubjectBuffer;
      m_Content       = _Content;
      m_Name          = _Name;
      m_Documentation = _Documentation;
      m_Parameters    = _Parameters;

      m_SubjectBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(OnSubjectBufferChanged);
    }
    public IParameter CurrentParameter
    {
      get { return m_CurrentParameter; }

      internal set
      {
        if (m_CurrentParameter != value)
        {
          IParameter PreviousParameter = m_CurrentParameter;
          
          m_CurrentParameter = value;

          this.RaiseCurrentParameterChanged(PreviousParameter, m_CurrentParameter);
        }
      }
    }
    private void RaiseCurrentParameterChanged(
        IParameter _PreviousCurrentParameter, 
        IParameter _NewCurrentParameter
      )
    {
      EventHandler<CurrentParameterChangedEventArgs> Handler = this.CurrentParameterChanged;
      
      if (Handler != null)
        Handler(this, new CurrentParameterChangedEventArgs(_PreviousCurrentParameter, _NewCurrentParameter));
    }
    internal void ComputeCurrentParameter()
    {
      if (Parameters.Count == 0)
      {
        CurrentParameter = null;

        return;
      }

      //the number of commas in the string is the index of the current parameter
      string SignatureText = ApplicableToSpan.GetText(m_SubjectBuffer.CurrentSnapshot);

      int CurrentIndex = 0;
      int CommaCount   = 0;

      while (CurrentIndex < SignatureText.Length)
      {
        int CommaIndex = SignatureText.IndexOf(',', CurrentIndex);
        
        if (CommaIndex == -1)
          break;
        
        CommaCount++;

        CurrentIndex = CommaIndex + 1;
      }

      if (CommaCount < Parameters.Count)
        CurrentParameter = Parameters[CommaCount];
      else
        CurrentParameter = Parameters[Parameters.Count - 1];
    }
    internal void OnSubjectBufferChanged(
        object                      _Sender, 
        TextContentChangedEventArgs _Args
      )
    {
      this.ComputeCurrentParameter();
    }

    public ITrackingSpan ApplicableToSpan
    {
      get { return (m_ApplicableToSpan); }

      internal set { m_ApplicableToSpan = value; }
    }

    public string Content
    {
      get { return (m_Content); }

      internal set { m_Content = value; }
    }

    public string Documentation
    {
      get { return (m_Documentation); }

      internal set { m_Documentation = value; }
    }

    public ReadOnlyCollection<IParameter> Parameters
    {
      get { return (m_Parameters); }

      internal set { m_Parameters = value; }
    }

    public string PrettyPrintedContent
    {
      get { return (m_PrintContent); }

      internal set { m_PrintContent = value; }
    }

    public string Name
    {
      get { return (m_Name); }

      internal set { m_Name = value; }
    }

    public event EventHandler<CurrentParameterChangedEventArgs> CurrentParameterChanged;
  }
}
