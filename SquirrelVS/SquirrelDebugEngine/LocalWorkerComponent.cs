using Microsoft.VisualStudio.Debugger;
using Microsoft.VisualStudio.Debugger.ComponentInterfaces;
using Microsoft.VisualStudio.Debugger.Native;
using System;
using System.Linq;

namespace SquirrelDebugEngine
{
  public class LocalWorkerComponent : IDkmCustomMessageForwardReceiver
  {
    public DkmCustomMessage SendLower(
          DkmCustomMessage _Message
        )
    {
      DkmProcess Process = _Message.Process;

      switch ((MessageToLocalWorker.MessageType)_Message.MessageCode)
      {
        case MessageToLocalWorker.MessageType.FetchSquirrelSymbols:
        {
          Guid                    ModuleInstanceID = new Guid(_Message.Parameter1 as byte[]);
          DkmNativeModuleInstance Module           = Process.GetNativeRuntimeInstance()
                                                            .GetNativeModuleInstances()
                                                            .FirstOrDefault(Instance => Instance.UniqueId == ModuleInstanceID);
          if (Module == null)
             return null;

          ulong SquirrelOpen  = AttachmentHelpers.TryGetFunctionAddress(Module, "sq_open", out _).GetValueOrDefault(0);
          ulong SquirrelClose = AttachmentHelpers.TryGetFunctionAddress(Module, "sq_close", out _).GetValueOrDefault(0);

          if (SquirrelOpen != 0 && SquirrelClose != 0)
          {
              SquirrelLocations Locations = new SquirrelLocations();

              Locations.OpenStartLocation = AttachmentHelpers.TryGetFunctionAddressAtDebugStart(Module, "sq_open", out _).GetValueOrDefault(0);
              Locations.OpenEndLocation   = AttachmentHelpers.TryGetFunctionAddressAtDebugEnd(Module, "sq_open", out _).GetValueOrDefault(0);

              Locations.CloseStartLocation = AttachmentHelpers.TryGetFunctionAddressAtDebugStart(Module, "sq_close", out _).GetValueOrDefault(0);
              Locations.CloseEndLocation   = AttachmentHelpers.TryGetFunctionAddressAtDebugEnd(Module, "sq_close", out _).GetValueOrDefault(0);

              return DkmCustomMessage.Create(
                  Process.Connection,
                  Process,
                  MessageToLocal.Guid,
                  (int)MessageToLocal.MessageType.Symbols,
                  Locations,
                  null
                );
          }

          break;
        }
      }

      return null;
    }
  }
}
