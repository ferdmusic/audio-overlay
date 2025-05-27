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
        /// Thresholds are considered "active" if the level is at or above their DBFSValue.
        /// </summary>
        /// <param name="currentDBFS">The current audio level in dBFS.</param>
        /// <returns>The active ThresholdLevel, or null if below all defined thresholds or if config is empty.</returns>
        public ThresholdLevel? GetCurrentWarningState(double currentDBFS) // Return type made nullable
        {
            if (_config.Thresholds == null || !_config.Thresholds.Any())
            {
                return null; // No thresholds defined
            }

            // Thresholds should be sorted by DBFSValue ascendingly by SortThresholds()
            // Iterate from the loudest threshold downwards to find the first one we are at or above
            for (int i = _config.Thresholds.Count - 1; i >= 0; i--)
            {
                if (currentDBFS >= _config.Thresholds[i].DBFSValue)
                {
                    return _config.Thresholds[i];
                }
            }
            
            // If below the lowest threshold (i.e., no threshold was met)
            return null; 
        }

        /// <summary>
        /// Gets the color representing the current audio level based on the warning configuration.
        /// Interpolates color between the two nearest thresholds if possible.
        /// </summary>
        /// <param name="dbfs">The current audio level in dBFS.</param>
        /// <returns>A string representing the color (e.g., hex code).</returns>
        public string GetInterpolatedColor(double dbfs)
        {
            if (_config.Thresholds == null || !_config.Thresholds.Any())
                return _config.DefaultColor ?? DefaultErrorColor; // Use DefaultColor or a hardcoded fallback

            var sortedThresholds = _config.Thresholds; // Assuming already sorted by SettingsService or constructor

            // Case 1: DBFS is below the lowest threshold
            if (dbfs < sortedThresholds[0].DBFSValue)
            {
                // Return the color of the lowest threshold, or a specific "below" color if defined.
                // For now, returning the DefaultColor as per previous logic when below lowest.
                return _config.DefaultColor ?? sortedThresholds[0].Color ?? DefaultErrorColor;
            }

            // Case 2: DBFS is at or above the highest threshold
            if (dbfs >= sortedThresholds[sortedThresholds.Count - 1].DBFSValue)
            {
                return sortedThresholds[sortedThresholds.Count - 1].Color ?? DefaultErrorColor;
            }

            // Case 3: DBFS is between two thresholds - find lower and upper
            ThresholdLevel lower = sortedThresholds[0];
            ThresholdLevel upper = sortedThresholds[sortedThresholds.Count - 1]; 

            for (int i = 0; i < sortedThresholds.Count - 1; i++)
            {
                if (dbfs >= sortedThresholds[i].DBFSValue && dbfs < sortedThresholds[i + 1].DBFSValue)
                {
                    lower = sortedThresholds[i];
                    upper = sortedThresholds[i + 1];
                    break;
                }
            }
            
            // Simplified: no complex interpolation here to avoid WPF dependencies.
            // Return the color of the lower threshold in the range.
            // More sophisticated, non-WPF color interpolation could be added if needed,
            // or this responsibility could be moved to a UI-layer helper.
            // For now, just return the lower bound color.
            // Or, if a very simple interpolation is desired and colors are known hex:
            // This example still leans towards returning the lower color to avoid complexity here.
            
            // Basic interpolation can be attempted if colors are parseable,
            // but System.Drawing.Color is not available in .NET Standard directly without extra packages.
            // Let's return the lower.Color for simplicity in the Core library.
            // The UI layer can handle more complex visual interpolations.
            return lower.Color ?? DefaultErrorColor;

            /* // Previous WPF-dependent interpolation logic:
            double range = upper.DBFSValue - lower.DBFSValue;
            if (range <= 0) 
            {
                return lower.Color ?? DefaultErrorColor;
            }

            double factor = (dbfs - lower.DBFSValue) / range;
            factor = Math.Max(0, Math.Min(1, factor));

            try 
            {
                // This part requires System.Windows.Media.Color or similar, which is not suitable for Core lib
                // var lowerColorMedia = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(lower.Color);
                // var upperColorMedia = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(upper.Color);

                // byte r = (byte)(lowerColorMedia.R + (upperColorMedia.R - lowerColorMedia.R) * factor);
                // byte g = (byte)(lowerColorMedia.G + (upperColorMedia.G - lowerColorMedia.G) * factor);
                // byte b = (byte)(lowerColorMedia.B + (upperColorMedia.B - lowerColorMedia.B) * factor);
                // return System.Windows.Media.Color.FromRgb(r, g, b).ToString();
                
                // Fallback for non-WPF environment:
                 return lower.Color ?? DefaultErrorColor;
            }
            catch (Exception ex)
            {
                Log.Error($"Color parsing/interpolation error. DBFS: {dbfs}. Lower: {lower.Color}, Upper: {upper.Color}", ex);
                return _config.DefaultColor ?? DefaultErrorColor;
            }
            */
        }

        /// <summary>
        /// Gets the current threshold level based on dBFS. This is essentially the same as GetCurrentWarningState.
        /// Consider removing this if GetCurrentWarningState serves the same purpose.
        /// </summary>
        /// <param name="dbfs">The current audio level in dBFS.</param>
        /// <returns>The active ThresholdLevel, or null.</returns>
        public ThresholdLevel? GetCurrentThreshold(double dbfs)
        {
            // This logic is identical to GetCurrentWarningState.
            // If GetCurrentWarningState is sufficient, this method can be removed to avoid redundancy.
            return GetCurrentWarningState(dbfs);
            /*
            if (_config.Thresholds == null || !_config.Thresholds.Any()) return null;

            ThresholdLevel? current = null;
            // Ensure thresholds are sorted if not already guaranteed by _config
            foreach (var threshold in _config.Thresholds.OrderBy(t => t.DBFSValue))
            {
                if (dbfs >= threshold.DBFSValue)
                {
                    current = threshold;
                }
                else
                {
                    break; // Since thresholds are sorted
                }
            }
            return current; // This can be null if dbfs is below all thresholds
            */
        }

        // Optional: Method to get a "safe" threshold if current is null
        public ThresholdLevel GetSafeThreshold(double dbfs)
        {
            var current = GetCurrentThreshold(dbfs);
            if (current != null) return current;
            return _config.Thresholds.OrderBy(t => t.DBFSValue).FirstOrDefault() ?? new ThresholdLevel("Safe", -60, _config.DefaultColor); // Use DefaultColor
        }
    }
}
