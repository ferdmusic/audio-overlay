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
        private static readonly string ConfigDirectory = Path.Combine(AppContext.BaseDirectory, "AppData"); 
        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, ConfigFileName);

        private JsonSerializerOptions _jsonOptions;

        public SettingsService()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Optional: for enum handling if any
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
                // Depending on the desired robustness, you might want to throw or handle this more gracefully.
            }
            Log.Info($"Configuration file path set to: {ConfigFilePath}");
        }

        public WarningConfiguration LoadWarningConfiguration()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    Log.Info($"Loading warning configuration from {ConfigFilePath}");
                    string json = File.ReadAllText(ConfigFilePath);
                    var config = JsonSerializer.Deserialize<WarningConfiguration>(json, _jsonOptions);
                    
                    if (config != null)
                    {
                        if (config.Thresholds == null || !config.Thresholds.Any())
                        {
                            Log.Info("Loaded configuration has no thresholds or thresholds list is null. Applying defaults.");
                            config = GetDefaultWarningConfiguration();
                            SaveWarningConfiguration(config); // Save defaults back immediately
                        }
                        else
                        {
                           config.SortThresholds(); // Ensure they are sorted upon loading
                           Log.Info($"Successfully loaded {config.Thresholds.Count} thresholds.");
                        }
                        return config;
                    }
                    Log.Error($"Failed to deserialize warning configuration from {ConfigFilePath}. File content might be invalid. Using defaults.");
                }
                else
                {
                    Log.Info($"Configuration file not found at {ConfigFilePath}. Creating and saving default configuration.");
                }
            }
            catch (JsonException jsonEx)
            {
                 Log.Error($"JSON deserialization error loading warning configuration from {ConfigFilePath}. Details: {jsonEx.Message}. Using defaults.", jsonEx);
            }
            catch (Exception ex)
            {
                Log.Error($"General error loading warning configuration from {ConfigFilePath}. Using defaults.", ex);
            }

            // If any error occurs or file doesn't exist, create, save, and return default configuration.
            var defaultConfig = GetDefaultWarningConfiguration();
            SaveWarningConfiguration(defaultConfig); 
            return defaultConfig;
        }

        public void SaveWarningConfiguration(WarningConfiguration config)
        {
            if (config == null)
            {
                Log.Error("Attempted to save a null warning configuration. Operation aborted.");
                return;
            }

            try
            {
                config.SortThresholds(); // Ensure thresholds are sorted before saving
                string json = JsonSerializer.Serialize(config, _jsonOptions);
                File.WriteAllText(ConfigFilePath, json);
                Log.Info($"Warning configuration saved to {ConfigFilePath} with {config.Thresholds.Count} thresholds.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error saving warning configuration to {ConfigFilePath}.", ex);
            }
        }

        public WarningConfiguration GetDefaultWarningConfiguration()
        {
            Log.Info("Generating default warning configuration.");
            var config = new WarningConfiguration
            {
                UseDefaultColorGradient = true, 
                Thresholds = new List<ThresholdLevel>
                {
                    // Standard order: Quieter (more negative dBFS) to Louder
                    new ThresholdLevel("Sicher", -18.0, "#00FF00"), // Green
                    new ThresholdLevel("Achtung", -9.0, "#FFFF00"), // Yellow
                    new ThresholdLevel("Kritisch", -3.0, "#FF0000")  // Red
                }
            };
            // The SortThresholds method will be called before saving or after loading,
            // so explicitly calling it here is good for ensuring consistency if this method is used elsewhere.
            config.SortThresholds(); 
            return config;
        }
    }
}
