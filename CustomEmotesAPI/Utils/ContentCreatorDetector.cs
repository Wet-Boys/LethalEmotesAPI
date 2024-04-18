using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using EmotesAPI;
using JetBrains.Annotations;

namespace LethalEmotesAPI.Utils;

/// <summary>
/// Some stuff in here may look scary,
/// I swear it's only so we can detect if OBS or XSplit is open, so we may prompt them to enable DMCA-friendly mode.
/// </summary>
internal static class ContentCreatorDetector
{
    [CanBeNull] private static ProcessMonitor _processMonitor;
    
    public static void Init()
    {
        // Don't waste (the very small amount of) system resources if they've disabled the prompt
        if (Settings.dontShowDmcaPrompt.Value)
            return;
        
        if (OnWineOrProton())
        {
            // Technically someone could be running on macOS or something, but those people aren't real
            _processMonitor = new LinuxProcessMonitor();
        }
        else
        {
            _processMonitor = new WindowsProcessMonitor();
        }

        _processMonitor.OnProcessCreated += ProcessMonitorOnProcessCreated;
    }

    private static void ProcessMonitorOnProcessCreated(string processName)
    {
        // TODO: Verify xsplit process name.
        if (!processName.Contains("obs", StringComparison.InvariantCultureIgnoreCase) && !processName.Contains("xsplit", StringComparison.InvariantCultureIgnoreCase))
            return;
        
        
    }

    private static bool OnWineOrProton()
    {
        var handle = GetModuleHandle("ntdll.dll");
        if (handle == IntPtr.Zero)
        {
            DebugClass.Log("Failed to get handle to ntdll.dll!");
            return false;
        }

        var procWineGetVersion = GetProcAddress(handle, "wine_get_version");
        if (procWineGetVersion != IntPtr.Zero)
            return true;
        
        DebugClass.Log("Failed to get Proc Address to wine_get_version!");
        return false;
    }
    
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string moduleName);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
    
    
    /// <summary>
    /// All Linux process exist as folders under /proc making it trivial to monitor for changes.
    /// </summary>
    private class LinuxProcessMonitor : ProcessMonitor
    {
        private readonly FileSystemWatcher _watcher;
        
        public LinuxProcessMonitor()
        {
            _watcher = new FileSystemWatcher(@"z:\proc");

            _watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
        
            _watcher.Created += (_, args) =>
            {
                var procName = ReadLinuxProcessName(args.FullPath);

                if (string.IsNullOrWhiteSpace(procName))
                    return;
                
                ProcessCreated(procName);
            };
        
            _watcher.EnableRaisingEvents = true;
        
            // Do initial check of all existing processes in-case obs was launched prior.
            foreach (var runningProcess in GetRunningProcesses())
                ProcessCreated(runningProcess);
        }

        protected sealed override string[] GetRunningProcesses()
        {
            return Directory.GetDirectories(@"z:\proc\", "*", SearchOption.TopDirectoryOnly)
                .Where(proc => int.TryParse(proc.Split('\\').Last(), out _))
                .Select(ReadLinuxProcessName)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToArray();
        }

        public override void Dispose()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }
        
        #region Linux Helper Methods

        private static string ReadLinuxProcessName(string procPath)
        {
            var statusContents = ReadLinuxProcStatusContents(procPath);

            return GetLinuxProcNameFromStatusContents(statusContents);
        }

        private static string ReadLinuxProcStatusContents(string procPath)
        {
            try
            {
                var statusPath = $"{procPath}\\status";
                var contents = File.ReadAllText(statusPath);
                return contents;
            }
            catch
            {
                return "";
            }
        }

        private static string GetLinuxProcNameFromStatusContents(string statusContents)
        {
            try
            {
                var lines = statusContents.Split('\n');
                foreach (var line in lines)
                {
                    if (!line.StartsWith("Name:"))
                        continue;

                    return line.Split(':')
                        .Last()
                        .Trim();
                }
            }
            catch
            {
                // Ignored
            }
        
            return "";
        }

        #endregion
    }
    
    /// <summary>
    /// Detecting new processes on Windows is less easy than Linux, at least when using netstandard2.1
    /// </summary>
    private class WindowsProcessMonitor : ProcessMonitor
    {
        private readonly CancellationTokenSource _cts = new();
        private string[] _lastProcesses = [];
        
        public WindowsProcessMonitor()
        {
            var pollThread = new Thread(Poll);
            pollThread.Start(_cts.Token);
        }

        private void Poll(object tokenObj)
        {
            var token = (CancellationToken)tokenObj;
            
            while (!token.IsCancellationRequested)
            {
                var currentProcesses = GetRunningProcesses();

                foreach (var process in currentProcesses)
                {
                    if (!_lastProcesses.Contains(process))
                        ProcessCreated(process);
                }

                _lastProcesses = currentProcesses;
                
                Thread.Sleep(TimeSpan.FromSeconds(0.25f));
            }
        }
        
        protected override string[] GetRunningProcesses()
        {
            return Process.GetProcesses()
                .Select(proc =>
                {
                    var name = proc.ProcessName;
                    proc.Dispose();
                
                    return name;
                })
                .ToArray();
        }

        public override void Dispose()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                Thread.Sleep(2500);
            }
            
            _cts.Dispose();
        }
    }

    private abstract class ProcessMonitor : IDisposable
    {
        public event ProcessCreatedDelegate OnProcessCreated;
        
        public delegate void ProcessCreatedDelegate(string processName);

        protected abstract string[] GetRunningProcesses();

        protected void ProcessCreated(string processName) => UnityThreadScheduler.EnqueueWorkOnUnityThread(() => OnProcessCreated?.Invoke(processName));

        public virtual void Quit()
        {
            ThreadPool.QueueUserWorkItem(_ => Dispose());
        }

        public abstract void Dispose();
    }
}