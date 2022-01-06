using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("Squirrel")]
    internal class SquirrelClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        static SquirrelClasifier Classifier;

        public IClassifier GetClassifier(ITextBuffer _TextBuffer)
        {
            if (Classifier == null)
                Classifier = new SquirrelClasifier(ClassificationRegistry);

            return Classifier;
        }
    }
}
