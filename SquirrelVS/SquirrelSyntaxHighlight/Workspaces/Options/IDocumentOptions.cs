using Microsoft.CodeAnalysis.Options;

namespace SquirrelSyntaxHighlight.Options
{
    /// <summary>
    /// Returned from a <see cref="IDocumentOptionsProvider"/>
    /// </summary>
    interface IDocumentOptions
    {
        bool TryGetDocumentOption(Document document, OptionKey option, OptionSet underlyingOptions, out object value);
    }
}
