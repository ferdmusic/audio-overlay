using System;
using System.Windows;
using System.Windows.Controls;
using AudioMonitor.Core.Services;
using AudioMonitor.Core.Models; // Required for ApplicationSettings, ThresholdLevel, OverlayEdge
using System.Linq;
using System.Collections.Generic; // Required for List<T>
using System.Collections.ObjectModel; // Required for ObservableCollection

namespace AudioMonitor.UI.Views
{
    public partial class SettingsWindow : Window
    {
        private readonly AudioService _audioService;
        public ApplicationSettings CurrentSettings { get; private set; }        public SettingsWindow(ApplicationSettings settings, AudioService audioService)
        {
            try
            {
                InitializeComponent();

                _audioService = audioService;
                CurrentSettings = settings;

                // Populate Audio Input ComboBox
                var devices = _audioService.GetInputDevices();
                AudioInputComboBox.Items.Clear(); // Add this line
                AudioInputComboBox.ItemsSource = devices;
                if (!string.IsNullOrEmpty(CurrentSettings.SelectedAudioDeviceId))
                {
                    var selectedDev = devices.FirstOrDefault(d => d.Id == CurrentSettings.SelectedAudioDeviceId);
                    if (selectedDev != null) AudioInputComboBox.SelectedItem = selectedDev;
                    else if (devices.Any()) AudioInputComboBox.SelectedIndex = 0;
                }
                else if (devices.Any()) AudioInputComboBox.SelectedIndex = 0;

                // Populate Overlay Position ComboBox
                OverlayPositionComboBox.Items.Clear(); // Add this line
                OverlayPositionComboBox.ItemsSource = System.Enum.GetValues(typeof(OverlayEdge)); // Changed from OverlayBehaviorComboBox
                OverlayPositionComboBox.SelectedItem = CurrentSettings.OverlayPosition; // Changed from OverlayBehaviorComboBox

                // Set threshold text boxes
                ThresholdSafeTextBox.Text = CurrentSettings.ThresholdSafe.ToString();
                ThresholdWarningTextBox.Text = CurrentSettings.ThresholdWarning.ToString();
                ThresholdCriticalTextBox.Text = CurrentSettings.ThresholdCritical.ToString();

                // Set overlay thickness
                OverlayThicknessTextBox.Text = CurrentSettings.OverlayThickness.ToString();

                // Set other control values from CurrentSettings
                AcousticWarningsCheckBox.IsChecked = CurrentSettings.AcousticWarningEnabled; // Changed from EnableAcousticWarningsCheckBox
                AutostartCheckBox.IsChecked = CurrentSettings.AutostartEnabled; // Changed from StartWithWindowsCheckBox

                // Set Acoustic Volume Slider and Text
                AcousticVolumeSlider.Value = CurrentSettings.AcousticWarningVolume * 100;
                AcousticVolumeText.Text = $"{AcousticVolumeSlider.Value}%";
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error in SettingsWindow constructor: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", "SettingsWindow Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e) // Changed from SaveButton_Click
        {
            // Update CurrentSettings from UI controls
            if (AudioInputComboBox.SelectedItem is AudioDevice dev)
                CurrentSettings.SelectedAudioDeviceId = dev.Id;
            
            if (OverlayPositionComboBox.SelectedItem is OverlayEdge edge) // Changed from OverlayBehaviorComboBox
                CurrentSettings.OverlayPosition = edge;

            // Update threshold values
            if (double.TryParse(ThresholdSafeTextBox.Text, out double safe))
                CurrentSettings.ThresholdSafe = safe;
            if (double.TryParse(ThresholdWarningTextBox.Text, out double warning))
                CurrentSettings.ThresholdWarning = warning;
            if (double.TryParse(ThresholdCriticalTextBox.Text, out double critical))
                CurrentSettings.ThresholdCritical = critical;

            // Update overlay thickness
            if (int.TryParse(OverlayThicknessTextBox.Text, out int thickness))
                CurrentSettings.OverlayThickness = thickness;
            
            CurrentSettings.AutostartEnabled = AutostartCheckBox.IsChecked == true; // Changed from StartWithWindowsCheckBox
            CurrentSettings.AcousticWarningEnabled = AcousticWarningsCheckBox.IsChecked == true; // Changed from EnableAcousticWarningsCheckBox
            CurrentSettings.AcousticWarningVolume = AcousticVolumeSlider.Value / 100.0;

            DialogResult = true;
            Close();
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e) // Changed from StartApplication_Click
        {
            SaveSettings_Click(sender, e); 
            // Actual monitoring start logic would be handled by the main application controller/viewmodel after settings are saved and this window closes.
            // For now, this button primarily ensures settings are saved before the user expects monitoring to reflect new settings.
            System.Windows.MessageBox.Show("Monitoring will start with the new settings after this window is closed.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Minimize to Tray Clicked (Not Implemented)", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            // this.WindowState = WindowState.Minimized; 
            // this.Hide(); 
        }

        private void AcousticVolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AcousticVolumeText != null)
            {
                AcousticVolumeText.Text = $"{(int)e.NewValue}%";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Any actions to perform when the window is loaded, if necessary.
        }

        private void AudioInputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle selection change if needed, e.g., update a live preview or validate selection.
        }
    }
}
