using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeIf : ASTNodeBase
  {
    public ASTNodeBase Test;
    public ASTNodeBase Consequent;
    public ASTNodeBase Alternate;

    public NodeIf()
    {
      Type = ASTNodeType.IfStatement;
    }
  }
}
