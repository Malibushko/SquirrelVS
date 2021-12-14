#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <cstdint>

extern "C"
{
  __declspec(dllexport) volatile char IsInitialized = 0;

  __declspec(dllexport) char WorkingDirectory[1024] = {};

  __declspec(dllexport) __declspec(noinline) void OnSquirrelFunctionCall()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelFunctionReturn()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelFunctionLine()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperInitialized()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperBreakpointHit()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperStepComplete()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperStepOut()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperStepInto()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperAsyncBreak()
  {
    volatile char dummy = 0;
  }

  __declspec(dllexport) DWORD breakpointLoopThreadId;
}

extern "C" __declspec(dllexport) volatile unsigned  AsyncBreakCode = 0;
extern "C" __declspec(dllexport) unsigned long long AsyncBreakData[1024] = {};

DWORD __stdcall BreakpointHookLoop(void * context)
{
  while (true)
  {
    Sleep(40);
  }

  return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
  switch (ul_reason_for_call)
  {
    case DLL_PROCESS_ATTACH:
      GetCurrentDirectoryA(1024, WorkingDirectory);

      CreateThread(0, 32 * 1024, BreakpointHookLoop, 0, 0, &breakpointLoopThreadId);

      IsInitialized = 1;

      OnSquirrelHelperInitialized();

      break;
    default:
      return 0;
  }

  return TRUE;
}

#include "squirrel/3.1/squirrel.h"

struct SquirrelBreakpointData
{
  SQInteger      Type;
  const SQChar * SourceName;
  SQInteger      Line;
  const SQChar * FunctionName;
};

enum StepperState
{
  None,
  StepInto,
  StepOver,
  StepOut
};

static_assert(sizeof(SquirrelBreakpointData) == 32, "Unexpected align");

extern "C" __declspec(dllexport) SQInteger              HitBreakpointIndex = 0;
extern "C" __declspec(dllexport) SQInteger              ActiveBreakpointCount = 0;
extern "C" __declspec(dllexport) SquirrelBreakpointData ActiveBreakpoints[256] = {};
extern "C" __declspec(dllexport) char                   BreakpointsStringBuffer[256 * 256] = {};

extern "C" __declspec(dllexport) SquirrelBreakpointData StackInfo = {};

extern "C" __declspec(dllexport) SQBool        BoolBuffer         = SQFalse;
extern "C" __declspec(dllexport) SQInteger     IntegerBuffer      = 0;
extern "C" __declspec(dllexport) SQFloat       FloatBuffer        = 0.f;
extern "C" __declspec(dllexport) SQUserPointer PointerBuffer      = nullptr;
extern "C" __declspec(dllexport) SQChar      * StringBuffer       = nullptr;

extern "C" __declspec(dllexport) int           StepperState       = StepperState::None;

int NestedCalls = 0;

extern "C" __declspec(dllexport) void SquirrelDebugHook_3_1(
    HSQUIRRELVM    _SquirrelVM,
    SQInteger      _Type,
    const SQChar * _SourceName, 
    SQInteger      _Line, 
    const SQChar * _FunctionName
  )
{
  StackInfo.Type         = _Type;
  StackInfo.SourceName   = _SourceName;
  StackInfo.Line         = _Line;
  StackInfo.FunctionName = _FunctionName;

  if (_Type == 'c')
    OnSquirrelFunctionCall();
  else
  if (_Type == 'r')
    OnSquirrelFunctionReturn();
  else
  if (_Type == 'l')
    OnSquirrelFunctionLine();
  

  switch (StepperState)
  {
    case StepperState::StepInto:
    {
      if (_Type == 'l')
      {
        NestedCalls = 0;

        OnSquirrelHelperAsyncBreak();
        return;
      }
      break;
    }

    case StepperState::StepOut:
    {
      if (_Type == 'r')
      {
        if (NestedCalls == 0)
          StepperState = StepperState::StepOver;
        else
          NestedCalls--;
      }
      break;
    }

    case StepperState::StepOver:
    {
      if (_Type == 'l' && NestedCalls == 0)
      {
        OnSquirrelHelperAsyncBreak();
        return;
      }
      else
        if (_Type == 'c')
          NestedCalls++;
        else
          if (_Type == 'r')
            NestedCalls--;

      break;
    }
  }
  for (int i = 0; i < ActiveBreakpointCount; i++)
  {
    if (!_SourceName || !ActiveBreakpoints[i].SourceName)
      break;

    if (//wcscmp(_SourceName, ActiveBreakpoints[i].SourceName) == 0 &&
        _Line == ActiveBreakpoints[i].Line)
    {
      HitBreakpointIndex = i;

      if (_Type == 'l')
      {
        OnSquirrelHelperAsyncBreak();
      }
      break;
    }
  }
}