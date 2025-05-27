using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using AudioMonitor.Core.Models;
using AudioMonitor.Core.Logging;

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
                        if (settings.WarningLevels == null || settings.WarningLevels.Thresholds == null || !settings.WarningLevels.Thresholds.Any())
                        {
                            Log.Info("Loaded settings have no warning levels or thresholds. Applying defaults for warning levels.");
                            settings.WarningLevels = WarningConfiguration.GetDefault();
                            // No need to save here, let the caller decide or save when settings are modified.
                        }
                        else
                        {
                           settings.WarningLevels.SortThresholds(); // Ensure they are sorted upon loading
                           Log.Info($"Successfully loaded {settings.WarningLevels.Thresholds.Count} thresholds.");
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
            if (settings.WarningLevels == null)
            {
                Log.Warning("ApplicationSettings.WarningLevels is null. Initializing with default warning levels before saving.");
                settings.WarningLevels = WarningConfiguration.GetDefault();
            }
            settings.WarningLevels.SortThresholds(); // Ensure thresholds are sorted before saving

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
