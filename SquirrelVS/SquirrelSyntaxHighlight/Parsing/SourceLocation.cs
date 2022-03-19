using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SquirrelSyntaxHighlight.Infrastructure;

namespace SquirrelSyntaxHighlight.Parsing
{
  [DebuggerDisplay("({Line}, {Column})")]
  [Serializable]
  public struct SourceLocation : IComparable<SourceLocation>, IEquatable<SourceLocation>
  {
    private readonly int                  _Index;
    public static readonly SourceLocation None     = new SourceLocation(0, 16707566, 0, true);
    public static readonly SourceLocation Invalid  = new SourceLocation(0, 0, 0, true);
    public static readonly SourceLocation MinValue = new SourceLocation(0, 1, 1);

    [DebuggerStepThrough]
    public SourceLocation(
        int _Index, 
        int _Line, 
        int _Column
      )
    {
      ValidateLocation(_Index, _Line, _Column);

      this._Index = _Index;
      this.Line   = _Line;
      this.Column = _Column;
    }

    [DebuggerStepThrough]
    public SourceLocation(
        int _Line, 
        int _Column
      )
    {
      ValidateLocation(0, _Line, _Column);

      this._Index = -1;
      this.Line   = _Line;
      this.Column = _Column;
    }

    [DebuggerStepThrough]
    private static void ValidateLocation(
        int _Index, 
        int _Line, 
        int _Column
      )
    {
      if (_Index < 0)
        throw SourceLocation.ErrorOutOfRange((object)nameof(_Index), (object)0);
      if (_Line < 1)
        throw SourceLocation.ErrorOutOfRange((object)nameof(_Line), (object)1);
      if (_Column < 1)
        throw SourceLocation.ErrorOutOfRange((object)nameof(_Column), (object)1);
    }

    [DebuggerStepThrough]
    private static Exception ErrorOutOfRange(object _P0, object _P1) => (Exception)new ArgumentOutOfRangeException("{0} must be greater than or equal to {1}".FormatInvariant(_P0, _P1));

    private SourceLocation(
        int  _Index, 
        int  _Line, 
        int  _Column, 
        bool _NoChecks
      )
    {
      this._Index = _Index;
      this.Line   = _Line;
      this.Column = _Column;
    }

    public int Index
    {
      get
      {
        if (this._Index < 0)
          throw new InvalidOperationException("Index is not valid");

        return this._Index;
      }
    }

    public int Line { get; }

    public int Column { get; }

    public static bool operator ==(SourceLocation _Left, SourceLocation _Right) => _Left.Line == _Right.Line && _Left.Column == _Right.Column;

    public static bool operator !=(SourceLocation _Left, SourceLocation _Right) => _Left.Line != _Right.Line || _Left.Column != _Right.Column;

    public static bool operator <(
        SourceLocation _Left, 
        SourceLocation _Right
      )
    {
      if (_Left.Line < _Right.Line)
        return true;

      return _Left.Line == _Right.Line && _Left.Column < _Right.Column;
    }

    public static bool operator >(
        SourceLocation _Left, 
        SourceLocation _Right
      )
    {
      if (_Left.Line > _Right.Line)
        return true;

      return _Left.Line == _Right.Line && _Left.Column > _Right.Column;
    }

    public static bool operator <=(
        SourceLocation _Left, 
        SourceLocation _Right
      )
    {
      if (_Left.Line < _Right.Line)
        return true;

      return _Left.Line == _Right.Line && _Left.Column <= _Right.Column;
    }

    public static bool operator >=(
        SourceLocation _Left, 
        SourceLocation _Right
      )
    {
      if (_Left.Line > _Right.Line)
        return true;
      return _Left.Line == _Right.Line && _Left.Column >= _Right.Column;
    }

    public static int Compare(
        SourceLocation _Left, 
        SourceLocation _Right
      )
    {
      if (_Left < _Right)
        return -1;

      return _Left > _Right ? 1 : 0;
    }

    public bool IsValid => this.Line > 0 && this.Column > 0;

    public SourceLocation AddColumns(
        int _Columns
      )
    {
      if (!this.IsValid)
        return SourceLocation.Invalid;

      int Index   = this._Index;
      int Column1 = this.Column;
      int Column2;

      if (_Columns > int.MaxValue - this.Column)
      {
        Column2 = int.MaxValue;

        if (Index >= 0)
          Index = int.MaxValue;
      }
      else if (_Columns == int.MinValue || _Columns < 0 && this.Column <= -_Columns)
      {
        Column2 = 1;

        if (Index >= 0)
          Index += 1 - this.Column;
      }
      else
      {
        Column2 = Column1 + _Columns;

        if (Index >= 0)
          Index += _Columns;
      }

      return Index < 0 ? new SourceLocation(this.Line, Column2) : new SourceLocation(Index, this.Line, Column2);
    }

    public override bool Equals(object Object) => Object is SourceLocation sourceLocation && sourceLocation.Line == this.Line && sourceLocation.Column == this.Column;

    public override int GetHashCode() => this.Line << 16 ^ this.Column;

    public override string ToString() => string.Format("({0}, {1})", (object)this.Line, (object)this.Column);

    public bool Equals(SourceLocation _Other) => _Other.Line == this.Line && _Other.Column == this.Column;

    public int CompareTo(SourceLocation _Other)
    {
      int Number = this.Line.CompareTo(_Other.Line);

      return Number != 0 ? Number : this.Column.CompareTo(_Other.Column);
    }
  }
}
