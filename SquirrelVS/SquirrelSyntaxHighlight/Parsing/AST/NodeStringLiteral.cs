using System;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeStringLiteral : ASTNodeBase
  {
    public string Value;

    public NodeStringLiteral()
    {
      Type = ASTNodeType.StringLiteral;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.String", new Span(StartPosition, EndPosition - StartPosition));
    }
  }
}
