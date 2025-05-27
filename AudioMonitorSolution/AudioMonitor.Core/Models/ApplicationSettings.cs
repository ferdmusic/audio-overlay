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
        public int OverlayThickness { get; set; } // Changed from OverlayHeight
        public bool AcousticWarningEnabled { get; set; }
        public double AcousticWarningVolume { get; set; } // 0.0 to 1.0
        public bool AutostartEnabled { get; set; }
        public WarningConfiguration WarningLevels { get; set; }

        // New properties for threshold levels
        public double ThresholdSafe { get; set; }
        public double ThresholdWarning { get; set; }
        public double ThresholdCritical { get; set; }


        public ApplicationSettings()
        {
            OverlayPosition = OverlayEdge.Top;
            OverlayThickness = 5; // Default thickness
            AcousticWarningEnabled = false;
            AcousticWarningVolume = 0.75;
            AutostartEnabled = false;
            WarningLevels = new WarningConfiguration();
            // Default threshold values
            ThresholdSafe = -12;
            ThresholdWarning = -6;
            ThresholdCritical = -3;
        }

        public static ApplicationSettings GetDefault()
        {
            var settings = new ApplicationSettings
            {
                SelectedAudioDeviceId = null,
                OverlayPosition = OverlayEdge.Top,
                OverlayThickness = 5, // Default thickness
                AcousticWarningEnabled = false,
                AcousticWarningVolume = 0.75,
                AutostartEnabled = false,
                WarningLevels = WarningConfiguration.GetDefault(),
                // Default threshold values
                ThresholdSafe = -12,
                ThresholdWarning = -6,
                ThresholdCritical = -3
            };
            return settings;
        }
    }
}
