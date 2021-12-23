
namespace SquirrelDebugEngine
{
  public class AddressRange
  {
    public ulong Start;
    public ulong End;

    public AddressRange()
    {
      // Empty
    }

    public AddressRange(
        ulong _Start, 
        ulong _End
      )
    {
      Start = _Start;
      End   = _End;
    }

    public AddressRange(
        AddressRange _Other
      )
    {
      Start = _Other.Start;
      End   = _Other.End;
    }

    public bool In(
        ulong _Address
      )
    {
      return _Address >= Start && _Address < End;
    }
  }
}
