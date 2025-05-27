// Filepath: c:\Users\Ferdmusic\Documents\GitHub\audio-overlay\AudioMonitorSolution\AudioMonitor.UI\Services\AutostartService.cs
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using AudioMonitor.Core.Logging; // Assuming Log class is accessible

namespace AudioMonitor.UI.Services
{
    public static class AutostartService
    {
        private const string AppName = "AudioMonitorLevelWarning"; // Name for the registry key
        private static readonly string RegistryPath = Path.Combine("Software", "Microsoft", "Windows", "CurrentVersion", "Run");

        public static bool IsAutostartEnabled()
        {
            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryPath, false))
                {
                    if (key == null)
                    {
                        Log.Warning($"Autostart: Registry key '{RegistryPath}' not found.");
                        return false;
                    }
                    object? value = key.GetValue(AppName);
                    // Check if the stored path matches the current executable path
                    string? currentExecutablePath = GetExecutablePath();
                    if (currentExecutablePath == null) return false; // Cannot determine current path

                    return value != null && value.ToString()?.Equals(currentExecutablePath, StringComparison.OrdinalIgnoreCase) == true;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Autostart: Error checking autostart status.", ex);
                return false;
            }
        }

        public static void SetAutostart(bool enable)
        {
            string? executablePath = GetExecutablePath();
            if (string.IsNullOrEmpty(executablePath))
            {
                Log.Error("Autostart: Could not determine executable path. Autostart not set.");
                return;
            }

            try
            {
                using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryPath, true)) // true for writable
                {
                    if (key == null)
                    {
                        Log.Error($"Autostart: Could not open or create registry key '{RegistryPath}' for writing.");
                        // Attempt to create the key if it doesn't exist (though CurrentUser\Software\Microsoft\Windows\CurrentVersion\Run should always exist)
                        // For deeper paths, one might need Registry.CurrentUser.CreateSubKey(...)
                        // However, modifying 'Run' directly is standard. If it's missing, something is very wrong with the OS.
                        return;
                    }

                    if (enable)
                    {
                        key.SetValue(AppName, $"\"{executablePath}\""); // Corrected: Removed extra parenthesis at the end of the line
                        Log.Info($"Autostart: Enabled for '{AppName}' with path '{executablePath}'.");
                    }
                    else
                    {
                        if (key.GetValue(AppName) != null)
                        {
                            key.DeleteValue(AppName, false); // false: do not throw if not found
                            Log.Info($"Autostart: Disabled for '{AppName}'.");
                        }
                        else
                        {
                            Log.Info($"Autostart: Already disabled for '{AppName}', no action taken.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Autostart: Error setting autostart status to {enable}.", ex);
            }
        }

        private static string? GetExecutablePath()
        {
            // For .NET Core/5+ applications, Process.GetCurrentProcess().MainModule.FileName might be more reliable
            // especially for single-file executables or when Assembly.GetEntryAssembly() could be null.
            try
            {
                // string? path = System.Reflection.Assembly.GetEntryAssembly()?.Location;
                string? path = Process.GetCurrentProcess()?.MainModule?.FileName;
                if (string.IsNullOrEmpty(path) && AppContext.BaseDirectory != null)
                {
                    // Fallback for some scenarios, e.g. if published as single file, MainModule.FileName might be the extractor
                    // This needs careful testing depending on the publish method.
                    // For a typical .csproj build, MainModule.FileName should be correct.
                    // Example: find the .exe in the base directory. This is a simplification.
                    // A more robust way would be to ensure the entry assembly location is correctly resolved.
                    // For now, relying on MainModule.FileName.
                }
                return path;
            }
            catch (Exception ex)
            {
                Log.Error("Autostart: Could not get executable path.", ex);
                return null;
            }
        }
    }
}
