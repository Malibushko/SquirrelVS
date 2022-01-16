using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeLocalVariableDeclaration : ASTNodeBase
  {
    public NodeLocalVariableDeclaration()
    {
      Type = ASTNodeType.LocalVariableStatement;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.Keyword", new Span(StartPosition, "local".Length));
    }
  }
}
