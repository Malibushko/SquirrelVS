using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquirrelSyntaxHighlight.Parsing;

namespace SquirrelSyntaxHighlight.Infrastructure
{
  public enum Severity
  {
    Suppressed  = 0,
    Error       = 1,
    Warning     = 2,
    Information = 3,
    Hint        = 4
  }

  public class ErrorSink
  {
    public static readonly ErrorSink Null = new ErrorSink();

    public ErrorSink()
    {
      // Empty
    }

    public virtual void Add(string message, SourceSpan span, int errorCode, Severity severity)
    {
      // Stub
    }
  }
}
