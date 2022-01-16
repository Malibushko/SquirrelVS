extern "C" __declspec(dllexport) __declspec(noinline) void OnSquirrelHelperAsyncBreak()
{
  volatile char dummy = 0;
  dummy++;
}