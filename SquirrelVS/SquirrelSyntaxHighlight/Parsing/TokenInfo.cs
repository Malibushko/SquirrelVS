using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquirrelSyntaxHighlight.Infrastructure;
using Microsoft.VisualStudio.Text;

namespace SquirrelSyntaxHighlight.Parsing
{
  public struct TokenInfo : IEquatable<TokenInfo>
  {
    public TokenCategory Category { get; set; }
    public TokenTriggers Trigger { get; set; }
    public SourceSpan SourceSpan { get; set; }

    public bool Equals(
        TokenInfo _Other
      )
    {
      return Category == _Other.Category && Trigger == _Other.Trigger && SourceSpan == _Other.SourceSpan;
    }

    public override string ToString()
    {
      return "TokenInfo: {0}, {1}, {2}".FormatInvariant((object)this.SourceSpan, (object)this.Category, (object)this.Trigger);
    }
  }
}
