using System;

namespace SquirrelSyntaxHighlight
{
    public sealed class DocumentEventArgs : EventArgs
    {
        public Document Document { get; }

        public DocumentEventArgs(Document document)
        {
            Document = document;
        }
    }
}
