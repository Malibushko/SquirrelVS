using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Editor.CodeDatabase
{
  static class CodeDatabaseConstants
  {
    private static readonly string RESOURCES_PATH              = "Resources";
    public static readonly string  BUILTIN_FUNCTIONS_INFO_PATH = $"{RESOURCES_PATH}/BuiltinFunctions.json";
    public static readonly string  BUILTIN_VARIABLES_INFO_PATH = $"{RESOURCES_PATH}/BuiltinVariables.json";
  }
}
