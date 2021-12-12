#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <cstdint>

extern "C"
{
  __declspec(dllexport) volatile char IsInitialized = 0;

  __declspec(dllexport) char WorkingDirectory[1024] = {};

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
    if (AsyncBreakCode != 0)
    {
      OnSquirrelHelperAsyncBreak();

      if (AsyncBreakCode == 2)
      {
        unsigned index = 2;
        while (void * state = (void *)AsyncBreakData[index++])
          ((int(*)(void *, void *, int, int))AsyncBreakData[0])(state, (void *)AsyncBreakData[1], 7, 0); // lua_sethook
        AsyncBreakCode = 0;
      }

      if (AsyncBreakCode == 4)
      {
        unsigned index = 2;
        while (void * state = (void *)AsyncBreakData[index++])
          ((int(*)(void *, void *, int, int))AsyncBreakData[0])(state, (void *)AsyncBreakData[1], 0, 0); // lua_sethook
        AsyncBreakCode = 0;
      }

      // If the code hasn't been cleared, it's a signal to stop the loop
      if (AsyncBreakCode != 0)
        break;
    }

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
  SQChar *       SourceName;
  SQInteger      Line;
  SQChar *       FunctionName;
};

static_assert(sizeof(SquirrelBreakpointData) == 32, "Unexpected align");

extern "C" __declspec(dllexport) SQInteger              HitBreakpointIndex = 0;
extern "C" __declspec(dllexport) SQInteger              ActiveBreakpointCount = 0;
extern "C" __declspec(dllexport) SquirrelBreakpointData ActiveBreakpoints[256] = {};
extern "C" __declspec(dllexport) char                   BreakpointsStringBuffer[256 * 256] = {};

extern "C" __declspec(dllexport) void SquirrelDebugHook_3_1(
    HSQUIRRELVM    _SquirrelVM,
    SQInteger      _Type,
    const SQChar * _SourceName, 
    SQInteger      _Line, 
    const SQChar * _FunctionName
  )
{
  for (int i = 0; i < ActiveBreakpointCount; i++)
  {
    if (!_SourceName || !ActiveBreakpoints[i].SourceName)
      return;

    if (//wcscmp(_SourceName, ActiveBreakpoints[i].SourceName) == 0 &&
        _Line == ActiveBreakpoints[i].Line)
    {
      HitBreakpointIndex = i;

      OnSquirrelHelperAsyncBreak();

      break;
    }
  }
}