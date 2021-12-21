#define WIN32_LEAN_AND_MEAN
#include <Windows.h>
#include <cstdint>
#include "DebugHelper.h"

extern "C"
{
  __declspec(dllexport) volatile char IsInitialized = 0;

  __declspec(dllexport) char WorkingDirectory[1024] = {};
}

DWORD __stdcall HelperLoop(void * context)
{
  while (true)
    Sleep(10);
  
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