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

        public void SortThresholds()
        {
            // Sort by DBFSValue in ascending order (e.g., -60, -30, -10, 0)
            // This means lower dBFS values (quieter sounds) come first.
            Thresholds = Thresholds.OrderBy(t => t.DBFSValue).ToList();
        }
    }
}
