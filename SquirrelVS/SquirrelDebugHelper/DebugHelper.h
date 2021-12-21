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
  }

  HELPER_API void OnSquirrelHelperStepComplete()
  {
    volatile char dummy = 0;
  }

  HELPER_API void OnSquirrelHelperAsyncBreak()
  {
    volatile char dummy = 0;
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

  /*
    Private buffers for callstack temporary holding
  */
  MemoryPool<1024 * 1024> FunctionNamesBuffer = {};
  MemoryPool<1024 * 1024> SourceNamesBuffer = {};

  struct BreakpointData
  {
    void *   SourceName;
    
    int64_t  Line;

    uint64_t SourceLength;
  };

  static_assert(alignof(void *) == 8,         "Unsupported alignment");
  static_assert(alignof(int64_t) == 8,        "Unsupported alignment");
  static_assert(sizeof(BreakpointData) == 24, "Unsupported sizeof");

  struct CallStackFrame
  {
    void * SourceName;

    void * FunctionName;
    
    uint64_t LastLine;
  };

  /*
    Represents squirrel call stack
  */

  __declspec(dllexport) CallStackFrame Callstack[256] = {};
  __declspec(dllexport) uint64_t       CallstackSize  = 0;

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
  enum StepType : int32_t
  {
    STEP_NONE,
    STEP_INTO,
    STEP_OVER,
    STEP_OUT
  };

  __declspec(dllexport) volatile StepType StepKind = STEP_NONE;

  __declspec(dllexport) volatile uint32_t SteppingStackDepth = 0;

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

  int TraceCall(
      void *   _SourceName,
      int64_t  _Line
    )
  {
    if (StepKind != STEP_NONE)
      ++SteppingStackDepth;

    return -1;
  }

  int TraceReturn(
      void *   _SourceName,
      int64_t  _Line
    )
  {
    if (StepKind != STEP_NONE)
    {
      --SteppingStackDepth;

      if (StepKind == STEP_OUT)
        OnSquirrelHelperStepComplete();
    }

    return -1;
  }

  int TraceLine(
      void *   _SourceName,
      int64_t  _Line
    )
  {
    if (StepKind != STEP_NONE)
    {
      SteppingStackDepth = 0;

      OnSquirrelHelperStepComplete();

      return -1;
    }

    const size_t Length = IsSQUnicode ? std::wcslen((const wchar_t *)_SourceName) : std::strlen((const char *)_SourceName);

    for (int i = 0; i < ActiveBreakpointsCount; ++i)
    {
      auto & Data = ActiveBreakpoints[i];

      if (Data.Line == _Line)
      {
        if (Data.SourceLength == 0)
          Data.SourceLength = IsSQUnicode ? std::wcslen((const wchar_t *)_SourceName) : std::strlen((const char *)_SourceName);

        if (Data.SourceLength == Length && StringsEqual(Data.SourceName, _SourceName, Length))
          return i;
      }
    }

    return -1;
  }

  __declspec(dllexport) void TraceRoutine(
      void *   _SquirrelVM,
      int64_t  _Type,
      void *   _SourceName,
      int64_t  _Line,
      void *   _FunctionName
    )
  {
    int HitBreakpoint = -1;

    if (_Type == 'r')
    {
      assert(CallstackSize > 0);

      auto & Frame = Callstack[CallstackSize - 1];

      delete Frame.FunctionName;
      delete Frame.SourceName;

      Frame = CallStackFrame{};

      CallstackSize--;
      
      HitBreakpoint = TraceReturn(_SourceName, _Line);
    }
    else
    if (_Type == 'l')
    { 
      assert(CallstackSize > 0);
      
      Callstack[CallstackSize - 1].LastLine = _Line;

      HitBreakpoint = TraceLine(_SourceName, _Line);
    }
    else
    if (_Type == 'c')
    { 
      auto & Frame = Callstack[CallstackSize];

      Frame.LastLine = _Line;
      
      if (_FunctionName != nullptr)
      {
        const size_t FunctionNameByteCount = GetSquirrelByteLength(_FunctionName);

        Frame.FunctionName = new char[FunctionNameByteCount];

        if (Frame.FunctionName == nullptr)
          return;

        std::memcpy(Frame.FunctionName, _FunctionName, FunctionNameByteCount);
      }

      if (_SourceName != nullptr)
      {
        const size_t SourceNameByteCount = GetSquirrelByteLength(_SourceName);

        Frame.SourceName = new char[SourceNameByteCount];

        if (Frame.SourceName == nullptr)
          return;

        std::memcpy(Frame.SourceName, _SourceName, SourceNameByteCount);
      }

      CallstackSize++;

      HitBreakpoint = TraceCall(_SourceName, _Line);
    }

    if (HitBreakpoint != -1)
    {
      ::HitBreakpointIndex = HitBreakpoint;

      OnSquirrelHelperAsyncBreak();
    }
  }
}