using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeVariableDeclaraction : ASTNodeBase
  {
    public NodeVariableDeclaraction()
    {
      Type = ASTNodeType.VariableDeclaration;
    }
  }
}
