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

    static public readonly Dictionary<string, string> BuiltinFunction = new Dictionary<string, string>
    {
      { "array",           "create and returns array of a specified size.if the optional parameter fill is specified its value will be used to fill the new array's slots. If the fill paramter is omitted null is used instead."},
      { "seterrorhandler", "sets the runtime error handler" },
      { "setdebughook",    "sets the debug hook" },
      { "enabledebuginfo", "enable/disable the debug line information generation at compile time. enable != null enables . enable == null disables" },
      { "getroottable",    "returns the root table of the VM." },
      { "setroottable",    "sets the root table of the VM." },
      { "assert",          "throws an exception if exp is null" },
      { "getconsttable",   "returns the const table of the VM." },
      {"setconsttable",    "sets the const table of the VM." },
      { "print",           "prints value in the standard output" },
      { "compilestring",   "compiles a string containing a squirrel script into a function and returns it" },
      { "collectgarbage",  "calls the garbage collector and returns the number of reference cycles found(and deleted)" },
      { "type",            "return the 'raw' type of an object without invoking the metatmethod '_typeof'." },
      { "getstackinfos",   "returns the stack informations of a given call stack level. returns a table formatted as follow:" },
      { "newthread",       "creates a new cooperative thread object(coroutine) and returns it" }
    };

    static public readonly SortedSet<string> BuiltinDelegates = new SortedSet<string>
    {
      "tofloat",
      "tostring",
      "tointeger",
      "tochar",
      "weakreaf",
      "slice",
      "find",
      "tolower",
      "toupper",
      "len",
      "rawget",
      "rawset",
      "rawdelete",
      "rawin",
      "clear",
      "append",
      "push",
      "extend",
      "pop",
      "top",
      "insert",
      "remove",
      "resize",
      "sort",
      "reverse",
      "call",
      "pcall",
      "acall",
      "pacall",
      "bindenv",
      "instance",
      "getattribute",
      "getattributes",
      "getclass",
      "getstatus",
      "wakeup",
      "getstatus",
      "ref"
    };

    static public readonly Dictionary<string, string> BuiltinVariables = new Dictionary<string, string>
    {
      { "_charsize_",  "string values describing the version of VM and compiler." },
      { "_version_",   "size in bytes of the internal VM rapresentation for characters(1 for ASCII builds 2 for UNICODE builds)."},
      { "_intsize_",   "size in bytes of the internal VM rapresentation for integers(4 for 32bits builds 8 for 64bits builds)." },
      { "_floatsize_", "size in bytes of the internal VM rapresentation for floats(4 for single precision builds 8 for double precision builds)." }
    };

  }
}
