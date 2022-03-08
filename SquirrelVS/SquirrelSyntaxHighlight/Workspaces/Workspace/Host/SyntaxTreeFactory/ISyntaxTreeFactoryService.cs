using System.Threading;
using Microsoft.CodeAnalysis.Host;
using SquirrelSyntaxHighlight.Syntax;
using SquirrelSyntaxHighlight.Text;

namespace SquirrelSyntaxHighlight.Host
{
    internal interface ISyntaxTreeFactoryService : ILanguageService
    {
        SyntaxTreeBase ParseSyntaxTree(SourceFile file, CancellationToken cancellationToken);
    }
}
