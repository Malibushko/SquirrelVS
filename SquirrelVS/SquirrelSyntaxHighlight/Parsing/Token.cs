using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Parsing
{
  public abstract class Token
  {
    public Token(
        TokenKind _Kind
      )
    {
      Kind = _Kind;
    }

    public TokenKind       Kind { get; }
    public virtual object  Value { get; }
    public virtual string  VerbatimImage { get; }
    public abstract string Image { get; }

    public override string ToString()
    {
      return Value.ToString();
    }
  }
}
