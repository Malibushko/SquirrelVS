using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeLiteral : ASTNodeBase
  {
    public string Raw;

    public NodeLiteral()
    {
      Type = ASTNodeType.Literal;
    }
  }
}
