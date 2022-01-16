using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;

namespace SquirrelSyntaxHighlight
{
  [Export(typeof(IClassifierProvider))]
  [ContentType("Squirrel")]
  internal class SquirrelClassifierProvider : IClassifierProvider
  {
    [Import]
    internal IClassificationTypeRegistryService ClassificationRegistry = null;

    private Dictionary<ITextBuffer, SquirrelClasifier> ClassifierRegistry;

    public SquirrelClassifierProvider()
    {
      ClassifierRegistry = new Dictionary<ITextBuffer, SquirrelClasifier>();
    }

    public IClassifier GetClassifier(ITextBuffer _TextBuffer)
    {
      if (ClassifierRegistry.TryGetValue(_TextBuffer, out SquirrelClasifier _Classifier))
        return _Classifier;

      var Classifier = new SquirrelClasifier(ClassificationRegistry, _TextBuffer);

      ClassifierRegistry.Add(_TextBuffer, Classifier);

      return Classifier;
    }
  }
}
