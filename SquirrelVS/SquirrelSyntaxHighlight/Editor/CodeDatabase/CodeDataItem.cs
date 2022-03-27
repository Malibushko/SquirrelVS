using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight.Editor.CodeDatabase
{
  public class CodeDataItem
  {
    public bool IsBuiltIn { get; set; }
  }

  public class ParameterDataItem
  {
    public string Name { get; set; }
    public string Documentation { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }

  public class FunctionDataItem : CodeDataItem
  {
    public string Name
    {
      get; set;
    }
    
    public string Documentation
    {
      get; set;
    }
   
    public List<ParameterDataItem> Parameters
    {
      get; set;
    }

    public bool Optional
    {
      get; set;
    }

    public override string ToString()
    {
      return $"{Name}({string.Join(",", Parameters)})";
    }
  };

  public class VariableDataItem : CodeDataItem
  {
    public string Name { get; set; }
    public string Documentation { get; set; }

    public override string ToString()
    {
      return Name;
    }
  }
}
