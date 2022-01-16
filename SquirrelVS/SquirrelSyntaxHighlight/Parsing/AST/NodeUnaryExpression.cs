using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeUnaryExpression : ASTNodeBase
  {
    public string Operator;

    public NodeUnaryExpression()
    {
      Type = ASTNodeType.UnaryExpression;
    }
    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.Keyword", new Span(StartPosition, Operator.Length));
    }
  }
}
