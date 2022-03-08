using System.Collections.Generic;
using System.Threading;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Text;
using SquirrelSyntaxHighlight.Compilation;
using SquirrelSyntaxHighlight.Syntax;

namespace SquirrelSyntaxHighlight.Classification
{
    internal interface IClassificationService : ILanguageService
    {
        void AddSyntacticClassifications(SyntaxTreeBase syntaxTree,
            TextSpan textSpan,
            List<ClassifiedSpan> result,
            CancellationToken cancellationToken);

        void AddSemanticClassifications(
            SemanticModelBase semanticModel,
            TextSpan textSpan,
            Workspace workspace,
            List<ClassifiedSpan> result,
            CancellationToken cancellationToken);
    }
}
