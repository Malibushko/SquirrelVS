using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
    public class SquirrelClasifier : IClassifier
    {
        IClassificationTypeRegistryService ClassificationTypeRegistry;

        internal SquirrelClasifier(IClassificationTypeRegistryService _Registry)
        {
          ClassificationTypeRegistry = _Registry;
        }
      
        #region Public Methods
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan _Span)
        {
            SquirrelClassificationLexer Lexer = new SquirrelClassificationLexer(_Span, ClassificationTypeRegistry);
      
            return Lexer.Lex();
        }

        #endregion

        #region Public Events
        #pragma warning disable 67

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
        
        #pragma warning restore 67
        #endregion
    }
}
