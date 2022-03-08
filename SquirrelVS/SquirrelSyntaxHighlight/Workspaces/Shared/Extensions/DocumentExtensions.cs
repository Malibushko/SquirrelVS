using Microsoft.CodeAnalysis.Host;

namespace SquirrelSyntaxHighlight.Shared.Extensions
{
    internal static class DocumentExtensions
    {
        public static TLanguageService GetLanguageService<TLanguageService>(this Document document) where TLanguageService : class, ILanguageService
            => document?.LanguageServices?.GetService<TLanguageService>();
    }
}
