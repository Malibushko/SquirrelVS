using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Editor.CompletionConsts
{
  static class CompletionFunctions
  {
    static private readonly string SnippetPattern = "Press 'Tab' 2 times to insert Code Snippet for ";

    static public readonly Dictionary<string, string> Keywords = new Dictionary<string, string>
    {
      { "if",         SnippetPattern + "If Statement"},
      { "switch",     SnippetPattern + "Switch Statement" },
      { "else",       string.Empty },
      { "for",        SnippetPattern + "For Statement" },
      { "foreach",    SnippetPattern + "Foreach Statement" },
      { "return",     string.Empty },
      { "null",       string.Empty },
      { "const",      string.Empty },
      { "break",      string.Empty },
      { "static",     string.Empty },
      { "class",      SnippetPattern + "Class Statement" },
      { "try",        SnippetPattern + "Try/Catch Clause" },
      { "catch",      string.Empty },
      { "case",       string.Empty },
      { "local",      SnippetPattern + "Local Variable Statement" },
      { "function",   SnippetPattern + "Function Statement" },
      { "delete",     string.Empty },
      { "instanceof", string.Empty },
      { "typeof",     string.Empty },
      { "extends",    string.Empty },
      { "while",      SnippetPattern + "While Statement" },
      { "enum",       SnippetPattern + "Enum Statement" },
      { "throw",      string.Empty }
    };
  }
}
