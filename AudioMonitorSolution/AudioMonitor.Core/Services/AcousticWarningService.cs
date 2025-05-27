// Filepath: c:\Users\Ferdmusic\Documents\GitHub\audio-overlay\AudioMonitorSolution\AudioMonitor.Core\Services\AcousticWarningService.cs
using AudioMonitor.Core.Logging;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioMonitor.Core.Services
{
    public class AcousticWarningService : IDisposable
    {
        private WaveOutEvent? _waveOutDevice;
        // private SineWaveProvider16? _sineWaveProvider; // Replaced
        private SignalGenerator? _signalGenerator; // Added
        private bool _isPlaying = false;
        private object _playbackLock = new object();

        public AcousticWarningService()
        {
        }

        public void PlayWarningSound(double volume, double frequency = 440.0, int durationMilliseconds = 300)
        {
            lock (_playbackLock)
            {
                if (_isPlaying)
                {
                    // Log.Debug("AcousticWarningService: Already playing, request ignored.");
                    return; // Don't interrupt or overlap sounds
                }
                _isPlaying = true;
            }

            try
            {
                // Dispose previous instances if they exist and were not cleaned up properly
                _waveOutDevice?.Dispose();
                // _signalGenerator is not IDisposable, but its source for WaveOutEvent will be

                // _sineWaveProvider = new SineWaveProvider16 // Replaced
                // {
                //     Frequency = (float)frequency,
                //     Amplitude = 0.5f 
                // };

                _signalGenerator = new SignalGenerator() // Added
                {                                          // Added
                    Gain = 0.2, // Amplitude for SignalGenerator (0.0 to 1.0) - 0.5 might be too loud initially
                    Frequency = frequency,                 // Added
                    Type = SignalGeneratorType.Sin         // Added
                };                                         // Added

                _waveOutDevice = new WaveOutEvent
                {
                    DesiredLatency = 200,
                    NumberOfBuffers = 2,
                    Volume = (float)Math.Clamp(volume, 0.0, 1.0)
                };

                // var timedProvider = _sineWaveProvider.Take(TimeSpan.FromMilliseconds(durationMilliseconds)); // Replaced
                var timedProvider = _signalGenerator.Take(TimeSpan.FromMilliseconds(durationMilliseconds)); // Added

                _waveOutDevice.Init(timedProvider);

                _waveOutDevice.PlaybackStopped += (sender, args) =>
                {
                    lock (_playbackLock)
                    {
                        _isPlaying = false;
                    }
                    // Clean up the WaveOutEvent after playback has stopped
                    _waveOutDevice?.Dispose();
                    _waveOutDevice = null;
                    // Log.Debug("AcousticWarningService: Playback stopped and resources released.");
                };

                _waveOutDevice.Play();
                // Log.Debug($"AcousticWarningService: Playing warning sound. Volume: {volume}, Freq: {frequency}, Duration: {durationMilliseconds}ms");
            }
            catch (Exception ex)
            {
                Log.Error("AcousticWarningService: Error playing warning sound.", ex);
                lock (_playbackLock)
                {
                    _isPlaying = false; // Reset flag on error
                }
                // Clean up in case of an error during setup or play
                _waveOutDevice?.Dispose();
                _waveOutDevice = null;
            }
        }

        public void Dispose()
        {
            lock (_playbackLock)
            {
                if (_waveOutDevice != null)
                {
                    if (_waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        _waveOutDevice.Stop();
                    }
                    _waveOutDevice.Dispose();
                    _waveOutDevice = null;
                }
                _isPlaying = false;
            }
            GC.SuppressFinalize(this);
            // Log.Debug("AcousticWarningService: Disposed.");
        }
    }
}
