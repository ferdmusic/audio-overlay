using System;
using System.Windows;
using System.Windows.Controls;
using AudioMonitor.Core.Services;
using AudioMonitor.Core.Models; // Required for ApplicationSettings, ThresholdLevel, OverlayEdge
using System.Linq;
using System.Collections.Generic; // Required for List<T>
using System.Collections.ObjectModel; // Required for ObservableCollection
using System.Windows.Data; // Required for CollectionViewSource
using System.Globalization;
using AudioMonitor.UI.Properties;

namespace AudioMonitor.UI.Views
{
    public class LanguageItem
    {
        public string Name { get; set; } // e.g., "English"
        public string Code { get; set; } // e.g., "en-US"

        public override string ToString() => Name; // Optional: For simple display if not using DisplayMemberPath
    }

    public partial class SettingsWindow : Window
    {
        private readonly AudioService _audioService;
        public ApplicationSettings CurrentSettings { get; private set; }        public SettingsWindow(ApplicationSettings settings, AudioService audioService)
        {
            try
            {
                InitializeComponent();

                _audioService = audioService;
                CurrentSettings = settings;                // Populate Audio Input ComboBox with grouped devices
                var deviceGroups = _audioService.GetGroupedInputDevices();
                
                // Create a flat collection for the ComboBox while maintaining grouping info
                var flatDevices = new ObservableCollection<AudioDevice>();
                foreach (var group in deviceGroups)
                {
                    foreach (var device in group.Devices)
                    {
                        flatDevices.Add(device);
                    }
                }

                // Set up CollectionViewSource for grouping
                var cvs = new CollectionViewSource { Source = flatDevices };
                cvs.GroupDescriptions.Add(new PropertyGroupDescription("DeviceType"));
                
                AudioInputComboBox.Items.Clear();
                AudioInputComboBox.ItemsSource = cvs.View;
                
                if (!string.IsNullOrEmpty(CurrentSettings.SelectedAudioDeviceId))
                {
                    var selectedDev = flatDevices.FirstOrDefault(d => d.Id == CurrentSettings.SelectedAudioDeviceId);
                    if (selectedDev != null) AudioInputComboBox.SelectedItem = selectedDev;
                    else if (flatDevices.Any()) AudioInputComboBox.SelectedIndex = 0;
                }
                else if (flatDevices.Any()) AudioInputComboBox.SelectedIndex = 0;

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

                // Populate Language ComboBox
                var languages = new List<LanguageItem>
                {
                    new LanguageItem { Name = "English", Code = "en-US" },
                    new LanguageItem { Name = "Deutsch", Code = "de-DE" }
                };
                LanguageComboBox.ItemsSource = languages;
                LanguageComboBox.DisplayMemberPath = "Name"; // Tell ComboBox to display the Name property
                LanguageComboBox.SelectedValuePath = "Code";   // Tell ComboBox the value is from the Code property

                // Set current selection
                if (!string.IsNullOrEmpty(CurrentSettings.LanguageCode))
                {
                    var currentLanguageItem = languages.FirstOrDefault(lang => lang.Code == CurrentSettings.LanguageCode);
                    if (currentLanguageItem != null)
                    {
                        LanguageComboBox.SelectedItem = currentLanguageItem;
                    }
                    else // Fallback if saved code is somehow not in our list
                    {
                        LanguageComboBox.SelectedItem = languages.FirstOrDefault(lang => lang.Code == "en-US");
                    }
                }
                else // Default to English if no language code is set in settings
                {
                    LanguageComboBox.SelectedItem = languages.FirstOrDefault(lang => lang.Code == "en-US");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(string.Format(Strings.SettingsWindowErrorMessage, ex.Message, ex.StackTrace), Strings.SettingsWindowErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
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

            string originalLanguageCode = CurrentSettings.LanguageCode;
            if (LanguageComboBox.SelectedItem is LanguageItem selectedLangItem)
            {
                CurrentSettings.LanguageCode = selectedLangItem.Code;
            }

            // Check if language changed and inform user about restart
            if (CurrentSettings.LanguageCode != originalLanguageCode)
            {
                MessageBox.Show(Strings.LanguageChangedMessage, 
                                Strings.LanguageChangedTitle, 
                                MessageBoxButton.OK, 
                                MessageBoxImage.Information);
            }

            DialogResult = true;
            Close();
        }

        private void StartMonitoring_Click(object sender, RoutedEventArgs e) // Changed from StartApplication_Click
        {
            SaveSettings_Click(sender, e); 
            // Actual monitoring start logic would be handled by the main application controller/viewmodel after settings are saved and this window closes.
            // For now, this button primarily ensures settings are saved before the user expects monitoring to reflect new settings.
            System.Windows.MessageBox.Show(Strings.MonitoringInfoMessage, Strings.MonitoringInfoTitle, MessageBoxButton.OK, MessageBoxImage.Information);
        }        private void MinimizeToTray_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
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

    public class DeviceTypeGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AudioDeviceType deviceType)
            {
                return deviceType switch
                {
                    AudioDeviceType.WASAPI => Strings.DeviceTypeWASAPI,
                    AudioDeviceType.ASIO => Strings.DeviceTypeASIO,
                    _ => Strings.DeviceTypeOther
                };
            }
            return Strings.DeviceTypeUnknown;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
