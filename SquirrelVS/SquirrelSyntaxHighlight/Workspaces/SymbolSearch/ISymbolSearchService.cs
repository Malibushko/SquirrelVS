using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Host;
using SquirrelSyntaxHighlight.Compilation;
using SquirrelSyntaxHighlight.Symbols;
using SquirrelSyntaxHighlight.Text;

namespace SquirrelSyntaxHighlight.SymbolSearch
{
    internal interface ISymbolSearchService : ILanguageService
    {
        SymbolSpan? FindSymbol(SemanticModelBase semanticModel, SourceLocation position);
        ImmutableArray<SymbolSpan> FindUsages(SemanticModelBase semanticModel, ISymbol symbol);
    }
}
