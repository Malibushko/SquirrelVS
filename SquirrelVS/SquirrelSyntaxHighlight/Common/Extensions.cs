using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Common
{
  static class Extensions
  {
    internal static bool IsOpenGrouping(
        this string _String
      )
    {
      return _String == "{" || _String == "[" || _String == "(";
    }

    internal static bool IsCloseGrouping(
        this string _String
      )
    {
      return _String == "}" || _String == "]" || _String == ")";
    }
  }
}
