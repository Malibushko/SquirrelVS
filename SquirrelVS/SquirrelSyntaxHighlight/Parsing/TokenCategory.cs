using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Parsing
{
  public enum TokenCategory
  {
    None = 0,
    Comment = 3,
    NumericLiteral = 6,
    CharacterLiteral = 7,
    StringLiteral = 8,
    Keyword = 10,
    Operator = 12,
    Identifier = 14,
    BuiltinIdentifier = 18,
    LanguageDefined = 256
  }
}
