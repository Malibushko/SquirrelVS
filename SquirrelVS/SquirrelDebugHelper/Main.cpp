#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <cstdint>
#include "DebugHelper.h"

extern "C"
{
  __declspec(dllexport) volatile char IsInitialized = 0;

  __declspec(dllexport) char WorkingDirectory[1024] = {};

  __declspec(dllexport) void (*sq_newclosure)(
      void *           _SquirrelVM, 
      void *           _ClosureRoutine,
      unsigned __int64 _FreeVariablesCount
    ) = nullptr;

  __declspec(dllexport) void (*sq_setdebughook)(
      void * _SquirrelVM
    ) = nullptr;

  __declspec(dllexport) void * SquirrelHandle = nullptr;
}

DWORD __stdcall HelperLoop(void * context)
{
  while (true)
  {
    if (sq_setdebughook && sq_newclosure && SquirrelHandle)
    {
      sq_newclosure  (SquirrelHandle, TraceRoutineHelper, 0);
      sq_setdebughook(SquirrelHandle);

      OnCreatedDebugHookNativeClosure();

      sq_setdebughook = nullptr;
      sq_newclosure   = nullptr;
      SquirrelHandle  = nullptr;
    }

    Sleep(10);
  }

  return 0;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved)
{
  switch (ul_reason_for_call)
  {
    case DLL_PROCESS_ATTACH:
      GetCurrentDirectoryA(1024, WorkingDirectory);

      CreateThread(0, 32 * 1024, HelperLoop, 0, 0, nullptr);

      IsInitialized = 1;

      OnSquirrelHelperInitialized();

      break;
    default:
      return 0;
  }

  return TRUE;
}