using System.Windows;
using AudioMonitor.OverlayRenderer;
using AudioMonitor.Core.Services;
using AudioMonitor.Core.Logic;
using AudioMonitor.Core.Models;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Controls;

namespace AudioMonitor.UI.Views
{
    public partial class MainWindow : Window
    {
        private List<OverlayWindow> _overlayWindows = new();
        private AudioService _audioService;
        private LevelAnalyzer _levelAnalyzer;
        private SettingsService _settingsService;
        private WarningConfiguration _config;
        private string _selectedDeviceId;
        private bool _monitoringActive = true;
        private ComboBox? _deviceComboBox;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _settingsService = new SettingsService();
            _config = _settingsService.LoadWarningConfiguration();
            _levelAnalyzer = new LevelAnalyzer(_config);
            _audioService = new AudioService();
            var devices = _audioService.GetInputDevices();

            // Device-Auswahl-ComboBox (UI minimal)
            _deviceComboBox = new ComboBox { Width = 400, Margin = new Thickness(10) };
            foreach (var dev in devices)
                _deviceComboBox.Items.Add(dev);
            _deviceComboBox.SelectionChanged += DeviceComboBox_SelectionChanged;
            _deviceComboBox.SelectedIndex = devices.FindIndex(d => d.IsFocusrite) >= 0 ? devices.FindIndex(d => d.IsFocusrite) : 0;
            (this.Content as Grid)?.Children.Add(_deviceComboBox);

            _selectedDeviceId = (devices.Count > 0) ? devices[_deviceComboBox.SelectedIndex].Id : null;
            if (_selectedDeviceId != null)
                _audioService.StartMonitoring(_selectedDeviceId);
            _audioService.LevelChanged += AudioService_LevelChanged;

            // Multi-Monitor Overlay
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                var overlay = new OverlayWindow();
                overlay.SetOverlayPositionAndSize(new System.Windows.Rect(
                    screen.Bounds.Left,
                    screen.Bounds.Top,
                    screen.Bounds.Width,
                    10 // Streifenhöhe, später konfigurierbar
                ));
                overlay.Show();
                _overlayWindows.Add(overlay);
            }
        }

        private void DeviceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_deviceComboBox?.SelectedItem is AudioMonitor.Core.Services.AudioDevice dev)
            {
                _audioService.StopMonitoring();
                _selectedDeviceId = dev.Id;
                _audioService.StartMonitoring(_selectedDeviceId);
            }
        }

        private void AudioService_LevelChanged(object sender, double dbfs)
        {
            var colorHex = _levelAnalyzer.GetInterpolatedColor(dbfs);
            var color = OverlayColorHelper.FromHex(colorHex);
            double opacity = CalculateOpacity(dbfs);
            bool isCritical = IsCritical(dbfs);
            Dispatcher.Invoke(() =>
            {
                foreach (var overlay in _overlayWindows)
                    overlay.SetOverlayColor(color, opacity, isCritical);
            });
        }

        private double CalculateOpacity(double dbfs)
        {
            // -18 dBFS = fast unsichtbar, 0 dBFS = voll sichtbar
            double min = -18, max = 0;
            double norm = Math.Clamp((dbfs - min) / (max - min), 0, 1);
            return 0.1 + norm * 0.9; // 0.1 bis 1.0
        }

        private bool IsCritical(double dbfs)
        {
            var crit = _config.Thresholds.LastOrDefault();
            return crit != null && dbfs >= crit.DBFSValue;
        }

        public void ToggleMonitoring()
        {
            _monitoringActive = !_monitoringActive;
            if (_monitoringActive)
            {
                if (_selectedDeviceId != null)
                    _audioService.StartMonitoring(_selectedDeviceId);
            }
            else
            {
                _audioService.StopMonitoring();
            }
        }
    }
}
