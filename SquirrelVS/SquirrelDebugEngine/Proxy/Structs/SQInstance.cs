using System;
using Microsoft.VisualStudio.Debugger;
using System.Collections.Generic;

namespace SquirrelDebugEngine.Proxy
{
  [StructProxy(StructName = "SQInstance")]
  internal class SQInstance : StructProxy, ISQObject, IVisualizableObject
  {
    public class Fields
    {
#pragma warning disable 0649

      public StructField<PointerProxy<SQClass>>   _class;
      public StructField<PointerProxy>            _userpointer;
      public StructField<ArrayProxy<SQObjectPtr>> _values;

#pragma warning restore 0649
    }

    private readonly Fields m_Fields;

    public SQInstance(
        DkmProcess _Process,
        ulong      _Address
      ) : 
      base(_Process, _Address)
    {
      InitializeStruct(this, out m_Fields);
    }

    public PointerProxy<SQClass> Class
    {
      get
      {
        return GetFieldProxy(m_Fields._class);
      }
    }

    public PointerProxy UserPointer
    {
      get
      {
        return GetFieldProxy(m_Fields._userpointer);
      }
    }

    public ArrayProxy<SQObjectPtr> Values
    {
      get
      {
        return GetFieldProxy(m_Fields._values);
      }
    }

    public FieldDataItem[] GetChildren()
    {
      List<FieldDataItem> Items = new List<FieldDataItem>();

      Items.Add(new FieldDataItem
      {
        Name         = "[Class]",
        Object = Class.Read()
      });

      var ClassMembers    = Class.Read().Members.Read();
      var NodesCount      = ClassMembers.NodeNumber;
      var Nodes           = ClassMembers.Nodes.Read();

      var Methods = new List<FieldDataItem>();
      var Members = new List<FieldDataItem>();

      for (int i = 0; i < NodesCount; i++)
      {
        var Node = Nodes[i];

        if (Node.Key.Type != SquirrelVariableInfo.Type.Null && 
            Node.Value.Type == SquirrelVariableInfo.Type.Integer)
        {
          var NodeFlags = (ulong)(Int64)Node.Value.Value;

          if (IsKeyField(NodeFlags))
          {
            Members.Add(new FieldDataItem
            {
              Name = (Node.Key.Value as SQString).Read(),
              Object = Values[(long)GetMemberIndex(NodeFlags)]
            });
          }
          else
          if (IsKeyMethod(NodeFlags))
          {
            Methods.Add(new FieldDataItem
            {
              Name   = (Node.Key.Value as SQString).Read(),
              Object = Class.Read().Methods[(long)GetMemberIndex(NodeFlags)].Value
            });
          }
        }
      }
      
      Methods.Sort((Left, Right) => Left.Name.CompareTo(Right.Name));
      Members.Sort((Left, Right) => Left.Name.CompareTo(Right.Name));

      Items.Add(new FieldDataItem
      {
        Name = "[Methods]",
        Object = new TableVisualizationProxy
        {
          Keys = Methods.ToArray()
        }
      });

      Items.Add(new FieldDataItem
      {
        Name = "[Members]",
        Object = new TableVisualizationProxy
        {
          Keys = Members.ToArray()
        }
      });

      return Items.ToArray();
    }

    public string GetDisplayType()
    {
      return SquirrelVariableInfo.Type.Instance.ToString();
    }

    public string GetDisplayNativeType()
    {
      return "SQInstance";
    }

    public string GetDisplayValue()
    {
      return "[Class instance]";
    }

    public DkmEvaluationFlags GetEvaluationFlags()
    {
      return SQObject.ExpandableEvaluationFlags;
    }

    private bool IsKeyField(
        ulong _FieldFlags
      )
    {
      ulong MEMBER_TYPE_FIELD = 0x02000000;

      return (_FieldFlags & MEMBER_TYPE_FIELD) != 0;
    }

    private bool IsKeyMethod(
        ulong _FieldFlags
      )
    {
      ulong MEMBER_TYPE_METHOD = 0x01000000;

      return (_FieldFlags & MEMBER_TYPE_METHOD) != 0;
    }

    private ulong GetMemberIndex(
        ulong _FieldFlags
      )
    {
      return _FieldFlags & 0x00FFFFFF;
    }

    public bool IsNativeExpression()
    {
      return false;
    }
  }
}
