using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeBinaryOperator : ASTNodeBase
  {
    public string Operator;

    public NodeBinaryOperator()
    {
      Type = ASTNodeType.BinaryOperator;
    }
  }
}
