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
        public ApplicationSettings CurrentSettings { get; private set; }

        // Use ObservableCollection for DataGrid binding to reflect add/remove operations
        public ObservableCollection<ThresholdLevel> ThresholdLevelsView { get; set; }

        public SettingsWindow(ApplicationSettings settings, AudioService audioService)
        {
            InitializeComponent();
            DataContext = this; // Set DataContext for binding ThresholdsDataGrid

            _audioService = audioService;
            CurrentSettings = settings; // Keep a reference to the passed settings object

            // Populate Device ComboBox
            var devices = _audioService.GetInputDevices();
            DeviceComboBox.ItemsSource = devices;
            if (!string.IsNullOrEmpty(CurrentSettings.SelectedAudioDeviceId))
            {
                var selectedDev = devices.FirstOrDefault(d => d.Id == CurrentSettings.SelectedAudioDeviceId);
                if (selectedDev != null) DeviceComboBox.SelectedItem = selectedDev;
                else if (devices.Any()) DeviceComboBox.SelectedIndex = 0;
            }
            else if (devices.Any()) DeviceComboBox.SelectedIndex = 0;

            // Populate Overlay Position ComboBox
            OverlayPositionComboBox.ItemsSource = System.Enum.GetValues(typeof(OverlayEdge));
            OverlayPositionComboBox.SelectedItem = CurrentSettings.OverlayPosition;

            // Set other control values from CurrentSettings
            OverlayHeightSlider.Value = CurrentSettings.OverlayHeight;
            AutostartCheckBox.IsChecked = CurrentSettings.AutostartEnabled;
            AcousticWarningCheckBox.IsChecked = CurrentSettings.AcousticWarningEnabled;
            AcousticWarningVolumeSlider.Value = CurrentSettings.AcousticWarningVolume;

            // Initialize ObservableCollection for the DataGrid
            if (CurrentSettings.WarningLevels == null)
            {
                CurrentSettings.WarningLevels = WarningConfiguration.GetDefault();
            }
            if (CurrentSettings.WarningLevels.Thresholds == null)
            {
                CurrentSettings.WarningLevels.Thresholds = new List<ThresholdLevel>();
            }
            ThresholdLevelsView = new ObservableCollection<ThresholdLevel>(CurrentSettings.WarningLevels.Thresholds);
            ThresholdsDataGrid.ItemsSource = ThresholdLevelsView; // Bind DataGrid
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Update CurrentSettings from UI controls
            if (DeviceComboBox.SelectedItem is AudioDevice dev)
                CurrentSettings.SelectedAudioDeviceId = dev.Id;
            
            CurrentSettings.OverlayPosition = (OverlayEdge)OverlayPositionComboBox.SelectedItem;
            CurrentSettings.OverlayHeight = (int)OverlayHeightSlider.Value;
            CurrentSettings.AutostartEnabled = AutostartCheckBox.IsChecked == true;
            CurrentSettings.AcousticWarningEnabled = AcousticWarningCheckBox.IsChecked == true;
            CurrentSettings.AcousticWarningVolume = AcousticWarningVolumeSlider.Value;

            // Update thresholds from the ObservableCollection back to the settings object
            CurrentSettings.WarningLevels.Thresholds = new List<ThresholdLevel>(ThresholdLevelsView);
            CurrentSettings.WarningLevels.SortThresholds(); // Ensure they are sorted

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddThreshold_Click(object sender, RoutedEventArgs e)
        {
            // Add a new default threshold to the view model collection
            // The DataGrid will automatically update.
            var newThreshold = new ThresholdLevel("New Level", -6, "#FFFFFF");
            ThresholdLevelsView.Add(newThreshold);
        }

        private void RemoveThreshold_Click(object sender, RoutedEventArgs e)
        {
            if (ThresholdsDataGrid.SelectedItem is ThresholdLevel selectedThreshold)
            {
                ThresholdLevelsView.Remove(selectedThreshold);
            }
            else
            {
                System.Windows.MessageBox.Show("Please select a threshold to remove.", "Remove Threshold", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
