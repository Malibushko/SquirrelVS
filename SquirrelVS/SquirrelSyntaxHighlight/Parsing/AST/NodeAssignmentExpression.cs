using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeAssignmentExpression : ASTNodeBase
  {
    public ASTNodeBase Left;
    public ASTNodeBase Right;
    public string      Operator = "=";

    public NodeAssignmentExpression()
    {
      Type = ASTNodeType.AssignmentExpression;
    }
  }
}
