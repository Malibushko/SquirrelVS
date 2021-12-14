using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDebugEngine
{
  internal enum SquirrelTypeMasks
  {
      SQOBJECT_REF_COUNTED  = 0x08000000,
      SQOBJECT_NUMERIC      = 0x04000000,
      SQOBJECT_DELEGABLE    = 0x02000000,
      SQOBJECT_CANBEFALSE   = 0x01000000,

      NULL           = 0x00000001,
      INTEGER        = 0x00000002,
      FLOAT          = 0x00000004,
      BOOL           = 0x00000008,
      STRING         = 0x00000010,
      TABLE          = 0x00000020,
      ARRAY          = 0x00000040,
      USERDATA       = 0x00000080,
      CLOSURE        = 0x00000100,
      NATIVE_CLOSURE = 0x00000200,
      GENERATOR      = 0x00000400,
      USER_POINTER   = 0x00000800,
      THREAD         = 0x00001000,
      FUNCPROTO      = 0x00002000,
      CLASS          = 0x00004000,
      INSTANCE       = 0x00008000,
      WEAKREF        = 0x00010000
  }
  public class SquirrelVariableInfo
  {
    public enum Type
    {
      Null          = SquirrelTypeMasks.NULL | SquirrelTypeMasks.SQOBJECT_CANBEFALSE,
      Integer       = SquirrelTypeMasks.INTEGER | SquirrelTypeMasks.SQOBJECT_NUMERIC | SquirrelTypeMasks.SQOBJECT_CANBEFALSE,
      Float         = SquirrelTypeMasks.FLOAT | SquirrelTypeMasks.SQOBJECT_NUMERIC | SquirrelTypeMasks.SQOBJECT_CANBEFALSE,
      UserPointer   = SquirrelTypeMasks.USER_POINTER, 
      String        = SquirrelTypeMasks.STRING | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Closure       = SquirrelTypeMasks.CLOSURE | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Array         = SquirrelTypeMasks.ARRAY | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      NativeClosure = SquirrelTypeMasks.NATIVE_CLOSURE | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Generator     = SquirrelTypeMasks.GENERATOR | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      UserData      = SquirrelTypeMasks.USERDATA | SquirrelTypeMasks.SQOBJECT_REF_COUNTED | SquirrelTypeMasks.SQOBJECT_DELEGABLE,
      Thread        = SquirrelTypeMasks.THREAD | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Class         = SquirrelTypeMasks.CLASS | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Instance      = SquirrelTypeMasks.INSTANCE | SquirrelTypeMasks.SQOBJECT_REF_COUNTED | SquirrelTypeMasks.SQOBJECT_DELEGABLE,
      WeakRef       = SquirrelTypeMasks.WEAKREF | SquirrelTypeMasks.SQOBJECT_REF_COUNTED,
      Bool          = SquirrelTypeMasks.BOOL | SquirrelTypeMasks.SQOBJECT_CANBEFALSE,
      Table         = SquirrelTypeMasks.TABLE | SquirrelTypeMasks.SQOBJECT_REF_COUNTED | SquirrelTypeMasks.SQOBJECT_DELEGABLE
    };

    public Type   ItemType;
    public string Name;
    public string Value;
  }
}
