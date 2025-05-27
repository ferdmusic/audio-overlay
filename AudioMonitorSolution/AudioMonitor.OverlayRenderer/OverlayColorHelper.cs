using System.Windows.Media;

namespace AudioMonitor.OverlayRenderer
{
    public static class OverlayColorHelper
    {
        public static Color FromHex(string hex, double alpha = 1.0)
        {
            if (string.IsNullOrWhiteSpace(hex)) return Colors.Transparent;
            if (hex.StartsWith("#")) hex = hex.Substring(1);
            byte a = (byte)(alpha * 255);
            byte r = 0, g = 0, b = 0;
            if (hex.Length == 6)
            {
                r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                r = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return Color.FromArgb(a, r, g, b);
        }
    }
}
