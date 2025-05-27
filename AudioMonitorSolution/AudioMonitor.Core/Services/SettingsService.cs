using System.Text.Json;
using System.Text.Json.Serialization;
using AudioMonitor.Core.Logging;
using AudioMonitor.Core.Models;

namespace AudioMonitor.Core.Services
{
    public class SettingsService
    {
        private static readonly string ConfigFileName = "config.json";
        // In a real app, use Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        // For this sandboxed environment, we'll place it in a known subfolder of the app's base directory.
        private static readonly string ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AudioMonitor");
        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, ConfigFileName);

        private JsonSerializerOptions _jsonOptions;

        public SettingsService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            try
            {
                if (!Directory.Exists(ConfigDirectory))
                {
                    Directory.CreateDirectory(ConfigDirectory);
                    Log.Info($"Created configuration directory at {ConfigDirectory}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating configuration directory at {ConfigDirectory}.", ex);
            }
            Log.Info($"Configuration file path set to: {ConfigFilePath}");
        }

        public ApplicationSettings LoadApplicationSettings()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    Log.Info($"Loading application settings from {ConfigFilePath}");
                    string json = File.ReadAllText(ConfigFilePath);
                    var settings = JsonSerializer.Deserialize<ApplicationSettings>(json, _jsonOptions);

                    if (settings != null)
                    {
                        // Ensure WarningLevels is initialized if null
                        if (settings.WarningLevels == null)
                        {
                            Log.Info("Loaded settings have no warning levels. Applying defaults for warning levels.");
                            settings.WarningLevels = WarningConfiguration.GetDefault();
                        }
                        else if (settings.WarningLevels.Thresholds == null || !settings.WarningLevels.Thresholds.Any())
                        {
                            Log.Info("Loaded settings have warning levels but no thresholds. Applying defaults for thresholds within warning levels.");
                            settings.WarningLevels.Thresholds = WarningConfiguration.GetDefault().Thresholds;
                        }
                        else
                        {
                            settings.WarningLevels.SortThresholds(); // Ensure they are sorted upon loading
                            Log.Info($"Successfully loaded {settings.WarningLevels.Thresholds.Count} thresholds.");
                        }

                        // Validate and default OverlayThickness if necessary (e.g., if it's a new setting)
                        if (settings.OverlayThickness <= 0) // Or some other validation like too large
                        {
                            Log.Info($"Loaded OverlayThickness is invalid ({settings.OverlayThickness}). Resetting to default.");
                            settings.OverlayThickness = ApplicationSettings.GetDefault().OverlayThickness;
                        }

                        // Validate and default new threshold properties if they are at their type defaults (e.g. 0 for double)
                        // which might indicate they weren't in the loaded config file.
                        if (settings.ThresholdSafe == 0 && settings.ThresholdWarning == 0 && settings.ThresholdCritical == 0)
                        {
                            Log.Info("Loaded dBFS thresholds are at default (0), likely from an older config. Applying application defaults.");
                            var defaultThresholds = ApplicationSettings.GetDefault();
                            settings.ThresholdSafe = defaultThresholds.ThresholdSafe;
                            settings.ThresholdWarning = defaultThresholds.ThresholdWarning;
                            settings.ThresholdCritical = defaultThresholds.ThresholdCritical;
                        }

                        return settings;
                    }
                    Log.Error($"Failed to deserialize application settings from {ConfigFilePath}. File content might be invalid. Using defaults.");
                }
                else
                {
                    Log.Info($"Configuration file not found at {ConfigFilePath}. Creating and saving default settings.");
                }
            }
            catch (JsonException jsonEx)
            {
                Log.Error($"JSON deserialization error loading application settings from {ConfigFilePath}. Details: {jsonEx.Message}. Using defaults.", jsonEx);
            }
            catch (Exception ex)
            {
                Log.Error($"General error loading application settings from {ConfigFilePath}. Using defaults.", ex);
            }

            // If any error occurs or file doesn't exist, create, save, and return default settings.
            var defaultSettings = ApplicationSettings.GetDefault();
            SaveApplicationSettings(defaultSettings);
            return defaultSettings;
        }

        public void SaveApplicationSettings(ApplicationSettings settings)
        {
            if (settings == null)
            {
                Log.Error("Attempted to save null application settings. Operation aborted.");
                return;
            }
            // Ensure WarningLevels and its Thresholds are not null before saving to prevent serialization issues.
            if (settings.WarningLevels == null)
            {
                Log.Warning("ApplicationSettings.WarningLevels is null during save. Initializing to default to prevent errors.");
                settings.WarningLevels = WarningConfiguration.GetDefault();
            }
            else if (settings.WarningLevels.Thresholds == null)
            {
                Log.Warning("ApplicationSettings.WarningLevels.Thresholds is null during save. Initializing to default to prevent errors.");
                settings.WarningLevels.Thresholds = WarningConfiguration.GetDefault().Thresholds;
            }

            try
            {
                Log.Info($"Saving application settings to {ConfigFilePath}");
                string json = JsonSerializer.Serialize(settings, _jsonOptions);
                File.WriteAllText(ConfigFilePath, json);
                Log.Info("Application settings saved successfully.");
            }
            catch (JsonException jsonEx)
            {
                Log.Error($"JSON serialization error saving application settings to {ConfigFilePath}. Details: {jsonEx.Message}.", jsonEx);
            }
            catch (Exception ex)
            {
                Log.Error($"General error saving application settings to {ConfigFilePath}.", ex);
            }
        }
    }
}
