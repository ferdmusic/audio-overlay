using System;
using System.Linq;
using System.Windows;
using System.Windows.Input; // Required for MouseButtonEventArgs
using AudioMonitor.OverlayRenderer;
using AudioMonitor.Core.Services;
using AudioMonitor.Core.Logic;
using AudioMonitor.Core.Models;
using System.Windows.Media;
using System.Windows.Controls;
using AudioMonitor.UI.Views;
using System.ComponentModel;
using System.Runtime.InteropServices; // Required for DllImport
using System.Windows.Interop; // Required for WindowInteropHelper
using AudioMonitor.UI.Services;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO; // Required for Path operations
using AudioMonitor.UI.Properties;
// We might need System.Drawing if we directly use System.Drawing.Rectangle, but Screen.Bounds is already that.

namespace AudioMonitor.UI.Views
{
    public partial class MainWindow : Window
    {
        // P/Invoke declarations for DPI awareness
        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        [DllImport("Shcore.dll", SetLastError = true)]
        private static extern int GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(System.Drawing.Rectangle r)
            {
                this.Left = r.Left;
                this.Top = r.Top;
                this.Right = r.Right;
                this.Bottom = r.Bottom;
            }
        }

        private enum MonitorDpiType
        {
            MDT_EFFECTIVE_DPI = 0,
            MDT_ANGULAR_DPI = 1,
            MDT_RAW_DPI = 2,
            MDT_DEFAULT = MDT_EFFECTIVE_DPI
        }

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
        private const int S_OK = 0; // HRESULT success code

