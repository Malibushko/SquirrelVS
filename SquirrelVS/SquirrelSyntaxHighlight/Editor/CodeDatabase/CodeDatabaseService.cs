using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.ComponentModel.Composition;

namespace SquirrelSyntaxHighlight.Editor.CodeDatabase
{
  [Export(typeof(ICodeDatabaseService))]
  public class CodeDatabaseService : ICodeDatabaseService
  {
    public CodeDatabaseService()
    {
      TryLoadBuiltinsInfo();
    }

    public IEnumerable<FunctionDataItem> GetBuiltinFunctionsInfo()
    {
      return m_Items.FindAll(Item => Item.IsBuiltIn && Item is FunctionDataItem).Cast<FunctionDataItem>();
    }

    public IEnumerable<VariableDataItem> GetBuiltinVariables()
    {
      return m_Items.FindAll(Item => Item.IsBuiltIn && Item is VariableDataItem).Cast<VariableDataItem>();
    }

    public string GetItemDocumentation(
        string ItemName
      )
    {
      var Item = m_Items.First(i => i.Name == ItemName); ;

      if (Item == null)
        return string.Empty;

      return $"{Item}\n{Item.Documentation}";
    }

    public bool HasFunctionInfo(
        string _FunctionName
      )
    {
      return m_Items.Any(Item => Item is FunctionDataItem && (Item as FunctionDataItem).Name == _FunctionName);
    }

    private void TryLoadBuiltinsInfo()
    {
      try
      {
        foreach (var Variable in JsonSerializer.Deserialize<List<VariableDataItem>>(File.ReadAllText(CodeDatabaseConstants.BUILTIN_VARIABLES_INFO_PATH)))
        {
          Variable.IsBuiltIn = true;

          m_Items.Add(Variable);
        }

        foreach (var Function in JsonSerializer.Deserialize<List<FunctionDataItem>>(File.ReadAllText(CodeDatabaseConstants.BUILTIN_FUNCTIONS_INFO_PATH)))
        {
          Function.IsBuiltIn = true;

          m_Items.Add(Function);
        }
      }
      catch (Exception _Exception)
      {
        // TODO: Log
      }
    }
    
    private List<CodeDataItem> m_Items = new List<CodeDataItem>();
  }
}
