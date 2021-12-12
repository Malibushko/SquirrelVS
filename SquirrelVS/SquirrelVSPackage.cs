using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE100;
using EnvDTE80;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Diagnostics;
using Task = System.Threading.Tasks.Task;
using System.Text;
using System.Runtime.ExceptionServices;

namespace SquirrelVS
{
  /// <summary>
  /// This is the class that implements the package exposed by this assembly.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The minimum requirement for a class to be considered a valid package for Visual Studio
  /// is to implement the IVsPackage interface and register itself with the shell.
  /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
  /// to do it: it derives from the Package class that provides the implementation of the
  /// IVsPackage interface and uses the registration attributes defined in the framework to
  /// register itself and its components with the shell. These attributes tell the pkgdef creation
  /// utility what data to put into .pkgdef file.
  /// </para>
  /// <para>
  /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
  /// </para>
  /// </remarks>
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
  [Guid(SquirrelVSPackage.PackageGuidString)]
  public sealed class SquirrelVSPackage : AsyncPackage
  {
    private IServiceProvider ServiceProvider => this;
    private IServiceContainer ServiceContainer => this;

    public const string PackageGuidString = "A391C18F-274F-4F9C-A8B1-F053E3C0738F";

    #region Package Members

    /// <summary>
    /// Initialization of the package; this method is called right after the package is sited, so this is the place
    /// where you can put all the initialization code that rely on services provided by VisualStudio.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
    /// <param name="progress">A provider for progress updates.</param>
    /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
      // When initialized asynchronously, the current thread may be a background thread at this point.
      // Do any initialization that requires the UI thread after switching to the UI thread.
      await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

      AppDomain.CurrentDomain.FirstChanceException += FirstChanceHandler;

      try
      {
        DTE2 dte = (DTE2)ServiceProvider.GetService(typeof(SDTE));

        Debugger5 debugger = dte?.Debugger as Debugger5;

        var debuggerEventHandler = new SquirrelDebuggerEventHandler(debugger);

        ServiceContainer.AddService(debuggerEventHandler.GetType(), debuggerEventHandler, promote: true);
      }
      catch (Exception _Exception)
      {
        System.Console.Write($"Failed to initialize debugger: {_Exception.Message}");

        Debug.WriteLine(_Exception.Message);
      }
    }

    #endregion
    static void FirstChanceHandler(object source, FirstChanceExceptionEventArgs e)
    {
      Console.WriteLine("FirstChanceException event raised in {0}: {1}",
          AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
    }

    [Guid("A5A48B02-119E-4DD3-9FFE-2B0D67C06EC0")]
    public class SquirrelDebuggerEventHandler : IVsCustomDebuggerEventHandler110
    {
      static class MessageToVsService
      {
        public static readonly int reloadBreakpoints = 1;
        public static readonly int scriptLoad = 2;
        public static readonly int setStatusText = 3;
      }

      private class ScriptLoadMessage
      {
        public string name;
        public string path;
        public string status;
        public string content;

        public bool ReadFrom(byte[] data)
        {
          using (var stream = new MemoryStream(data))
          {
            using (var reader = new BinaryReader(stream))
            {
              name = reader.ReadString();
              path = reader.ReadString();
              status = reader.ReadString();
              content = reader.ReadString();
            }
          }

          return true;
        }
      }

      private class StatusTextMessage
      {
        public int id;
        public string content;

        public bool ReadFrom(byte[] data)
        {
          using (var stream = new MemoryStream(data))
          {
            using (var reader = new BinaryReader(stream))
            {
              id = reader.ReadInt32();
              content = reader.ReadString();
            }
          }

          return true;
        }
      }

      private class BreakpointData
      {
        public EnvDTE90a.Breakpoint3 source;

        public bool Enabled;

        public string FunctionName;
        public string File;
        public int FileLine;
        public int FileColumn;
        public string Condition;
        public EnvDTE.dbgBreakpointConditionType ConditionType;
        public int HitCountTarget;
        public EnvDTE.dbgHitCountType HitCountType;
      }

      private readonly Debugger5 debugger;

      public SquirrelDebuggerEventHandler(Debugger5 debugger)
      {
        this.debugger = debugger;
      }

      public int OnCustomDebugEvent(ref Guid ProcessId, VsComponentMessage message)
      {
        System.Console.Write($"Received custom event {message.MessageCode}");

        ThreadHelper.ThrowIfNotOnUIThread();

        if (message.MessageCode == MessageToVsService.reloadBreakpoints)
        {
          try
          {
            System.Collections.Generic.List<BreakpointData> reload = new System.Collections.Generic.List<BreakpointData>();
            var Path                                               = Encoding.UTF8.GetString(message.Parameter1 as byte[]);

            foreach (var breakpoint in debugger.Breakpoints)
            {
              if (breakpoint is EnvDTE90a.Breakpoint3 breakpoint3)
              {
                if (!breakpoint3.Enabled)
                  continue;

                string breakpointPath = breakpoint3.File;

                if (breakpointPath.EndsWith(".nut"))
                {
                  reload.Add(new BreakpointData
                  {
                    source = breakpoint3,

                    Enabled = breakpoint3.Enabled,

                    FunctionName = breakpoint3.FunctionName,
                    File = breakpoint3.File,
                    FileLine = breakpoint3.FileLine,
                    FileColumn = breakpoint3.FileColumn,
                    Condition = breakpoint3.Condition,
                    ConditionType = breakpoint3.ConditionType,
                    HitCountTarget = breakpoint3.HitCountTarget,
                    HitCountType = breakpoint3.HitCountType,
                  });
                }
              }
            }

            foreach (var breakpoint in reload)
              breakpoint.source.Delete();

            foreach (var breakpoint in reload)
            {
              debugger.Breakpoints.Add(breakpoint.FunctionName, breakpoint.File, breakpoint.FileLine, breakpoint.FileColumn, breakpoint.Condition, breakpoint.ConditionType, "Squirrel", "", 1, "", breakpoint.HitCountTarget, breakpoint.HitCountType);

              if (!breakpoint.Enabled)
                debugger.Breakpoints.Item(debugger.Breakpoints.Count - 1).Enabled = false;
            }
          }
          catch (Exception e)
          {
            System.Console.Write("Failed to reload breakpoints with " + e.Message);
          }
        }
        return 0;
      }
    }
  }
}
