using Microsoft.VisualStudio.TestTools.UnitTesting;
using AudioMonitor.Core.Models;
using AudioMonitor.Core.Services;
using System;
using System.IO;
using System.Linq;
using System.Text.Json; // Required for direct comparison if needed, and for JsonStringEnumConverter

namespace AudioMonitor.Core.Tests
{
    [TestClass]
    public class SettingsServiceTests
    {
        private SettingsService? _settingsService; // Made nullable
        private string? _testConfigFilePath; // Made nullable

        [TestInitialize]
        public void Setup()
        {
            _settingsService = new SettingsService();

            // Construct the expected path to clean up correctly.
            // This mirrors the logic in SettingsService for determining the path.
            string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioMonitor");
            _testConfigFilePath = Path.Combine(configDir, "config.json");

            // Clean up any existing config file from previous test runs before each test.
            if (File.Exists(_testConfigFilePath))
            {
                File.Delete(_testConfigFilePath);
            }
        }

        [TestMethod]
        public void SaveAndLoadSettings_ShouldPreserveSettings()
        {
            // Arrange
            var originalSettings = ApplicationSettings.GetDefault();
            originalSettings.SelectedAudioDeviceId = "test-device-id";
            originalSettings.OverlayPosition = OverlayEdge.Bottom;
            originalSettings.OverlayThickness = 25; // Changed from OverlayHeight
            originalSettings.AcousticWarningEnabled = true;
            originalSettings.AcousticWarningVolume = 0.55;
            originalSettings.AutostartEnabled = true;
            originalSettings.WarningLevels.Thresholds.Add(new ThresholdLevel("TestLevel", -5, "#123456"));
            originalSettings.WarningLevels.SortThresholds(); // Ensure it's sorted before comparison

            // Act
            _settingsService!.SaveApplicationSettings(originalSettings); // Added null-forgiving operator
            var loadedSettings = _settingsService!.LoadApplicationSettings(); // Added null-forgiving operator

            // Assert
            Assert.IsNotNull(loadedSettings);
            Assert.AreEqual(originalSettings.SelectedAudioDeviceId, loadedSettings.SelectedAudioDeviceId);
            Assert.AreEqual(originalSettings.OverlayPosition, loadedSettings.OverlayPosition);
            Assert.AreEqual(originalSettings.OverlayThickness, loadedSettings.OverlayThickness); // Changed from OverlayHeight
            Assert.AreEqual(originalSettings.AcousticWarningEnabled, loadedSettings.AcousticWarningEnabled);
            Assert.AreEqual(originalSettings.AcousticWarningVolume, loadedSettings.AcousticWarningVolume, 0.001); // Delta for double
            Assert.AreEqual(originalSettings.AutostartEnabled, loadedSettings.AutostartEnabled);

            Assert.IsNotNull(loadedSettings.WarningLevels);
            Assert.IsNotNull(loadedSettings.WarningLevels.Thresholds);
            Assert.AreEqual(originalSettings.WarningLevels.Thresholds.Count, loadedSettings.WarningLevels.Thresholds.Count);

            // Sort both by DBFSValue then Name to ensure order for comparison
            var originalThresholds = originalSettings.WarningLevels.Thresholds.OrderBy(t => t.DBFSValue).ThenBy(t => t.Name).ToList();
            var loadedThresholds = loadedSettings.WarningLevels.Thresholds.OrderBy(t => t.DBFSValue).ThenBy(t => t.Name).ToList();

            for (int i = 0; i < originalThresholds.Count; i++)
            {
                Assert.AreEqual(originalThresholds[i].Name, loadedThresholds[i].Name);
                Assert.AreEqual(originalThresholds[i].DBFSValue, loadedThresholds[i].DBFSValue, 0.001);
                Assert.AreEqual(originalThresholds[i].Color, loadedThresholds[i].Color);
            }
        }

        [TestMethod]
        public void LoadSettings_WhenNoFileExists_ReturnsDefaultSettings()
        {
            // Arrange (ensure no file exists - Setup does this)

            // Act
            var loadedSettings = _settingsService!.LoadApplicationSettings(); // Added null-forgiving operator

            // Assert
            var defaultSettings = ApplicationSettings.GetDefault();
            Assert.IsNotNull(loadedSettings);
            // Check a few key default properties
            Assert.AreEqual(defaultSettings.OverlayPosition, loadedSettings.OverlayPosition);
            Assert.AreEqual(defaultSettings.OverlayThickness, loadedSettings.OverlayThickness); // Changed from OverlayHeight
            Assert.AreEqual(defaultSettings.AcousticWarningEnabled, loadedSettings.AcousticWarningEnabled);
            Assert.AreEqual(defaultSettings.WarningLevels.Thresholds.Count, loadedSettings.WarningLevels.Thresholds.Count);
            
            // Verify that a new default config file was created
            Assert.IsTrue(File.Exists(_testConfigFilePath!), "A new config file should have been created with default settings."); // Added null-forgiving operator
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            // Clean up the config file created during the test.
            if (!string.IsNullOrEmpty(_testConfigFilePath) && File.Exists(_testConfigFilePath))
            {
                File.Delete(_testConfigFilePath);
            }
        }
    }
}
