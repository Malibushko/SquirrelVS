using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
  public enum ASTNodeType
  {
    Invalid = -1,
    SourceFile,
    IfStatement,
    LocalVariableStatement,
    VariableDeclaration,
    Identifier,
    Literal,
    BinaryOperator,
    MethodDefinition,
    FunctionExpression,
    FunctionDeclaration,
    Parameter,
    AssignmentExpression,
    ClassDeclaration,
    ClassBody,
    BlockStatement,
    UnaryExpression,
    StringLiteral
  }

  public class NodeSpan
  {
    public string Type;
    public Span   Span;

    public NodeSpan(
        string _Type, 
        Span   _Span
      )
    {
      Type = _Type;
      Span = _Span;
    }
  }

  public class ASTNodeBase
  {
    public ASTNodeType       Type;
    public int               StartPosition;
    public int               EndPosition;
    public List<ASTNodeBase> Children = new List<ASTNodeBase>();

    public virtual NodeSpan TryGetSpan()
    {
      switch (Type)
      {
        case ASTNodeType.Invalid:
        case ASTNodeType.SourceFile:
          return null;
      }

      return null;
    }
  }
}
