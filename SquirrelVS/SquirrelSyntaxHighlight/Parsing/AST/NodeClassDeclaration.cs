using System;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeClassDeclaration : ASTNodeBase
  {
    public NodeClassDeclaration()
    {
      Type = ASTNodeType.ClassDeclaration;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.Keyword", new Span(StartPosition, "class".Length));
    }
  }
}
