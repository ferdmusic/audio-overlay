namespace AudioMonitor.Core.Models
{
    public class WarningConfiguration
    {
        public List<ThresholdLevel> Thresholds { get; set; }
        public bool UseDefaultColorGradient { get; set; } // If true, UI might use a hardcoded gradient. If false, uses ThresholdLevel.Color.
        public string DefaultColor { get; set; } // Added DefaultColor

        public WarningConfiguration()
        {
            Thresholds = new List<ThresholdLevel>();
            UseDefaultColorGradient = true;
            DefaultColor = "#A9A9A9"; // Default to DarkGray or some other neutral
        }

        public static WarningConfiguration GetDefault()
        {
            return new WarningConfiguration
            {
                UseDefaultColorGradient = true,
                DefaultColor = "#A9A9A9", // Consistent default
                Thresholds = new List<ThresholdLevel>
                {
                    new ThresholdLevel("Safe", -18, "#00FF00"),      // Green
                    new ThresholdLevel("Caution", -9, "#FFFF00"),   // Yellow
                    new ThresholdLevel("Critical", -3, "#FF0000")    // Red
                }
            };
        }

        public void SortThresholds()
        {
            // Sort by DBFSValue in ascending order (e.g., -60, -30, -10, 0)
            // This means lower dBFS values (quieter sounds) come first.
            Thresholds = Thresholds.OrderBy(t => t.DBFSValue).ToList();
        }
    }
}
