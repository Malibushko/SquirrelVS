using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeParameter : ASTNodeBase
  {
    public string Name;

    public NodeParameter()
    {
      Type = ASTNodeType.Parameter;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.FunctionVariable", new Span(StartPosition, EndPosition - StartPosition));
    }
  }
}
