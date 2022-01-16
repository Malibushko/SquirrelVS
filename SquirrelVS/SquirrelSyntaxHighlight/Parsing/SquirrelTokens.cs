using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelSyntaxHighlight
{
  public static class ParsingConstants
  {
    public const char EOB = (char)0;
  }
  
  public enum ENumberType
  {
    Int,
    Float,
    Hex,
    Scientific,
    Octal
  }

  public enum EToken
  { 
    Identifier = 258,
    StringLiteral,
    CharLiteral,
    Integer,
    Float,
    Delegate,
    Delete,
    Equal,
    NotEqual,
    LessEqual,
    GreatEqual,
    Switch,
    Arrow,
    And,
    Or,
    If,
    Else,
    While,
    Break,
    For,
    Do,
    Null,
    ForEach,
    In,
    NewSlot,
    Modulo,
    Local,
    Clone,
    Function,
    Return,
    Typeof,
    Uminus,
    PlusEqual,
    PlusPlus,
    MinusEqual,
    MinusMinus,
    Continue,
    Yield,
    Try,
    Catch,
    Throw,
    ShiftLeft,
    ShiftRight,
    Resume,
    DoubleColon,
    Case,
    Default,
    UShiftRight,
    Class,
    Extends,
    Constructor,
    IntstanceOf,
    VarParams,
    VarArgc,
    VarArgv,
    This,
    Parent,
    True,
    False,
    MulEqual,
    DivEqual,
    ModEqual,
    AttributeOpen,
    AttributeClose,
    Static,
    Enum,
    Const,
    Comment
  }
}
