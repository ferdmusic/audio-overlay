using AudioMonitor.Core.Logic;
using AudioMonitor.Core.Models;
using AudioMonitor.Core.Services;
using AudioMonitor.UI.Views;
using System.Windows;
using Application = System.Windows.Application;
using AudioMonitor.UI.Services; // For AutostartService
using System.Globalization;
using System.Threading;

namespace AudioMonitor.UI
{
    public partial class App : Application
    {
        // private NotifyIcon? _trayIcon; // Removed
        // private AudioService? _audioService; // Managed by MainWindow
        // private LevelAnalyzer? _levelAnalyzer; // Managed by MainWindow
        private SettingsService? _settingsService;
        // private WarningConfiguration? _config; // Part of ApplicationSettings
        private ApplicationSettings? _appSettings; // Centralized settings
        // private List<OverlayWindow> _overlayWindows = new(); // Managed by MainWindow
        // private string? _selectedDeviceId; // Part of ApplicationSettings
        // private int _overlayHeight = 10; // Part of ApplicationSettings
        // private string _overlayPosition = "Top"; // Part of ApplicationSettings
        // private bool _acousticWarningEnabled = false; // Part of ApplicationSettings

        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _settingsService = new SettingsService();
            _appSettings = _settingsService.LoadApplicationSettings();

            // --- Language Initialization Start ---
            string targetLanguageCode = "en-US"; // Default to English

            if (_appSettings != null && !string.IsNullOrEmpty(_appSettings.LanguageCode))
            {
                targetLanguageCode = _appSettings.LanguageCode;
            }
            // If _appSettings or LanguageCode is null/empty, it will use the "en-US" default from above.
            // This also handles the case where ApplicationSettings might not have been loaded successfully,
            // though _appSettings is initialized right before this potential block.

            try
            {
                CultureInfo targetCulture = new CultureInfo(targetLanguageCode);
                Thread.CurrentThread.CurrentUICulture = targetCulture;
                Thread.CurrentThread.CurrentCulture = targetCulture; // Optional: Also set CurrentCulture for formatting
            }
            catch (CultureNotFoundException)
            {
                // Log this error if a logging mechanism exists. For now, fall back to en-US.
                // Example: Core.Logging.Log.Warning($"Culture '{targetLanguageCode}' not found. Falling back to en-US.");
                try
                {
                    CultureInfo fallbackCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = fallbackCulture;
                    Thread.CurrentThread.CurrentCulture = fallbackCulture; // Optional
                }
                catch (CultureNotFoundException)
                {
                    // This should ideally not happen for "en-US". If it does, something is seriously wrong.
                    // Core.Logging.Log.Error($"Fallback culture 'en-US' also not found. Application may not localize correctly.");
                }
            }
            // --- Language Initialization End ---

            // Apply Autostart setting if it was changed outside the app (e.g. manual registry edit)
            // Or, more simply, ensure the registry reflects the stored setting.
            if (_appSettings.AutostartEnabled != AutostartService.IsAutostartEnabled())
            {
                AutostartService.SetAutostart(_appSettings.AutostartEnabled);
            }

            // MainWindow will handle its own audio service, overlays, etc., based on _appSettings
            _mainWindow = new MainWindow(); // MainWindow's constructor/Loaded event will use _appSettings
            // If you want to prevent MainWindow from showing initially and only run in tray:
            // _mainWindow.Hide(); // or manage visibility based on a setting
            _mainWindow.Show(); 

            // SetupTray(); // Removed call
            // Initial setup of overlays and audio monitoring is now handled by MainWindow
        }

        // Remove AudioService_LevelChanged, SetupOverlays as they are now in MainWindow.xaml.cs

        // private void SetupTray() // Removed method
        // {
        // }

        // private void ShowMainWindowSettings() // Removed method
        // {
        // }

        // private void ToggleMainWindowMonitoring() // Removed method
        // {
        // }

        // public void UpdateTrayIconStatus(bool isMonitoring, bool isCritical) // Removed method
        // {
        // }

        private void ExitApp() // This was part of the old tray icon, MainWindow now handles its own exit.
        {
            // If _mainWindow is not null and _isExplicitClose is managed there,
            // then Application.Current.Shutdown() might be called from MainWindow's Exit logic.
            // For now, let's ensure a clean shutdown if this method were somehow still called.
            // However, the goal is for MainWindow's ExitMenuItem_Click to handle this.
            if (_mainWindow != null)
            {
                // _mainWindow.IsExplicitClose = true; // This logic is in MainWindow
                _mainWindow.Close(); // This will trigger MainWindow's Closing logic
            }
            else
            {
                Application.Current.Shutdown();
            }
        }
    }
}
