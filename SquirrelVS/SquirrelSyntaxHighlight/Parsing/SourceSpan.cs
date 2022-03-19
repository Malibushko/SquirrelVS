using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SquirrelSyntaxHighlight.Infrastructure;

namespace SquirrelSyntaxHighlight.Parsing
{
  [DebuggerDisplay("({Start.Line}, {Start.Column})-({End.Line}, {End.Column})")]
  [Serializable]
  public struct SourceSpan : IComparable<SourceSpan>
  {
    public static readonly SourceSpan None    = new SourceSpan(SourceLocation.None, SourceLocation.None);
    public static readonly SourceSpan Invalid = new SourceSpan(SourceLocation.Invalid, SourceLocation.Invalid);

    [DebuggerStepThrough]
    public SourceSpan(
        SourceLocation _Start, 
        SourceLocation _End
      )
    {
      SourceSpan.ValidateLocations(_Start, _End);
      
      this.Start = _Start;
      this.End   = _End;
    }

    [DebuggerStepThrough]
    public SourceSpan(
        int _StartLine, 
        int _StartColumn, 
        int _EndLine, 
        int _EndColumn
      ) : this(new SourceLocation(_StartLine, _StartColumn), new SourceLocation(_EndLine, _EndColumn))
    {
      // Empty
    }

    [DebuggerStepThrough]
    private static void ValidateLocations(
        SourceLocation _Start, 
        SourceLocation _End
      )
    {
      if (_Start.IsValid && _End.IsValid)
      {
        if (_Start > _End)
          throw new ArgumentException("Start and End must be well ordered");
      }
      else if (_Start.IsValid || _End.IsValid)
        throw new ArgumentException("Start and End must both be valid or both invalid");
    }

    public SourceLocation Start { get; }

    public SourceLocation End { get; }

    public bool IsValid => this.Start.IsValid && this.End.IsValid;

    public SourceSpan Union(
        SourceSpan _Other
      )
    {
      SourceLocation sourceLocation = _Other.Start;
      int line1 = sourceLocation.Line;
      sourceLocation = this.Start;
      int line2 = sourceLocation.Line;
      int line3 = Math.Min(line1, line2);
      sourceLocation = _Other.Start;
      int column1 = sourceLocation.Column;
      sourceLocation = this.Start;
      int column2 = sourceLocation.Column;
      int num = Math.Min(column1, column2);
      sourceLocation = _Other.End;
      int line4 = sourceLocation.Line;
      sourceLocation = this.End;
      int line5 = sourceLocation.Line;
      int line6 = Math.Max(line4, line5);
      sourceLocation = _Other.End;
      int column3 = sourceLocation.Column;
      sourceLocation = this.End;
      int column4 = sourceLocation.Column;
      int column5 = Math.Max(column3, column4);
      int column6 = num;
      return new SourceSpan(new SourceLocation(line3, column6), new SourceLocation(line6, column5));
    }

    public static bool operator ==(SourceSpan _Left, SourceSpan _Right) => _Left.Start == _Right.Start && _Left.End == _Right.End;

    public static bool operator !=(SourceSpan _Left, SourceSpan _Right) => _Left.Start != _Right.Start || _Left.End != _Right.End;

    public int CompareTo(SourceSpan other)
    {
      SourceLocation start1 = this.Start;
      int line1 = start1.Line;
      start1 = other.Start;
      int line2 = start1.Line;
      if (line1 < line2)
        return -1;
      SourceLocation start2 = this.Start;
      int line3 = start2.Line;
      start2 = other.Start;
      int line4 = start2.Line;
      if (line3 != line4)
        return 1;
      SourceLocation start3 = this.Start;
      int column1 = start3.Column;
      start3 = other.Start;
      int column2 = start3.Column;
      if (column1 < column2)
        return -1;
      SourceLocation start4 = this.Start;
      int column3 = start4.Column;
      start4 = other.Start;
      int column4 = start4.Column;
      return column3 != column4 ? 1 : 0;
    }

    public override bool Equals(object _Object) => _Object is SourceSpan sourceSpan && this.Start == sourceSpan.Start && this.End == sourceSpan.End;

    public override string ToString() => "{0} - {1}".FormatInvariant((object)this.Start, (object)this.End);

    public override int GetHashCode()
    {
      SourceLocation sourceLocation = this.Start;
      int column = sourceLocation.Column;
      sourceLocation = this.End;
      int num1 = sourceLocation.Column << 7;
      int num2 = column ^ num1;
      sourceLocation = this.Start;
      int num3 = sourceLocation.Line << 14;
      int num4 = num2 ^ num3;
      sourceLocation = this.End;
      int num5 = sourceLocation.Line << 23;
      return num4 ^ num5;
    }
  }
}
