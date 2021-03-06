#pragma once
#include <string>
#include <cassert>
#include "MemoryPool.h"

#define HELPER_API __declspec(dllexport) __declspec(noinline)

extern "C"
{
  HELPER_API void OnSquirrelHelperInitialized()
  {
    volatile char dummy = 0;
    dummy++;
  }

  HELPER_API void OnSquirrelHelperStepComplete()
  {
    volatile char dummy = 0;
    dummy++;
  }

  HELPER_API void OnSquirrelHelperAsyncBreak()
  {
    volatile char dummy = 0;
    dummy++;
  }

  HELPER_API void OnCreatedDebugHookNativeClosure()
  {
    volatile char dummy = 0;    
    dummy++;
  }


  // The following struct definitions should be kept in perfect sync with the corresponding ones in C# in Debugger.
  // To ensure matching layout, only types which are the same size on all platforms should be used. This means
  // no pointers.
#pragma pack(push, 8)

  /*
    Written by debugger
    Is SQ_UNICODE defined
  */
  __declspec(dllexport) volatile bool IsSQUnicode = false;

  /*
    Buffer in which debugger writes string data
  */
  __declspec(dllexport) char StringBuffer[1024 * 1024];

  struct BreakpointData
  {
    void *   SourceName;
    
    int64_t  Line;

    uint64_t SourceLength;
  };

  static_assert(alignof(void *) == 8,         "Unsupported alignment");
  static_assert(alignof(int64_t) == 8,        "Unsupported alignment");
  static_assert(sizeof(BreakpointData) == 24, "Unsupported sizeof");

  struct SquirrelOffsets
  {
    uint64_t StackTopOffset;
    uint64_t StackOffset;
    uint64_t SquirrelObjectPtrSize;
    uint64_t SquirrelObjectValueOffset;
    uint64_t SquirrelStringValueOffset;
  };
  
  static_assert(sizeof(SquirrelOffsets) == sizeof(uint64_t) * 5);

  /*
    Written by debugger during initialization
  */
  __declspec(dllexport) volatile SquirrelOffsets Offsets;

  /*
    Only valid inside OnBreakpointHit
  */
  __declspec(dllexport) volatile uint64_t HitBreakpointIndex;

  /*
    Active breakpoints from debugger
  */
  __declspec(dllexport) volatile BreakpointData ActiveBreakpoints[256] = {};
  __declspec(dllexport) volatile uint64_t       ActiveBreakpointsCount = 0;

  // Current stepping action, if any
  enum StepType : uint32_t
  {
    STEP_NONE = (uint32_t)-1,
    STEP_INTO = 0,
    STEP_OVER = 1,
    STEP_OUT  = 2
  };

  __declspec(dllexport) volatile int64_t  LastType = 0;
  __declspec(dllexport) volatile int64_t  LastLine = 0;

  __declspec(dllexport) volatile StepType StepKind = STEP_NONE;

  __declspec(dllexport) volatile int32_t SteppingStackDepth = 0;

  size_t GetSquirrelStringLength(
      const void * _String
    )
  {
    return IsSQUnicode ? std::wcslen((const wchar_t *)_String) : std::strlen((const char *)_String);
  }

  size_t GetSquirrelByteLength(
      const void * _String
    )
  {
    return (GetSquirrelStringLength(_String) + 1) * (IsSQUnicode ? sizeof(wchar_t) : sizeof(char));
  }

  bool StringsEqual(
      const void * _Left,
      const void * _Right,
      const size_t _Size
    )
  {
    return std::memcmp(_Left, _Right, _Size * (IsSQUnicode ? sizeof(wchar_t) : sizeof(char))) == 0;
  }

#pragma pack(pop)

  struct DebugData
  {
    const void * Type;
    const void * SourceName;
    const void * Line;
    const void * FunctionName;

    DebugData(
        const void * _BaseAddress
      )
    {
      const __int64 Top   = *reinterpret_cast<const __int64 *>(reinterpret_cast<const char*>(_BaseAddress) + Offsets.StackTopOffset);
      const char *  Stack = *reinterpret_cast<const char * const*>(reinterpret_cast<const char *>(_BaseAddress) + Offsets.StackOffset);

      auto ReadNextField = [Offset = -1, _BaseAddress, Top, Stack](const void *& _Field, bool IsString = false) mutable
      {
        _Field = Stack + ((Top + Offset) * Offsets.SquirrelObjectPtrSize) + Offsets.SquirrelObjectValueOffset;

        if (IsString)
          _Field = *reinterpret_cast<const char* const*>(_Field) + Offsets.SquirrelStringValueOffset;

        Offset--;
      };

      ReadNextField(FunctionName, true);
      ReadNextField(Line);
      ReadNextField(SourceName, true);
      ReadNextField(Type);
    }
  };

  void UpdateStepper(
      int64_t _HookCallType
    )
  {
    switch (StepKind)
    {
      case STEP_OVER:
      {
        switch ((char)_HookCallType)
        {
          case 'c':
          {
            ++SteppingStackDepth;
            break;
          }
          case 'r':
            --SteppingStackDepth;
            
            if (SteppingStackDepth < 0)
              OnSquirrelHelperStepComplete();
            break;

          case 'l':
          {
            if (SteppingStackDepth <= 0)
              OnSquirrelHelperStepComplete();

            break;
          }
        }

        break;
      }
      case STEP_INTO:
      {
        switch((char)_HookCallType)
        {
          case 'r':
          case 'l':
            SteppingStackDepth = 0;

            OnSquirrelHelperStepComplete();
            break;
        }

        break;
      }
      case STEP_OUT:
      {
          switch ((char)_HookCallType)
          {
              case 'c':
                  ++SteppingStackDepth;
                  break;
              case 'r':
              {
                  if (SteppingStackDepth == 0)
                    OnSquirrelHelperStepComplete();
                  else
                    --SteppingStackDepth;

                  break;
              }
          }
        break;
      }
    }
  }

  int TraceLine(
      const void *   _SourceName,
      const int64_t  _Line
    )
  {
    const size_t Length = GetSquirrelByteLength(_SourceName);

    for (int i = 0; i < ActiveBreakpointsCount; ++i)
    {
      auto & Data = ActiveBreakpoints[i];

      if (Data.Line == _Line)
      {
        if (Data.SourceLength == 0)
          Data.SourceLength = GetSquirrelByteLength(Data.SourceName);

        if (Data.SourceLength == Length && StringsEqual(Data.SourceName, _SourceName, Length))
          return i;
      }
    }

    return -1;
  }

  __declspec(dllexport) void TraceRoutine(
      const void *   _SquirrelVM,
      const int64_t  _Type,
      const void *   _SourceName,
      const int64_t  _Line,
      const void *   _FunctionName
    )
  {
    if (_SourceName == nullptr || _FunctionName == nullptr)
      return;

    const int HitBreakpointID = [&]() -> int
    {
      switch (_Type)
      {
        case 'l': // Line
          return TraceLine(_SourceName, _Line);
        case 'r': // Return
        case 'c': // Call
          break; 
      }

      return -1;
    }();

    ::LastLine = _Line;
    ::LastType = _Type;

    if (HitBreakpointID != -1)
    {
      ::HitBreakpointIndex      = HitBreakpointID;

      OnSquirrelHelperAsyncBreak();
    }
    else
      UpdateStepper(_Type);
  }

  extern "C" __declspec(dllexport) int64_t TraceRoutineHelper(
    void * _SquirrelVM
  )
  {
    DebugData DataLocations(static_cast<const char *>(_SquirrelVM));

    TraceRoutine(
      _SquirrelVM,
      *static_cast<const int64_t *>(DataLocations.Type),
      DataLocations.SourceName,
      *static_cast<const int64_t *>(DataLocations.Line),
      DataLocations.FunctionName
    );

    return 0;
  }
}