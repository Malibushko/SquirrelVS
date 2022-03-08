using System.Threading;
using Microsoft.CodeAnalysis.Host;
using SquirrelSyntaxHighlight.Compilation;
using SquirrelSyntaxHighlight.Symbols;
using SquirrelSyntaxHighlight.Syntax;

namespace SquirrelSyntaxHighlight.LanguageServices
{
    internal interface ISemanticFactsService : ILanguageService
    {
        ISymbol GetDeclaredSymbol(SemanticModelBase semanticModel, ISyntaxToken token, CancellationToken cancellationToken);
    }
}
