using System.Collections.Generic;

namespace AudioMonitor.Core.Models
{
    public enum OverlayEdge
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public class ApplicationSettings
    {
        public string? SelectedAudioDeviceId { get; set; }
        public OverlayEdge OverlayPosition { get; set; }
        public int OverlayHeight { get; set; } // in pixels
        public bool AcousticWarningEnabled { get; set; }
        public double AcousticWarningVolume { get; set; } // 0.0 to 1.0
        public bool AutostartEnabled { get; set; }
        public WarningConfiguration WarningLevels { get; set; }

        public ApplicationSettings()
        {
            OverlayPosition = OverlayEdge.Top;
            OverlayHeight = 10;
            AcousticWarningEnabled = false;
            AcousticWarningVolume = 0.75;
            AutostartEnabled = false;
            WarningLevels = new WarningConfiguration();
        }

        public static ApplicationSettings GetDefault()
        {
            var settings = new ApplicationSettings
            {
                SelectedAudioDeviceId = null,
                OverlayPosition = OverlayEdge.Top,
                OverlayHeight = 10,
                AcousticWarningEnabled = false,
                AcousticWarningVolume = 0.75,
                AutostartEnabled = false,
                WarningLevels = WarningConfiguration.GetDefault()
            };
            return settings;
        }
    }
}
