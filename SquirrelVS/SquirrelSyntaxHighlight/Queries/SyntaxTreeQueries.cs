using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Queries
{
  static class SyntaxTreeQueries
  {
    private static string QUERIES_ROOT = "Queries/Queries";

    public static readonly string LOCALS_QUERY      = QUERIES_ROOT + "/locals.scm";
    public static readonly string HIGHLIGHTS_QUERY  = QUERIES_ROOT + "/highlights.scm";
    public static readonly string TAGS_QUERY        = QUERIES_ROOT + "/tags.scm";
    public static readonly string INJECTIONS_QUERY  = QUERIES_ROOT + "/injections.scm";
  }
}
