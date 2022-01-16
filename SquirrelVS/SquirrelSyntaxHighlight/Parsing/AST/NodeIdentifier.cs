using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
  public class NodeIdentifier : ASTNodeBase
  {
    public string Name;

    public NodeIdentifier()
    {
      Type = ASTNodeType.Identifier;
    }

    public override NodeSpan TryGetSpan()
    {
      return new NodeSpan("Squirrel.Global", new Span(StartPosition, EndPosition - StartPosition));
    }
  }
}
