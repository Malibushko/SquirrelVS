using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public class NodeMethodDefinition : ASTNodeBase
  {
    public bool   Static = false;
    public string Kind   = "method";

    public NodeMethodDefinition()
    {
      Type = ASTNodeType.MethodDefinition;
    }
  }
}
