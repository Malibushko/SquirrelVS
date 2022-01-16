using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
  public class AST : IEnumerable<ASTNodeBase>
  {

    private ASTNodeBase            Root;
    private Stack<ExpressionState> StatesStack;
    private Stack<object>          ValueStack;

    private class ExpressionState
    {
      public enum DerefState
      {
        NO_DEREF,
        FIELD
      }

      public bool        IsClassOrDelete = false;
      public bool        IsFuncArg       = false;
      public bool        IsFreeVar       = false;
      public DerefState  Deref           = DerefState.NO_DEREF;
    }

    public AST(
        int _Start, 
        int _End
      )
    {
      Root = new ASTNodeBase
      {
        Type          = ASTNodeType.SourceFile,
        StartPosition = _Start,
        EndPosition   = _End,
        Children      = new List<ASTNodeBase>()
      };

      StatesStack = new Stack<ExpressionState>();
      ValueStack  = new Stack<object>();
    }

    public void Feed(
        SquirrelLexer _Lexer,
        EToken        _Token
      )
    {
      Feed(_Lexer, _Token, Root);
    }

    public void Feed(
        SquirrelLexer _Lexer,
        EToken        _Token,
        ASTNodeBase   _Root
      )
    {
      try
      {
        switch (_Token)
        {
          case (EToken)';':
            break;
          case EToken.Local:
            LocalDeclStatement(_Lexer, _Root);
            break;
          case EToken.Class:
            ClassStatement(_Lexer, _Root);
            break;
          case EToken.Function:
            FunctionStatement(_Lexer, _Root);
            break;
        }
      } catch (Exception _Exception)
      {

      }
    }

    public List<NodeSpan> GetSpans(
        int _Start,
        int _End
      )
    {
      List<NodeSpan> Spans = new List<NodeSpan>();

      foreach (var Node in this)
      {
       // if (Node.StartPosition >= _Start && Node.EndPosition <= _End)
      //  {
          var Span = Node.TryGetSpan();

          if (Span != null)
            Spans.Add(Span);
    //    }
      }

      return Spans;
    }

    public List<NodeSpan> GetSpans()
    {
      return GetSpans(Root.StartPosition, Root.EndPosition);
    }
    
    #region IEnumerator

    public IEnumerator<ASTNodeBase> GetEnumerator()
    {
      Queue<ASTNodeBase> Queue = new Queue<ASTNodeBase>();

      Queue.Enqueue(Root);

      while (Queue.Count > 0)
      {
        ASTNodeBase Node = Queue.Dequeue();

        foreach (var Child in Node.Children)
          Queue.Enqueue(Child);

        yield return Node;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Build Utilities

    void Error(
        string _Error
      )
    {
      throw new Exception(_Error);
    }
    
    private void PushExpressionState()
    {
      StatesStack.Push(new ExpressionState());
    }

    private ExpressionState PopExpressionState()
    {
      return StatesStack.Pop();
    }
    
    private void Statement(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      Feed(_Lexer, _Lexer.Lex(), _Root);
    }

    private void LocalDeclStatement(
          SquirrelLexer _Lexer,
          ASTNodeBase   _Root
        )
    {
      NodeLocalVariableDeclaration Declarations = new NodeLocalVariableDeclaration()
      {
        StartPosition = _Lexer.LastLexPosition
      };

      do
      {
        _Lexer.Lex();
        
        var VariableDeclaration = new NodeVariableDeclaraction();

        VariableDeclaration.Children.Add(new NodeIdentifier
        {
          StartPosition = _Lexer.LastLexPosition,
          EndPosition   = _Lexer.CurrentLexPosition - 1,
          Name          = Expect(_Lexer, EToken.Identifier) as string
        });

        if (_Lexer.CurrentToken == '=')
        {
          _Lexer.Lex();

          Expression(_Lexer, VariableDeclaration);
        }

        Declarations.Children.Add(VariableDeclaration);

      } while (_Lexer.CurrentToken == ',');

      Declarations.EndPosition = _Lexer.CurrentLexPosition - 1;

      _Root.Children.Add(Declarations);
    }

    private ExpressionState Expression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root,
        bool          _IsFuncArg = false
      )
    {
      PushExpressionState();
      var State = StatesStack.Peek();

      State.IsClassOrDelete = false;
      State.IsFuncArg       = _IsFuncArg;

      LogicalOrExp(_Lexer, _Root);

      return PopExpressionState();
    }

    private object Expect(
        SquirrelLexer _Lexer,
        EToken        _Token
      )
    {
      var CurrentToken = (EToken)_Lexer.CurrentToken;

      if (CurrentToken != _Token)
      {
        if (CurrentToken == EToken.Constructor && CurrentToken == EToken.Identifier)
        {
          // do nothing
        }
        else
        {
          string ExpectTypename = _Token.ToString();

          if ((int)_Token > 255)
          {
            switch (_Token)
            {
              case EToken.Identifier:
                ExpectTypename = "IDENTIFIER";
                break;
              case EToken.StringLiteral:
                ExpectTypename = "STRING_LITERAL";
                break;
              case EToken.Integer:
                ExpectTypename = "INTEGER";
                break;
              case EToken.Float:
                ExpectTypename = "FLOAT";
                break;
            }

            Error($"expected '{ExpectTypename}'");
          }
          else
          {
            Error($"expected '{(char)_Token}'");
          }
        }
      }

      object Return = null;

      switch (_Token)
      {
        case EToken.Identifier:
        case EToken.StringLiteral:
          Return = _Lexer.StringValue;
          break;
        case EToken.Integer:
          Return = _Lexer.NumberValue;
          break;
        case EToken.Float:
          Return = _Lexer.FloatValue;
          break;
      }

      _Lexer.Lex();

      return Return;
    }
    
    private void LogicalOrExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      LogicalAndExp(_Lexer, _Root);

      for (; ;)
      {
        if ((EToken)_Lexer.CurrentToken == EToken.Or)
        {
          var StartPosition = _Lexer.LastLexPosition;
        }
        else
          return;
      }
    }

    private void LogicalAndExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      BitwiseOrExp(_Lexer, _Root);
    }

    private void BitwiseOrExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      BitwiseXorExp(_Lexer, _Root);
    }

    private void BitwiseXorExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      BitwiseAndExp(_Lexer, _Root);
    }

    private void BitwiseAndExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      CompExp(_Lexer, _Root);
    }

    private void CompExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      ShiftExp(_Lexer, _Root);
    }

    private void ShiftExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      PlusExp(_Lexer, _Root);
    }

    private void PlusExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      MultExp(_Lexer, _Root);
    }

    private void MultExp(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      PrefixedExpression(_Lexer, _Root);
    }

    private void PrefixedExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      int Position = Factor(_Lexer, _Root);

      for (;;)
      {
        switch (_Lexer.CurrentToken)
        {
          case '.':
          {
            Position = -1;

            _Lexer.Lex();

            if ((EToken)_Lexer.CurrentToken == EToken.Parent)
            {
              _Lexer.Lex();

              if (!NeedGet(_Lexer, _Root))
                Error("parent cannot be set");
            }
            else
            {
              // TODO: add nodes
            }

            StatesStack.Peek().Deref     = ExpressionState.DerefState.FIELD;
            StatesStack.Peek().IsFreeVar = false;

            break;
          }
          case '[':
          {
            if (_Lexer.PreviousToken == '\n')
              Error("cannot break deref/or comma needed after [exp]=exp slot declaration");

            _Lexer.Lex();
            
            Expression(_Lexer, _Root);
            
            Expect(_Lexer, (EToken)']');

            Position = -1;

            if (NeedGet(_Lexer, _Root))
            {
              // TODO: add nodes
            }

            StatesStack.Peek().Deref     = ExpressionState.DerefState.FIELD;
            StatesStack.Peek().IsFreeVar = false;

            break;
          }
          case (int)EToken.PlusPlus:
          case (int)EToken.MinusMinus:
          {
            if (StatesStack.Peek().Deref != ExpressionState.DerefState.NO_DEREF &&
                !IsEndOfStatement(_Lexer, _Root))
            {
              var Token = _Lexer.CurrentToken;
              
              _Lexer.Lex();
              // TODO: add nodes
            }
            return;
          }
          case '(':
          {
            if (StatesStack.Peek().Deref != ExpressionState.DerefState.NO_DEREF)
            {
              if (Position < 0)
              {
                // TODO: Add call node
              }
            }

            StatesStack.Peek().Deref = ExpressionState.DerefState.NO_DEREF;
            
            _Lexer.Lex();

            FunctionCallArgs(_Lexer, _Root);

            break;
          }
          default:
            return;
        }
      }
    }

    private int Factor(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      StatesStack.Peek().Deref = ExpressionState.DerefState.NO_DEREF;

      switch ((EToken)_Lexer.CurrentToken)
      {
        case EToken.StringLiteral:
        {
          _Root.Children.Add(new NodeStringLiteral
          {
            Value         = _Lexer.StringValue,
            StartPosition = _Lexer.LastLexPosition,
            EndPosition   = _Lexer.CurrentLexPosition - 1
          });

          _Lexer.Lex();

          break;
        }
        case EToken.VarArgc:
          break;
        case EToken.VarArgv:
          break;
        case EToken.Identifier:
        case EToken.Constructor:
        case EToken.This:
        {
          string ID = string.Empty;
          
          switch ((EToken)_Lexer.CurrentToken)
          {
            case EToken.Identifier:
              ID = _Lexer.StringValue;
              break;
            case EToken.This:
              ID = "this";
              break;
            case EToken.Constructor:
              ID = "constructor";
              break;
          }

          _Lexer.Lex();

        // TODO:
        //  if (IsLocalVariable())
        //  {
        //
        //  }
        //  else
        //  if (IsConstant())
        //  {
        //
        //  }
        //  else
          {
            ValueStack.Push(ID);
            StatesStack.Peek().Deref = ExpressionState.DerefState.FIELD;
          }
          break;
        }
        case EToken.Parent:
          _Lexer.Lex();
          break;
        case EToken.DoubleColon:
          _Lexer.CurrentToken = '.';
          break;
        case EToken.Null:
          _Lexer.Lex();
          break;
        case EToken.Integer:
          ValueStack.Push(_Lexer.NumberValue);
          _Lexer.Lex();
          break;
        case EToken.Float:
          ValueStack.Push(_Lexer.FloatValue);
          _Lexer.Lex();
          break;
        case EToken.True:
          ValueStack.Push(true);
          _Lexer.Lex();
          break;
        case EToken.False:
          ValueStack.Push(false); 
          _Lexer.Lex();
          break;
        case (EToken)'[':
          _Lexer.Lex();
          while (_Lexer.CurrentToken != ']')
          {
            Expression(_Lexer, _Root);
            if (_Lexer.CurrentToken == ',')
              _Lexer.Lex();
          }
          
          _Lexer.Lex();

          break;
        case (EToken)'{':
          _Lexer.Lex();
          ParseTableOrClass(_Lexer, _Root, ',');
          break;
        case EToken.Function:
          FunctionExpression(_Lexer, _Root);
          break;
        case EToken.Class:
          _Lexer.Lex();
          ClassExpression(_Lexer, _Root, out _);
          break;
        case (EToken)'-':
        case (EToken)'!':
        case (EToken)'~':
        case EToken.Typeof:
        case EToken.Clone:
          UnaryExpression(_Lexer, _Root);
          break;
        case EToken.Delete:
          DeleteExpression(_Lexer, _Root);
          break;
        case EToken.Delegate:
          /* DelegateExpression(_Lexer, _Root); */
          break;
        case (EToken)'(':
          _Lexer.Lex();
          CommaExpression(_Lexer, _Root);
          Expect(_Lexer, (EToken)')');
          break;
        default:
          Error("expression expected");
          break;
      }

      return -1;
    }

    void ParseTableOrClass(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root,
        char          _Separator,
        EToken        _Terminator = (EToken)'}'
      )
    {
      while ((EToken)_Lexer.CurrentToken != _Terminator)
      {
        bool HasAttributes = false;
        bool IsStatic      = false;

        if (_Separator == ';')
        {
          if (_Lexer.CurrentToken == (int)EToken.AttributeOpen)
          {
            ParseTableOrClass(_Lexer, _Root, ',', EToken.AttributeClose);
            HasAttributes = true;
          }

          if (_Lexer.CurrentToken == (int)EToken.Static)
          {
            IsStatic = true;
            _Lexer.Lex();
          }
        }
        switch ((EToken)_Lexer.CurrentToken)
        {
          case EToken.Function:
          case EToken.Constructor:
          {
            var LastToken = (EToken)_Lexer.CurrentToken;

            _Lexer.Lex();

            string MethodName = (LastToken == EToken.Function ? Expect(_Lexer, EToken.Identifier) as string : "constructor");

            Expect(_Lexer, (EToken)'(');

            var MethodDefinition = new NodeMethodDefinition
            {
              Kind = LastToken == EToken.Function ? "method" : "constructor",
              Static = IsStatic
            };

            MethodDefinition.Children.Add(new NodeIdentifier
            {
              Name = MethodName
            });

            CreateFunction(_Lexer, MethodDefinition);

            _Root.Children.Add(MethodDefinition);

            break;
          }
          case (EToken)'[':
            break;
          default:
          {
            string Identifier = Expect(_Lexer, EToken.Identifier) as string;

            var AssignmentExpression = new NodeAssignmentExpression
            {
              Left = new NodeIdentifier
              {
                Name = Identifier,
                StartPosition = _Lexer.LastLexPosition,
                EndPosition = _Lexer.CurrentLexPosition - 1
              }
            };

            Expression(_Lexer, AssignmentExpression);

            _Root.Children.Add(AssignmentExpression);

            break;
          }
        }
        
        if (_Lexer.CurrentToken == _Separator)
          _Lexer.Lex();
      }

      _Lexer.Lex();
    }

    void CreateFunction(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      int DefaultParams = 0;

      while (_Lexer.CurrentToken != ')')
      {
        if ((EToken)_Lexer.CurrentToken == EToken.VarParams)
        {
          if (DefaultParams > 0)
            Error("function with default parameters cannot have variable number of parameters");

          _Lexer.Lex();
          if (_Lexer.CurrentToken != ')')
            Error("expected ')'");
          break;
        }
        else
        {
          var NameStartPosition = _Lexer.LastLexPosition;

          string ParameterName = Expect(_Lexer, EToken.Identifier) as string;
          
          _Root.Children.Add(new NodeParameter
          {
            Name          = ParameterName,
            StartPosition = NameStartPosition,
            EndPosition   = _Lexer.LastLexPosition,
          });

          if (_Lexer.CurrentToken == '=')
          {
            _Lexer.Lex();
            
            Expression(_Lexer, _Root);

            ++DefaultParams;
          }
          else
          {
            if (DefaultParams > 0)
              Error("expected '='");
          }
          if (_Lexer.CurrentToken == ',')
            _Lexer.Lex();
          else
          if (_Lexer.CurrentToken != ')')
            Error("expected ')' or ','");
        }
      }

      Expect(_Lexer, (EToken)')');

      Statement(_Lexer, _Root);
    }

    void ClassStatement(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      var StartPosition = _Lexer.LastLexPosition;

      ExpressionState State;
      _Lexer.Lex();
      PushExpressionState();
      
      StatesStack.Peek().IsClassOrDelete = true;
      StatesStack.Peek().IsFuncArg       = false;

      PrefixedExpression(_Lexer, _Root);

      State = PopExpressionState();

      if (State.Deref == ExpressionState.DerefState.NO_DEREF)
      {
        Error("invalid class name");
      }
      else 
      if (State.Deref == ExpressionState.DerefState.FIELD)
      {
        var ClassNode = new NodeClassDeclaration();

        ClassNode.Children.Add(new NodeIdentifier
        {
          Name = ValueStack.Pop() as string,
          StartPosition = StartPosition
        });

        var ClassBody = new NodeClassBody
        {
          StartPosition = _Lexer.CurrentLexPosition
        };

        ClassExpression(_Lexer, ClassBody, out Span _Span);

        ClassBody.StartPosition = _Span.Start;
        ClassBody.EndPosition   = _Span.End;

        ClassNode.EndPosition = _Span.End;

        ClassNode.Children.Add(ClassBody);
      }
      else
      {
        Error("cannot create a class in a local with the syntax(class <local>)");
      }
    }

    void FunctionStatement(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      var StartPosition = _Lexer.LastLexPosition;

      _Lexer.Lex();

      var    NameStartPosition = _Lexer.LastLexPosition;
      string ID                = Expect(_Lexer, EToken.Identifier) as string;
      
      while ((EToken)_Lexer.CurrentToken == EToken.DoubleColon)
      {
        _Lexer.Lex();

        NameStartPosition = _Lexer.LastLexPosition;
        ID                = Expect(_Lexer, EToken.Identifier) as string;
      }

      var Function = new NodeFunctionExpression()
      {
        StartPosition = StartPosition
      };

      Function.Children.Add(new NodeIdentifier
      {
        Name          = ID,
        StartPosition = NameStartPosition,
        EndPosition   = _Lexer.LastLexPosition
      });
      
      Expect(_Lexer, (EToken)'(');

      CreateFunction(_Lexer, Function);

      Function.EndPosition = _Lexer.CurrentLexPosition - 1;

      _Root.Children.Add(Function);
    }

    void ClassExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root,
        out Span      _Span
      )
    {
      if ((EToken)_Lexer.CurrentToken == EToken.Extends)
      {
        _Lexer.Lex();
        Expression(_Lexer, _Root);
      }

      if ((EToken)_Lexer.CurrentToken == EToken.AttributeOpen)
      {
        _Lexer.Lex();
        ParseTableOrClass(_Lexer, _Root, ',', EToken.AttributeClose);
      }

      var StartPosition = _Lexer.CurrentLexPosition - 1;

      Expect(_Lexer, (EToken)'{');

      ParseTableOrClass(_Lexer, _Root, ';');

      var EndPosition = _Lexer.CurrentLexPosition - 1;

      _Span = new Span(StartPosition, EndPosition);
    }

    void FunctionExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      _Lexer.Lex();
      Expect(_Lexer, (EToken)'(');
      CreateFunction(_Lexer, _Root);
    }

    void UnaryExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      _Lexer.Lex();
      PrefixedExpression(_Lexer, _Root);
    }

    void DeleteExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      var StartPosition = _Lexer.LastLexPosition;

      _Lexer.Lex();
      
      PushExpressionState();
      
      StatesStack.Peek().IsClassOrDelete = true;
      StatesStack.Peek().IsFuncArg       = false;
      
      PrefixedExpression(_Lexer, _Root);
      
      var State = PopExpressionState();

      if (State.Deref == ExpressionState.DerefState.NO_DEREF)
        Error("can't delete an expression");

      if (State.Deref == ExpressionState.DerefState.FIELD)
      {
        _Root.Children.Add(new NodeUnaryExpression
        {
          Operator      = "delete",
          StartPosition = StartPosition,
          EndPosition   = _Lexer.LastLexPosition
        });
      }
      else
      {
        Error("cannot delete a local");
      }
    }

    void CommaExpression(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      for (Expression(_Lexer, _Root); _Lexer.CurrentToken == ','; _Lexer.Lex(), CommaExpression(_Lexer, _Root))
      {
        // Empty
      }
    }

    bool NeedGet(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      switch ((EToken)_Lexer.CurrentToken)
      {
        case (EToken)'=':
        case (EToken)'(':
        case EToken.NewSlot:
        case EToken.PlusPlus:
        case EToken.MinusMinus:
        case EToken.PlusEqual:
        case EToken.MinusEqual:
        case EToken.MulEqual:
        case EToken.DivEqual:
        case EToken.ModEqual:
          return false;
      }

      return !StatesStack.Peek().IsClassOrDelete ||
             (StatesStack.Peek().IsClassOrDelete &&
             (_Lexer.CurrentToken == '.' || _Lexer.CurrentToken == '['));
    }

    void FunctionCallArgs(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      while (_Lexer.CurrentToken != ')')
      {
        Expression(_Lexer, _Root, true);

        if (_Lexer.CurrentToken == ',')
        {
          _Lexer.Lex();

          if (_Lexer.CurrentToken == ')')
            Error("expression expected, found ')'");
        }
      }

      _Lexer.Lex();
    }
    bool IsEndOfStatement(
        SquirrelLexer _Lexer,
        ASTNodeBase   _Root
      )
    {
      return _Lexer.PreviousToken == '\n'                 ||
             _Lexer.CurrentToken  == ParsingConstants.EOB ||
             _Lexer.CurrentToken  == '}'                  ||
             _Lexer.CurrentToken  == ';';
    }
    #endregion
  }
}
