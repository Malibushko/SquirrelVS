using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeBlockStatement : ASTNodeBase
  {
    public NodeBlockStatement()
    {
      Type = ASTNodeType.BlockStatement;
    }
  }
}
