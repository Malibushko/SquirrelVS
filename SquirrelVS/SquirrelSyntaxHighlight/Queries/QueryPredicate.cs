using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SquirrelSyntaxHighlight.Queries
{
  internal abstract class QueryPredicate
  {
    public string Capture;
    public string Value;

    public abstract bool Match(
        string _CaptureValue
      );
  }

  internal class MatchQueryPredicate : QueryPredicate
  {
    public override bool Match(
          string _CaptureValue
       )
    {
      return Regex.IsMatch(_CaptureValue, Value); 
    }
  }
  internal class EqualQueryPredicate : QueryPredicate
  {
    public override bool Match(
        string _CaptureValue  
      )
    {
      return _CaptureValue == Value;
    }
  }
}
