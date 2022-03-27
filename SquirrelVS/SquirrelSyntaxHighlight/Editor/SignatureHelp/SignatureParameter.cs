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
  internal class SignatureParameter : IParameter
  {
    public string     Documentation { get; private set; }
    public Span       Locus { get; private set; }
    public string     Name { get; private set; }
    public ISignature Signature { get; private set; }
    public Span       PrettyPrintedLocus { get; private set; }

    public SignatureParameter(
        string     _Documentation, 
        Span       _Locus, 
        string     _Name, 
        ISignature _Signature
      )
    {
      Documentation = _Documentation;
      Locus         = _Locus;
      Name          = _Name;
      Signature     = _Signature;
    }
  }

}
