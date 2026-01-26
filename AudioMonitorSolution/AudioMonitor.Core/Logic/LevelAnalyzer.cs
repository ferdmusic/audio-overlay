using AudioMonitor.Core.Logging;
using AudioMonitor.Core.Models;

namespace AudioMonitor.Core.Logic
{
    public class LevelAnalyzer
    {
        private readonly WarningConfiguration _config;

        // Default colors for fallback if configuration is incomplete
        private const string DefaultBelowThresholdColor = "#008000"; // Dark Green (for below "Sicher")
        private const string DefaultErrorColor = "#808080"; // Grey (for errors or undefined states)

        // Performance optimization: Cache for interpolated colors
        private string? _cachedColorHex;
        private double _cachedDbfs = double.MinValue;
        private const double CacheTolerance = 0.1; // Cache results within 0.1 dBFS

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
            // Performance optimization: Use cached result if dBFS is within tolerance
            if (_cachedColorHex != null && Math.Abs(dbfs - _cachedDbfs) < CacheTolerance)
            {
                return _cachedColorHex;
            }

            if (_config.Thresholds == null || !_config.Thresholds.Any())
            {
                var defaultColor = _config.DefaultColor ?? DefaultErrorColor;
                _cachedDbfs = dbfs;
                _cachedColorHex = defaultColor;
                return defaultColor;
            }

            var sortedThresholds = _config.Thresholds; // Assuming already sorted by SettingsService or constructor
            string resultColor;

            // Case 1: DBFS is below the lowest threshold
            if (dbfs < sortedThresholds[0].DBFSValue)
            {
                resultColor = _config.DefaultColor ?? sortedThresholds[0].Color ?? DefaultErrorColor;
            }
            // Case 2: DBFS is at or above the highest threshold
            else if (dbfs >= sortedThresholds[sortedThresholds.Count - 1].DBFSValue)
            {
                resultColor = sortedThresholds[sortedThresholds.Count - 1].Color ?? DefaultErrorColor;
            }
            else
            {
                // Case 3: DBFS is between two thresholds - find lower and upper using optimized search
                // Since lists are typically small (3-5 items), linear search is actually fine
                // Binary search would add complexity without much benefit for such small lists
                ThresholdLevel lower = sortedThresholds[0];

                for (int i = 0; i < sortedThresholds.Count - 1; i++)
                {
                    if (dbfs >= sortedThresholds[i].DBFSValue && dbfs < sortedThresholds[i + 1].DBFSValue)
                    {
                        lower = sortedThresholds[i];
                        break;
                    }
                }

                resultColor = lower.Color ?? DefaultErrorColor;
            }

            // Cache the result
            _cachedDbfs = dbfs;
            _cachedColorHex = resultColor;
            return resultColor;

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
