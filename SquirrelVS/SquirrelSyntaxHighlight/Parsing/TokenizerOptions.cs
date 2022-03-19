using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Parsing
{
  [Flags]
  public enum TokenizerOptions
  {
    None = 0,
    Verbatim = 1,
    VerbatimCommentsAndLineJoins = 2,
    GroupingRecovery = 4,
    StubFile = 8,
    FStringExpression = 16
  }
}
