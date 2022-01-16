using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQTable")]
  internal class SQTable : StructProxy, ISQObject, IVisualizableObject
  {
    private class Fields
    {
#pragma warning disable 0649

      public StructField<PointerProxy<ArrayProxy<HashNode>>> _nodes;
      public StructField<Int64Proxy>                         _numofnodes;
      public StructField<Int64Proxy>                         _usednodes;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQTable(
          DkmProcess process,
          ulong address
        ) :
      base(process, address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public PointerProxy<ArrayProxy<HashNode>> Nodes
    {
      get
      {
        return GetFieldProxy(m_Fields._nodes);
      }
    }

    public Int64 NodeNumber
    {
      get
      {
        return GetFieldProxy(m_Fields._numofnodes).Read();
      }
    }

    public Int64 UsedNodeNumber
    {
      get
      {
        return GetFieldProxy(m_Fields._usednodes).Read();
      }
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      var NodeItems  = Nodes.Read();
      var TotalNodes = NodeNumber;
      
      for (int i = 0; i < ((int)TotalNodes); i++)
      {
        var Node = NodeItems[i];

        if (Node.Key.Type != SquirrelVariableInfo.Type.Null)
        {
          Items.Add(new FieldDataItem()
          {
            Name         = $"[{Node.Key.GetDisplayValue()}]",
            Object = Node.Value
          });
        }
      }
      
      Debug.Assert(Items.Count == UsedNodeNumber);

      return Items.ToArray();
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Table.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQTable";
    }

    public string GetDisplayValue()
    {
      return $"Table [{UsedNodeNumber} key(s)]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return UsedNodeNumber > 0 ? SQObject.ExpandableEvaluationFlags : SQObject.DefaultEvaluationFlags;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