        private List<OverlayWindow> _overlayWindows = new();
        private AudioService? _audioService; // Nullable
        private LevelAnalyzer? _levelAnalyzer; // Nullable
        private SettingsService? _settingsService; // Nullable
        private ApplicationSettings? _appSettings; // Nullable
        private AcousticWarningService? _acousticWarningService; // Nullable
        private bool _monitoringActive = true;
        private System.Windows.Controls.ComboBox? _deviceComboBox;
        private SettingsWindow? _settingsWindow;
        private bool _isExplicitClose = false; // Flag to allow explicit closing from tray
        private Hardcodet.Wpf.TaskbarNotification.TaskbarIcon? _myNotifyIcon; // Field for the TaskbarIcon

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            // Closing event is handled by Window_Closing method defined below
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExplicitClose)
            {
                e.Cancel = true; // Cancel the closing event
                this.Hide();     // Hide the window
                if (_myNotifyIcon != null) _myNotifyIcon.Visibility = Visibility.Visible;
            }
            else
            {
                // Actual closing logic (save settings, dispose resources)
                if (_settingsService != null && _appSettings != null)
                {
                    _settingsService.SaveApplicationSettings(_appSettings);
                }
                _acousticWarningService?.Dispose();
                _audioService?.Dispose(); // Dispose AudioService
                if (_myNotifyIcon != null) _myNotifyIcon.Dispose();
            }
        }

        private void ShowMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        private void ResetSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Strings.ResetSettingsConfirmMessage,
                                Strings.ResetSettingsConfirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_settingsService != null && _appSettings != null)
                {
                    _settingsService.DeleteSettings();
                    _appSettings = ApplicationSettings.GetDefault();
                    _settingsService.SaveApplicationSettings(_appSettings);

                    // Re-initialize with default settings
                    _levelAnalyzer = new LevelAnalyzer(_appSettings.WarningLevels);

                    // Update Device ComboBox
                    var allDevices = new ObservableCollection<AudioDevice>();
                    if (_audioService != null)
                    {
                        var groupedDevices = _audioService.GetGroupedInputDevices();
                        foreach (var group in groupedDevices)
                        {
                            foreach (var device in group.Devices)
                            {
                                allDevices.Add(device);
                            }
                        }
                    }
                    
                    var collectionViewSource = new CollectionViewSource();
                    collectionViewSource.Source = allDevices;
                    collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("DeviceType"));
                    if (_deviceComboBox != null)
                    {
                        _deviceComboBox.ItemsSource = collectionViewSource.View;
                        if (!string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId))
                        {
                            var selectedDevice = allDevices.FirstOrDefault(d => d.Id == _appSettings.SelectedAudioDeviceId);
                            if (selectedDevice != null)
                            {
                                _deviceComboBox.SelectedItem = selectedDevice;
                            }
                            else if (allDevices.Any())
                            {
                                _deviceComboBox.SelectedIndex = 0;
                                _appSettings.SelectedAudioDeviceId = allDevices[0].Id; // Update settings
                            }
                        }
                        else if (allDevices.Any())
                        {
                             _deviceComboBox.SelectedIndex = 0;
                             _appSettings.SelectedAudioDeviceId = allDevices[0].Id; // Update settings
                        }
                    }


                    // Restart audio monitoring
                    _audioService?.StopMonitoring();
                    if (!string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId))
                    {
                        _audioService?.StartMonitoring(_appSettings.SelectedAudioDeviceId);
                    }

                    InitializeOverlays(); // Re-initialize overlays
                    AutostartService.SetAutostart(_appSettings.AutostartEnabled);

                    MessageBox.Show(Strings.ResetSettingsSuccessMessage, Strings.ResetSettingsSuccessTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _isExplicitClose = true;
            // Dispose the TaskbarIcon before shutting down
            if (_myNotifyIcon != null)
            {
                _myNotifyIcon.Dispose();
                _myNotifyIcon = null; // Set to null to prevent further access
            }
            Application.Current.Shutdown();
        }

        private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            if (_myNotifyIcon != null) _myNotifyIcon.Visibility = Visibility.Visible;
        }

        private void SettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Retrieve the TaskbarIcon from resources
            _myNotifyIcon = (Hardcodet.Wpf.TaskbarNotification.TaskbarIcon)this.Resources["MyNotifyIconResource"];

            // Programmatically add "Reset Settings" menu item
            if (_myNotifyIcon != null && _myNotifyIcon.ContextMenu != null)
            {
                var resetSettingsMenuItem = new System.Windows.Controls.MenuItem { Header = Strings.TrayMenuResetSettings };
                resetSettingsMenuItem.Click += ResetSettingsMenuItem_Click;

                // Insert before the "Exit" item, or at a specific position
                // Assuming "Exit" is the last item or preceded by a separator
                int exitItemIndex = -1;
                for(int i = 0; i < _myNotifyIcon.ContextMenu.Items.Count; i++)
                {
                    if ((_myNotifyIcon.ContextMenu.Items[i] as System.Windows.Controls.MenuItem)?.Header.ToString() == "Exit")
                    {
                        exitItemIndex = i;
                        break;
                    }
                }
                if (exitItemIndex > 0 && _myNotifyIcon.ContextMenu.Items[exitItemIndex-1] is Separator)
                {
                     _myNotifyIcon.ContextMenu.Items.Insert(exitItemIndex -1, resetSettingsMenuItem);
                }
                else if (exitItemIndex != -1)
                {
                    _myNotifyIcon.ContextMenu.Items.Insert(exitItemIndex, new Separator());
                    _myNotifyIcon.ContextMenu.Items.Insert(exitItemIndex + 1, resetSettingsMenuItem);
                }
                else // Fallback: add to the end
                {
                     if (_myNotifyIcon.ContextMenu.Items.Count > 0 && !(_myNotifyIcon.ContextMenu.Items[_myNotifyIcon.ContextMenu.Items.Count -1] is Separator))
                     {
                        _myNotifyIcon.ContextMenu.Items.Add(new Separator());
                     }
                    _myNotifyIcon.ContextMenu.Items.Add(resetSettingsMenuItem);
                }
            }

            _settingsService = new SettingsService();
            _appSettings = _settingsService.LoadApplicationSettings();
            _acousticWarningService = new AcousticWarningService(); 

            if (_appSettings == null) // Should not happen if GetDefault works
            {
                MessageBox.Show(Strings.LoadSettingsErrorMessage, Strings.LoadSettingsErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                // Consider closing or using hardcoded defaults
                _appSettings = ApplicationSettings.GetDefault(); 
            }            _levelAnalyzer = new LevelAnalyzer(_appSettings.WarningLevels);
            _audioService = new AudioService();
            var groupedDevices = _audioService.GetGroupedInputDevices();

            _deviceComboBox = DeviceComboBox;
            
            // Create CollectionViewSource for grouping
            var collectionViewSource = new CollectionViewSource();
            var allDevices = new ObservableCollection<AudioDevice>();
            
            // Add all devices from groups to the collection
            foreach (var group in groupedDevices)
            {
                foreach (var device in group.Devices)
                {
                    allDevices.Add(device);
                }
            }
            
            collectionViewSource.Source = allDevices;
            collectionViewSource.GroupDescriptions.Add(new PropertyGroupDescription("DeviceType"));
            _deviceComboBox.ItemsSource = collectionViewSource.View;
            _deviceComboBox.SelectionChanged += DeviceComboBox_SelectionChanged;            if (!string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId))
            {
                var selectedDevice = allDevices.FirstOrDefault(d => d.Id == _appSettings.SelectedAudioDeviceId);
                if (selectedDevice != null)
                {
                    _deviceComboBox.SelectedItem = selectedDevice;
                }
                else if (allDevices.Any()) // Fallback if saved device not found
                {
                    _deviceComboBox.SelectedIndex = 0;
                     _appSettings.SelectedAudioDeviceId = allDevices[0].Id; // Update settings
                }
            }
            else if (allDevices.Any()) // No device saved, select first one
            {
                 _deviceComboBox.SelectedIndex = 0;
                 _appSettings.SelectedAudioDeviceId = allDevices[0].Id; // Update settings
            }
            
            // Auto-select Focusrite if no device is pre-selected or if the pre-selected is not Focusrite but one exists
            var focusriteDevice = allDevices.FirstOrDefault(d => d.IsFocusrite);
            if (focusriteDevice != null && (_deviceComboBox.SelectedItem == null || !((AudioDevice)_deviceComboBox.SelectedItem).IsFocusrite))
            {
                _deviceComboBox.SelectedItem = focusriteDevice;
                _appSettings.SelectedAudioDeviceId = focusriteDevice.Id;
            }


            if (!string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId))
            {
                _audioService.StartMonitoring(_appSettings.SelectedAudioDeviceId); // CS8604 handled by prior null/empty check
            }
            _audioService.LevelChanged += AudioService_LevelChanged;

            InitializeOverlays();
            UpdateMonitoringStatusInApp(); // Notify App about initial status
        }

        // Add drag functionality for the borderless window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void InitializeOverlays()
        {
            // Clear existing overlays if any (e.g., on settings change)
            foreach (var overlay in _overlayWindows)
            {
                overlay.Close();
            }
            _overlayWindows.Clear();

            // Use System.Windows.Forms.Screen fully qualified
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var overlay = new OverlayWindow();
                // Set initial position and size based on _appSettings
                // The actual positioning logic will be in UpdateOverlayLayout, called by this method
                _overlayWindows.Add(overlay); 
                overlay.Show();            }
            UpdateOverlayLayout(); // Apply layout based on current settings
        }

        private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_deviceComboBox?.SelectedItem is AudioMonitor.Core.Services.AudioDevice dev)
            {
                if (dev.Id != _appSettings?.SelectedAudioDeviceId) // Null check for _appSettings
                {
                    _audioService?.StopMonitoring(); // Null check
                    if (_appSettings != null) _appSettings.SelectedAudioDeviceId = dev.Id; // Null check
                    if (!string.IsNullOrEmpty(dev.Id)) _audioService?.StartMonitoring(dev.Id); // Null check for dev.Id
                    if (_settingsService != null && _appSettings != null) _settingsService.SaveApplicationSettings(_appSettings);
                }
            }
        }

        private void AudioService_LevelChanged(object? sender, double dbfs) // sender is nullable
        {
            if (!_monitoringActive || _levelAnalyzer == null || _appSettings == null) return; // Null checks

            var colorHex = _levelAnalyzer.GetInterpolatedColor(dbfs);
            var color = OverlayColorHelper.FromHex(colorHex);
            double opacity = CalculateOpacity(dbfs);
            bool isCritical = IsCritical(dbfs);
            
            if (_appSettings.AcousticWarningEnabled && isCritical) 
            {                                                        
                _acousticWarningService?.PlayWarningSound(_appSettings.AcousticWarningVolume); // Null check
            }                                                        

            Dispatcher.Invoke(() =>
            {
                foreach (var overlay in _overlayWindows)
                    overlay.SetOverlayColor(color, opacity, isCritical);
                
                // Update tray icon directly
                UpdateTaskbarIconState(_monitoringActive, isCritical);
            });
        }

        private double CalculateOpacity(double dbfs)
        {
            if (_appSettings == null) return 0.1; // Default low opacity if settings not loaded
            var safeThreshold = _appSettings.WarningLevels.Thresholds.OrderBy(t => t.DBFSValue).FirstOrDefault()?.DBFSValue ?? -18;
            var criticalThreshold = _appSettings.WarningLevels.Thresholds.OrderByDescending(t => t.DBFSValue).FirstOrDefault()?.DBFSValue ?? 0;
            
            double min = safeThreshold; // Pegel, bei dem es fast transparent ist
            double max = criticalThreshold; // Pegel, bei dem es voll sichtbar ist

            if (dbfs <= min) return 0.1; // Nahezu transparent
            if (dbfs >= max) return 1.0; // Voll sichtbar

            double norm = (dbfs - min) / (max - min);
            return Math.Clamp(0.1 + norm * 0.9, 0.1, 1.0); // Skaliert von 0.1 bis 1.0
        }

        private bool IsCritical(double dbfs)
        {
            if (_appSettings == null) return false; // Not critical if settings not loaded
            var criticalThreshold = _appSettings.WarningLevels.Thresholds.LastOrDefault();
            return criticalThreshold != null && dbfs >= criticalThreshold.DBFSValue;
        }

        public void ToggleMonitoring()
        {
            _monitoringActive = !_monitoringActive;
            if (_monitoringActive)
            {
                if (_appSettings != null && !string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId)) // Null check
                    _audioService?.StartMonitoring(_appSettings.SelectedAudioDeviceId); // Null check
            }
            else
            {
                _audioService?.StopMonitoring(); // Null check
                // Optionally, hide overlays or set to a neutral state (e.g., transparent black)
                Dispatcher.Invoke(() =>
                {
                    foreach (var overlay in _overlayWindows)
                        overlay.SetOverlayColor(System.Windows.Media.Colors.Transparent, 0, false); // Or some neutral state
                });
            }
            UpdateMonitoringStatusInApp();
        }

        private void UpdateMonitoringStatusInApp()
        {
            // For tray icon status, we need to know if current level is critical when monitoring is active.
            // This is a simplification; ideally, the critical status comes from AudioService_LevelChanged.
            // For now, just pass false for isCritical when toggling, actual critical status is updated by LevelChanged.
            bool isCurrentlyCritical = false; 
            if(_monitoringActive && _audioService != null) {
                // This is a snapshot, actual critical status is event-driven by AudioService_LevelChanged
                // isCurrentlyCritical = IsCritical(_audioService.CurrentDBFSLevel); 
            }
            UpdateTaskbarIconState(_monitoringActive, isCurrentlyCritical); 
        }

        private void UpdateTaskbarIconState(bool isMonitoring, bool isCritical)
        {
            if (_myNotifyIcon == null) return;

            if (!isMonitoring)
            {
                _myNotifyIcon.ToolTipText = Strings.ToolTipPaused;
                StatusTextBlock.Text = $"{Strings.StatusLabel} {Strings.StatusPaused}";
                // TODO: Update _myNotifyIcon.IconSource for paused state if a specific icon is available
                // Example: _myNotifyIcon.IconSource = new BitmapImage(new Uri("pack://application:,,,/YourAppAssemblyName;component/Resources/icon_paused.ico"));
            }
            else if (isCritical)
            {
                _myNotifyIcon.ToolTipText = Strings.ToolTipCritical;
                StatusTextBlock.Text = $"{Strings.StatusLabel} {Strings.StatusCritical}"; // Assuming StatusCritical exists or use Monitoring
                // TODO: Update _myNotifyIcon.IconSource for critical state
            }
            else
            {
                _myNotifyIcon.ToolTipText = Strings.ToolTipMonitoring;
                StatusTextBlock.Text = $"{Strings.StatusLabel} {Strings.StatusMonitoring}";
                // TODO: Update _myNotifyIcon.IconSource for normal monitoring state (back to default)
            }
        }        public void ShowSettings()
        {
            if (_audioService == null || _appSettings == null || _settingsService == null) return;

            ApplicationSettings? settingsCopy = null;
            try
            {
                 settingsCopy = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(System.Text.Json.JsonSerializer.Serialize(_appSettings));
            }
            catch (System.Text.Json.JsonException ex)
            {
                Core.Logging.Log.Error("Failed to clone application settings for SettingsWindow.", ex);
                MessageBox.Show(Strings.PrepareSettingsErrorMessage, Strings.PrepareSettingsErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (settingsCopy == null)
            {
                Core.Logging.Log.Error("Failed to clone application settings (result was null).");
                MessageBox.Show(Strings.CloneSettingsErrorMessage, Strings.CloneSettingsErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _settingsWindow = new SettingsWindow(settingsCopy, _audioService); 
            }
            catch (Exception ex)
            {
                Core.Logging.Log.Error("Failed to create SettingsWindow.", ex);
                MessageBox.Show(string.Format(Strings.CreateSettingsWindowErrorMessage, ex.Message), Strings.CreateSettingsWindowErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (_settingsWindow.ShowDialog() == true)
                {
                    var newSettings = _settingsWindow.CurrentSettings;
                    if (newSettings == null)
                    {
                        Core.Logging.Log.Error("SettingsWindow returned null settings after dialog confirmation.");
                        MessageBox.Show(Strings.ApplySettingsErrorMessage, Strings.ApplySettingsErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _appSettings = newSettings; 

                    _levelAnalyzer = new LevelAnalyzer(_appSettings.WarningLevels); 

                    if (_deviceComboBox != null && (_deviceComboBox.SelectedItem == null || ((AudioDevice)_deviceComboBox.SelectedItem).Id != _appSettings.SelectedAudioDeviceId))
                    {
                         var deviceToSelect = _audioService.GetInputDevices().FirstOrDefault(d => d.Id == _appSettings.SelectedAudioDeviceId);
                         if (deviceToSelect != null)
                         {
                            _deviceComboBox.SelectedItem = deviceToSelect; 
                         }
                    }
                    else if (_audioService.CurrentDeviceId != _appSettings.SelectedAudioDeviceId && !string.IsNullOrEmpty(_appSettings.SelectedAudioDeviceId))
                    {
                        _audioService.StopMonitoring();
                        _audioService.StartMonitoring(_appSettings.SelectedAudioDeviceId);
                    }

                    UpdateOverlayLayout(); 
                    _settingsService.SaveApplicationSettings(_appSettings); 
                    AutostartService.SetAutostart(_appSettings.AutostartEnabled);
                }
            }
            catch (Exception ex)
            {
                Core.Logging.Log.Error("Error showing SettingsWindow dialog.", ex);
                MessageBox.Show(string.Format(Strings.ShowSettingsDialogErrorMessage, ex.Message), Strings.ShowSettingsDialogErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateOverlayLayout()
        {
            if (_overlayWindows == null || !_overlayWindows.Any() || _appSettings == null) return;

            var overlayThicknessSettingDips = (double)_appSettings.OverlayThickness; // Changed OverlayHeight to OverlayThickness
            var overlayPosition = _appSettings.OverlayPosition;

            var screens = System.Windows.Forms.Screen.AllScreens;

            for (int i = 0; i < Math.Min(_overlayWindows.Count, screens.Length); i++)
            {
                var overlay = _overlayWindows[i];
                var screen = screens[i];
                var screenBoundsPhysical = screen.Bounds; // System.Drawing.Rectangle (physical pixels)

                RECT screenRectPhysical = new RECT(screenBoundsPhysical);
                IntPtr hMonitor = MonitorFromRect(ref screenRectPhysical, MONITOR_DEFAULTTONEAREST);

                uint dpiX = 96; // Default DPI
                uint dpiY = 96; // Default DPI

                if (GetDpiForMonitor(hMonitor, MonitorDpiType.MDT_EFFECTIVE_DPI, out uint monitorDpiX, out uint monitorDpiY) == S_OK)
                {
                    dpiX = monitorDpiX;
                    dpiY = monitorDpiY;
                }

                double scaleX = dpiX / 96.0;
                double scaleY = dpiY / 96.0;

                // Convert screen bounds from physical pixels to DIPs
                double screenLeftDips = screenBoundsPhysical.Left / scaleX;
                double screenTopDips = screenBoundsPhysical.Top / scaleY;
                double screenWidthDips = screenBoundsPhysical.Width / scaleX;
                double screenHeightDips = screenBoundsPhysical.Height / scaleY;

                overlay.Width = screenWidthDips;
                overlay.Height = overlayThicknessSettingDips; // Use the renamed property
                overlay.Left = screenLeftDips;

                switch (overlayPosition)
                {
                    case OverlayEdge.Top:
                        overlay.Top = screenTopDips;
                        break;
                    case OverlayEdge.Bottom:
                        overlay.Top = screenTopDips + screenHeightDips - overlayThicknessSettingDips; // Use the renamed property
                        break;
                    // Add cases for Left and Right if they involve different Width/Height/Top/Left logic
                    // For now, assuming Left/Right might mean a vertical bar, which needs more layout logic.
                    // If Left/Right are full-width like Top/Bottom but just positioned differently, adjust Top/Left.
                    // This example primarily handles Top/Bottom horizontal bars.
                }
            }
        }
    }
}
