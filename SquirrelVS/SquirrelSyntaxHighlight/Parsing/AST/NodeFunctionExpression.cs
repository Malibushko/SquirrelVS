using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight
{
  public class NodeFunctionExpression : ASTNodeBase
  {
    public NodeFunctionExpression()
    {
      Type = ASTNodeType.FunctionExpression;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.Keyword", new Span(StartPosition, "function".Length));
    }
  }
}
