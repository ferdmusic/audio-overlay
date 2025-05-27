namespace AudioMonitor.Core.Models
{
    public class ThresholdLevel
    {
        public string Name { get; set; }
        public double DBFSValue { get; set; }
        public string Color { get; set; } // Hex string like "#RRGGBB"

        public ThresholdLevel()
        {
            Name = string.Empty;
            Color = "#000000"; // Default to black
        }

        public ThresholdLevel(string name, double dbfsValue, string color)
        {
            Name = name;
            DBFSValue = dbfsValue;
            Color = color;
        }
    }
}
