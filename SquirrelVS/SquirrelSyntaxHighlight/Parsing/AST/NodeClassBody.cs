using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeClassBody : ASTNodeBase
  {
    public NodeClassBody()
    {
      Type = ASTNodeType.ClassBody;
    }
  }
}
