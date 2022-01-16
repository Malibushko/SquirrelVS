using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelLexer
  {
    public SnapshotSpan               Snapshot;

    public int                        CurrentLexPosition;
    public int                        LastLexPosition;

    public int                        LastTokenLine;
    public int                        CurrentLine;
    public int                        CurrentColumn;
    public int                        PreviousToken;
    public int                        CurrentToken;
    
    public int                        CurrentData;

    public float                      FloatValue;
    public long                       NumberValue;
    public string                     StringValue;

    public Dictionary<string, EToken> StringToKeyword;
    public Dictionary<EToken, string> KeywordToString;

    public SquirrelLexer(
        SnapshotSpan _Snapshot
      )
    {
      Snapshot = _Snapshot;

      StringToKeyword = new Dictionary<string, EToken>();

      LastTokenLine = CurrentLine = 1;

      PreviousToken      = CurrentToken = -1;
      CurrentColumn      = 0;
      CurrentLexPosition = 0;

      StringToKeyword.Add("while", EToken.While);
      StringToKeyword.Add("do", EToken.Do);
      StringToKeyword.Add("if", EToken.If);
      StringToKeyword.Add("else", EToken.Else);
      StringToKeyword.Add("break", EToken.Break);
      StringToKeyword.Add("continue", EToken.Continue);
      StringToKeyword.Add("return", EToken.Return);
      StringToKeyword.Add("null", EToken.Null);
      StringToKeyword.Add("function", EToken.Function);
      StringToKeyword.Add("local", EToken.Local);
      StringToKeyword.Add("for", EToken.For);
      StringToKeyword.Add("foreach", EToken.ForEach);
      StringToKeyword.Add("in", EToken.In);
      StringToKeyword.Add("typeof", EToken.Typeof);
      StringToKeyword.Add("delegate", EToken.Delegate);
      StringToKeyword.Add("delete", EToken.Delete);
      StringToKeyword.Add("try", EToken.Try);
      StringToKeyword.Add("catch", EToken.Catch);
      StringToKeyword.Add("throw", EToken.Throw);
      StringToKeyword.Add("clone", EToken.Clone);
      StringToKeyword.Add("yield", EToken.Yield);
      StringToKeyword.Add("resume", EToken.Resume);
      StringToKeyword.Add("switch", EToken.Switch);
      StringToKeyword.Add("case", EToken.Case);
      StringToKeyword.Add("default", EToken.Default);
      StringToKeyword.Add("this", EToken.This);
      StringToKeyword.Add("parent", EToken.Parent);
      StringToKeyword.Add("class", EToken.Class);
      StringToKeyword.Add("extends", EToken.Extends);
      StringToKeyword.Add("constructor", EToken.Constructor);
      StringToKeyword.Add("instanceof", EToken.IntstanceOf);
      StringToKeyword.Add("vargc", EToken.VarArgc);
      StringToKeyword.Add("vargv", EToken.VarArgv);
      StringToKeyword.Add("true", EToken.True);
      StringToKeyword.Add("false", EToken.False);
      StringToKeyword.Add("static", EToken.Static);
      StringToKeyword.Add("const", EToken.Const);

      KeywordToString = new Dictionary<EToken, string>();

      foreach (var KeyValue in StringToKeyword)
        KeywordToString.Add(KeyValue.Value, KeyValue.Key);
    
      Next();
    }
   
    public EToken Lex()
    {
      LastTokenLine   = CurrentLine;
      LastLexPosition = CurrentLexPosition - 1;

      while (CurrentData != ParsingConstants.EOB)
      {
        switch (CurrentData)
        {
          case '\t':
          case '\r':
          case ' ':
            Next();
            LastLexPosition = CurrentLexPosition - 1;
            continue;
          case '\n':
            CurrentLine++;
            PreviousToken = CurrentData;
            CurrentData = '\n';
            Next();
            LastLexPosition = CurrentLexPosition - 1;
            CurrentColumn = 1;
            continue;
          case '/':
            Next();
            switch (CurrentData)
            {
              case '*':
                Next();

                LexBlockComment();

                continue;
              case '/':
                do { Next(); } while (CurrentData != '\n' && CurrentData != ParsingConstants.EOB);
                continue;
              case '=':
                Next();
                return UpdateToken(EToken.DivEqual);
              case '>':
                Next();
                return UpdateToken(EToken.AttributeClose);
              default:
                return UpdateToken('/');
            }
          case '=':
            Next();
            if (CurrentData != '=')
              return UpdateToken('=');
            else
            {
              Next();
              return UpdateToken(EToken.Equal);
            }
          case '<':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.LessEqual);
            }
            else
            if (CurrentData == '-')
            {
              Next();
              return UpdateToken(EToken.NewSlot);
            }
            else
            if (CurrentData == '<')
            {
              Next();
              return UpdateToken(EToken.ShiftLeft);
            }
            else
            if (CurrentData == '/')
            {
              Next();
              return UpdateToken(EToken.AttributeOpen);
            }
            else
              return UpdateToken('<');
          case '>':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.GreatEqual);
            }
            else
            if (CurrentData == '>')
            {
              Next();
              return UpdateToken(EToken.ShiftRight);
            }
            else
              return UpdateToken('>');
          case '!':
            Next();
            if (CurrentData != '=')
              return UpdateToken('!');
            else
            {
              Next();
              return UpdateToken(EToken.NotEqual);
            }
          case '@':
          {
            int SType;
            Next();

            if (CurrentData != '"')
              Error("string expected");

            if ((SType = ReadString('"', true)) != -1)
              return UpdateToken((EToken)SType);

            Error("error parsing the string");

            goto case '"';
          }
          case '"':
          case '\'':
          {
            int SType;
            if ((SType = ReadString(CurrentData, false)) != -1)
              return UpdateToken((char)SType);

            Error("error parsing the string");

            goto case '{';
          }

          case '{':
          case '}':
          case '(':
          case ')':
          case '[':
          case ']':
          case ';':
          case ',':
          case '?':
          case '^':
          case '~':
            int Return = CurrentData;
            Next();
            return UpdateToken((char)Return);
          case '.':
            Next();
            if (CurrentData != '.')
              return UpdateToken('.');
            Next();
            if (CurrentData != '.')
              Error("invalid token '..'");
            Next();
            return UpdateToken(EToken.VarParams);
          case '&':
            Next();
            if (CurrentData != '&')
              return UpdateToken('&');
            else
            {
              Next();
              return UpdateToken(EToken.And);
            }
          case '|':
            Next();
            if (CurrentData != '|')
              return UpdateToken('|');
            else
            {
              Next();
              return UpdateToken(EToken.Or);
            }
          case ':':
            Next();
            if (CurrentData != ':')
              return UpdateToken(':');
            else
            {
              Next();
              return UpdateToken(EToken.DoubleColon);
            }
          case '*':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.MulEqual);
            }
            return UpdateToken('*');
          case '%':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.ModEqual);
            }
            return UpdateToken('%');
          case '-':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.MinusEqual);
            }
            else 
            if (CurrentData == '-')
            {
              Next();
              return UpdateToken(EToken.MinusMinus);
            }
            return UpdateToken('-');
          case '+':
            Next();
            if (CurrentData == '=')
            {
              Next();
              return UpdateToken(EToken.PlusEqual);
            }
            else if (CurrentData == '+')
            {
              Next();
              return UpdateToken(EToken.PlusPlus);
            }
            else
              return UpdateToken('+');
          case ParsingConstants.EOB:
            return 0;
          default:
          {
            if (Char.IsDigit((char)CurrentData))
              return UpdateToken(ReadNumber());
            else
            if (Char.IsLetterOrDigit((char)CurrentData) || CurrentData == '_')
              return UpdateToken((EToken)ReadID());
            else
            {
              var Current = (char)CurrentData;
              if (Char.IsControl(Current))
                Error("unexpected character(control)");
              Next();

              return UpdateToken(Current);
            }
          }
        }
      }

      return (EToken)ParsingConstants.EOB;
    }

    #region Service

    private void Error(
        string Message
      )
    {

    }
    private void Next(
        bool _UpdateColumn = true
      )
    {
      CurrentData = Read();

      if (_UpdateColumn)
        CurrentColumn++;
    }

    private EToken UpdateToken(
        char _NewToken
      )
    {
      return UpdateToken((EToken)_NewToken);
    }

    private EToken UpdateToken(
        EToken _NewToken
      )
    {
      PreviousToken   = CurrentToken;
      CurrentToken    = (int)_NewToken;

      return _NewToken;
    }
    private void LexBlockComment()
    {
      bool Done = false;

      while (!Done)
      {
        switch (CurrentData)
        {
          case '*':
          {
            Next();

            if (CurrentData == '/')
            {
              Done = true;
              Next();
            }

            continue;
          }
          case '\n':
            CurrentLine++;
            Next();
            continue;
          case ParsingConstants.EOB:
            Error("missing '*/' in comment");
            Next();
            return;
          default:
            Next();
            break;
        }
      }
    }

    private EToken GetIDType(
        string _Type
      )
    {
      if (StringToKeyword.TryGetValue(_Type, out EToken _Value))
        return _Value;

      return EToken.Identifier;
    }

    private bool IsOctalDigit(
        char _Digit
      )
    {
      return _Digit >= '0' && _Digit <= '7';
    }
    private bool IsXDigit(char c)
    {
      if ('0' <= c && c <= '9') return true;
      if ('a' <= c && c <= 'f') return true;
      if ('A' <= c && c <= 'F') return true;
      return false;
    }

    private bool IsExponent(char c)
    {
      return Char.ToUpper(c) == 'E';
    }

    private EToken ReadNumber()
    {
      ENumberType Type      = ENumberType.Int;
      var         FirstChar = CurrentData;
      var         Buffer    = new StringBuilder();
      
      Next();

      if (FirstChar == '0' && (Char.ToUpper((char)CurrentData) == 'X' || IsOctalDigit((char)CurrentData)))
      {
        if (IsOctalDigit((char)CurrentData))
        {
          Type = ENumberType.Octal;
          while (IsOctalDigit((char)CurrentData))
          {
            Buffer.Append((char)CurrentData);
            Next();
          }
          if (Char.IsDigit((char)CurrentData))
            Error("invalid octal number");
        }
        else
        {
          Next();
          Type = ENumberType.Hex;

          while (IsXDigit((char)CurrentData))
          {
            Buffer.Append((char)CurrentData);
            Next();
          }
          if (Buffer.Length > sizeof(ulong) * 2)
            Error("too many digits for an Hex number");
        }
      }
      else
      {
        Buffer.Append((char)FirstChar);

        while (CurrentData == '.'                  || 
               Char.IsDigit((char)CurrentData)     ||
               IsExponent((char)CurrentData))
        {
          if (CurrentData == '.' || IsExponent((char)CurrentData))
            Type = ENumberType.Float;

          if (IsExponent((char)CurrentData))
          {
            if (Type != ENumberType.Float)
              Error("invalid numeric format");
            
            Type = ENumberType.Scientific;
            
            Buffer.Append((char)CurrentData);
            Next();

            if (CurrentData == '+' || CurrentData == '-')
            {
              Buffer.Append((char)CurrentData);
              Next();
            }

            if (!Char.IsDigit((char)CurrentData))
              Error("Exponent Expected");
          }

          Buffer.Append((char)CurrentData);
          Next();
        }
      }

      switch (Type)
      {
        case ENumberType.Int:
          NumberValue = long.Parse(Buffer.ToString());
          return EToken.Integer;
        case ENumberType.Float:
          FloatValue = float.Parse(Buffer.ToString());
          return EToken.Float;
        case ENumberType.Hex:
          NumberValue = long.Parse(Buffer.ToString(), System.Globalization.NumberStyles.HexNumber);
          return EToken.Integer;
        case ENumberType.Scientific:
          NumberValue = long.Parse(Buffer.ToString(), System.Globalization.NumberStyles.AllowExponent);
          return EToken.Integer;
        case ENumberType.Octal:
          NumberValue = Convert.ToInt64(Buffer.ToString(), 8);
          return EToken.Integer;
      }

      return 0;
    }
    private int ReadID()
    {
      var TempBuffer = new StringBuilder();

      do
      {
        TempBuffer.Append((char)CurrentData);
        Next();
      } while (CurrentData == '_' || Char.IsLetterOrDigit((char)CurrentData));

      var TempString = TempBuffer.ToString();

      var ID = GetIDType(TempString);

      if (ID == EToken.Identifier || ID == EToken.Constructor)
        StringValue = TempString;
      
      return (int)ID;
    }

    private int ReadString(
        int  _Delimeter,
        bool _Verbatim
      )
    {
      var Buffer = new StringBuilder();

      Next();
      if (CurrentData == ParsingConstants.EOB)
        return -1;

      for (;;)
      {
        while (CurrentData != _Delimeter)
        {
          switch (CurrentData)
          {
            case ParsingConstants.EOB:
              Error("unfinished string");
              return -1;
            case '\n':
              if (!_Verbatim)
                Error("newline in a constant");
              Buffer.Append((char)CurrentData);
              Next();
              ++CurrentLine;
              break;
            default:
              Buffer.Append((char)CurrentData);
              Next();
              break;
          }
        }
        Next();

        if (_Verbatim && CurrentData == '"')
        {
          Buffer.Append((char)CurrentData);
          Next();
        }
        else
          break;
      }

      if ((char)_Delimeter == '\'')
      {
        if (Buffer.Length == 0)
          Error("empty constant");

        if (Buffer.Length > 1)
          Error("constant too long");

        NumberValue = Buffer[0];
        return (int)EToken.CharLiteral;
      }

      StringValue = Buffer.ToString();
      return (int)EToken.StringLiteral;
    }

    private char Read()
    {
      if (CurrentLexPosition >= Snapshot.Snapshot.Length)
        return ParsingConstants.EOB;

      return Snapshot.Snapshot[CurrentLexPosition++];
    }
    #endregion
  }
}
