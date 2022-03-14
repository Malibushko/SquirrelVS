using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tree_sitter;


namespace SquirrelSyntaxHighlight.Editor
{
  internal class SyntaxTreeWalker
  {
    static public IEnumerable<TSNode> Traverse(
        TSTreeCursor _Cursor
      )
    {
      bool ReachedRoot = false;
      bool Retracing;

      while (!ReachedRoot)
      { 
        using (var Node = api.TsTreeCursorCurrentNode(ref _Cursor))
          yield return Node;

        if (api.TsTreeCursorGotoFirstChild(ref _Cursor))
          continue;

        if (api.TsTreeCursorGotoNextSibling(ref _Cursor))
          continue;

        Retracing = true;

        while (Retracing)
        {
          if (!api.TsTreeCursorGotoParent(ref _Cursor))
          {
            Retracing   = false;
            ReachedRoot = true;
          }

          if (api.TsTreeCursorGotoNextSibling(ref _Cursor))
            Retracing = false;
        }
      }
    }
  }
}
