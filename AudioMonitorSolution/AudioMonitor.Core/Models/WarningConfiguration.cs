using System.Collections.Generic;
using System.Linq;

namespace AudioMonitor.Core.Models
{
    public class WarningConfiguration
    {
        public List<ThresholdLevel> Thresholds { get; set; }
        public bool UseDefaultColorGradient { get; set; } // If true, UI might use a hardcoded gradient. If false, uses ThresholdLevel.Color.

        public WarningConfiguration()
        {
            Thresholds = new List<ThresholdLevel>();
            UseDefaultColorGradient = true;
        }

        public static WarningConfiguration GetDefault()
        {
            return new WarningConfiguration
            {
                UseDefaultColorGradient = true,
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
