using Microsoft.CodeAnalysis.Host;
using SquirrelSyntaxHighlight.Compilation;
using SquirrelSyntaxHighlight.Syntax;

namespace SquirrelSyntaxHighlight.Host
{
    internal interface ICompilationFactoryService : ILanguageService
    {
        CompilationBase CreateCompilation(SyntaxTreeBase syntaxTree);
    }
}
