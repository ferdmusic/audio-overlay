using System;
using System.Linq;
using AudioMonitor.Core.Models;
using AudioMonitor.Core.Logging;

namespace AudioMonitor.Core.Logic
{
    public class LevelAnalyzer
    {
        private readonly WarningConfiguration _config;

        // Default colors for fallback if configuration is incomplete
        private const string DefaultBelowThresholdColor = "#008000"; // Dark Green (for below "Sicher")
        private const string DefaultErrorColor = "#808080"; // Grey (for errors or undefined states)

        public LevelAnalyzer(WarningConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
            if (_config.Thresholds == null || !_config.Thresholds.Any())
            {
                Log.Error("LevelAnalyzer initialized with no thresholds in configuration. Behavior will be undefined.");
                // Optionally, initialize with hardcoded defaults as a fallback, though SettingsService should handle this.
                _config.Thresholds = new System.Collections.Generic.List<ThresholdLevel>(); 
            }
            else
            {
                // Ensure thresholds are sorted by DBFSValue in ascending order (e.g., -60, -30, -10)
                _config.SortThresholds(); 
            }
        }

        /// <summary>
        /// Determines the active ThresholdLevel based on the current dBFS value.
        /// Thresholds are considered "active" if the level is at or above their DBFSValue,
        /// up to the DBFSValue of the next threshold.
        /// </summary>
        /// <param name="currentDBFS">The current audio level in dBFS.</param>
        /// <returns>The active ThresholdLevel, or null if below all defined thresholds or if config is empty.</returns>
        public ThresholdLevel GetCurrentWarningState(double currentDBFS)
        {
            if (!_config.Thresholds.Any())
            {
                return null; // No thresholds defined
            }

            // Iterate from the loudest threshold downwards
            for (int i = _config.Thresholds.Count - 1; i >= 0; i--)
            {
                if (currentDBFS >= _config.Thresholds[i].DBFSValue)
                {
                    return _config.Thresholds[i];
                }
            }
            
            // If below the lowest threshold
            return null; 
        }

        /// <summary>
        /// Gets the color representing the current audio level based on the warning configuration.
        /// Simplification: Returns the color of the currently active threshold.
        /// If below the lowest threshold, returns a default "below threshold" color.
        /// True color interpolation between threshold colors is a future refinement.
        /// </summary>
        /// <param name="currentDBFS">The current audio level in dBFS.</param>
        /// <returns>A string representing the color (e.g., hex code).</returns>
        public string GetInterpolatedColor(double currentDBFS)
        {
            if (!_config.Thresholds.Any())
            {
                Log.Debug("GetInterpolatedColor: No thresholds configured, returning default error color.");
                return DefaultErrorColor;
            }

            var activeState = GetCurrentWarningState(currentDBFS);

            if (activeState != null)
            {
                // Log.Debug($"GetInterpolatedColor: Active state for {currentDBFS}dBFS is '{activeState.Name}', Color: {activeState.Color}");
                return activeState.Color;
            }
            
            // If currentDBFS is below the lowest defined threshold
            // Return the color of the lowest threshold, or a specific "below all" color
            // For this version, let's use the DefaultBelowThresholdColor.
            // A more advanced version might use the lowest threshold's color with transparency.
            // Log.Debug($"GetInterpolatedColor: Level {currentDBFS}dBFS is below lowest threshold, returning DefaultBelowThresholdColor.");
            return DefaultBelowThresholdColor; 
        }
    }
}
