// In AudioMonitor.Core/Services/AudioService.cs
using AudioMonitor.Core.Logging;
using NAudio.CoreAudioApi; // For MMDeviceEnumerator
using NAudio.Wave;
using System.Linq;

#if ASIO_SUPPORT
// We need AsioDriver for GetAsioDriverNames()
#endif

namespace AudioMonitor.Core.Services
{    public enum AudioDeviceType
    {
        WASAPI,
        ASIO
    }

    public class AudioDevice
    {
        public string? Name { get; set; }
        public string? Id { get; set; } // Using string DeviceID from MMDevice
        public bool IsFocusrite { get; set; }
        public AudioDeviceType DeviceType { get; set; }

        public override string ToString() => Name ?? "Unknown Device";
    }

    public class AudioDeviceGroup
    {
        public string GroupName { get; set; } = "";
        public List<AudioDevice> Devices { get; set; } = new List<AudioDevice>();
    }

    public class AudioService : IDisposable
    {
        private IWaveIn? _waveInDevice; // Renamed from _capture for clarity
#if ASIO_SUPPORT
        private AsioOut? _asioOutDevice; // For ASIO operations
#endif
        private string? _monitoringDeviceId;
        // private float[]? _asioBuffer; // CS0169: Field is never used. Commenting out for now

        public string? CurrentDeviceId => _monitoringDeviceId;

        public event EventHandler<double>? LevelChanged; // Made nullable
        public double CurrentDBFSLevel { get; private set; } = -96.0; // Default to a low value, representing silence

        public List<AudioDevice> GetInputDevices()
        {
            var devices = new List<AudioDevice>();
            var enumerator = new MMDeviceEnumerator();
            // Enumerate WASAPI Capture Devices
            try
            {
                var mmDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active).ToList();
                Log.Info($"Found {mmDevices.Count} active WASAPI capture devices.");
                for (int i = 0; i < mmDevices.Count; i++)
                {
                    var device = mmDevices[i];
                    bool isFocusrite = device.FriendlyName.ToLowerInvariant().Contains("focusrite");                    devices.Add(new AudioDevice
                    {
                        Name = device.FriendlyName,
                        Id = device.ID,
                        IsFocusrite = isFocusrite,
                        DeviceType = AudioDeviceType.WASAPI
                    });
                    Log.Info($"Device {i}: Name='{device.FriendlyName}', ID='{device.ID}', IsFocusrite={isFocusrite}");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error enumerating WASAPI input devices.", ex);
            }

#if ASIO_SUPPORT
            // Enumerate ASIO Drivers
            try
            {
                // Use fully qualified name for AsioDriver to be safe.
                var asioDriverNames = NAudio.Wave.Asio.AsioDriver.GetAsioDriverNames();
                Log.Info($"Found {asioDriverNames.Length} ASIO drivers.");
                for (int i = 0; i < asioDriverNames.Length; i++)
                {
                    string driverName = asioDriverNames[i];
                    bool isFocusrite = driverName.ToLowerInvariant().Contains("focusrite");                    devices.Add(new AudioDevice
                    {
                        Name = $"ASIO: {driverName}",
                        Id = $"ASIO:{driverName}",
                        IsFocusrite = isFocusrite,
                        DeviceType = AudioDeviceType.ASIO
                    });
                    Log.Info($"ASIO Driver {i}: Name='{driverName}', IsFocusrite={isFocusrite}");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error enumerating ASIO drivers. Ensure ASIO drivers are installed.", ex);
            }
#endif
            if (!devices.Any())
            {
                Log.Info("No active input devices found. For microphone input, ensure it's enabled and not exclusively used by another application.");
            }
            return devices;
        }

        public List<AudioDeviceGroup> GetGroupedInputDevices()
        {
            var devices = GetInputDevices();
            var groups = new List<AudioDeviceGroup>();

            // Group WASAPI devices
            var wasapiDevices = devices.Where(d => d.DeviceType == AudioDeviceType.WASAPI).ToList();
            if (wasapiDevices.Any())
            {
                groups.Add(new AudioDeviceGroup
                {
                    GroupName = "WASAPI Devices",
                    Devices = wasapiDevices
                });
            }

            // Group ASIO devices
            var asioDevices = devices.Where(d => d.DeviceType == AudioDeviceType.ASIO).ToList();
            if (asioDevices.Any())
            {
                groups.Add(new AudioDeviceGroup
                {
                    GroupName = "ASIO Devices",
                    Devices = asioDevices
                });
            }

            return groups;
        }

        public void StartMonitoring(string deviceId)
        {
            StopMonitoring(); // Stop any existing monitoring

            _monitoringDeviceId = deviceId;

#if ASIO_SUPPORT
            if (deviceId != null && deviceId.StartsWith("ASIO:"))
            {
                string driverName = deviceId.Substring(5);
                try
                {
                    Log.Info($"Attempting to initialize ASIO driver: {driverName}");
                    _asioOutDevice = new AsioOut(driverName);

                    // Configure for recording.
                    // TODO: Allow configuration of desiredChannels and desiredSampleRate
                    int desiredChannels = _asioOutDevice.DriverInputChannelCount > 0 ? Math.Min(2, _asioOutDevice.DriverInputChannelCount) : 1; // Default to 1 or 2 channels
                    int desiredSampleRate = 44100; // Or a supported rate from driver capabilities

                    Log.Info($"ASIO: Initializing with {desiredChannels} channels at {desiredSampleRate} Hz. Driver input channels: {_asioOutDevice.DriverInputChannelCount}");

                    _asioOutDevice.InitRecordAndPlayback(
                        null, // No playback source, only recording
                        desiredChannels,
                        desiredSampleRate);

                    // Optional: Set input channel offset if you don't want to record from the first channels
                    // _asioOutDevice.InputChannelOffset = 0; // Default is 0

                    _asioOutDevice.AudioAvailable += OnAsioAudioAvailable;

                    _asioOutDevice.Play(); // Starts the ASIO engine (and thus recording)
                    Log.Info($"Started ASIO monitoring on driver: {driverName}");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to start ASIO monitoring on driver: {driverName}", ex);
                    _asioOutDevice?.Dispose();
                    _asioOutDevice = null;
                    _monitoringDeviceId = null; // Clear monitoring device ID on failure
                    return;
                }
            }
#endif
            // Fallback to WASAPI if not ASIO or ASIO_SUPPORT is not defined
            try
            {
                var enumerator = new MMDeviceEnumerator();
                MMDevice? captureDevice = null;

                if (string.IsNullOrEmpty(deviceId))
                {
                    captureDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                    if (captureDevice == null)
                    {
                        Log.Error("No default WASAPI capture device found.");
                        _monitoringDeviceId = null;
                        return;
                    }
                    _monitoringDeviceId = captureDevice.ID;
                    Log.Info($"No device ID specified, using default WASAPI capture device: {captureDevice.FriendlyName} (ID: {_monitoringDeviceId})");
                }
                else
                {
                    try
                    {
                        captureDevice = enumerator.GetDevice(deviceId);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error getting WASAPI device with ID \'{deviceId}\'.", ex);
                        _monitoringDeviceId = null;
                        return;
                    }
                }

                if (captureDevice == null)
                {
                    Log.Error($"WASAPI Capture device with ID \'{_monitoringDeviceId ?? deviceId}\' not found or could not be initialized.");
                    _monitoringDeviceId = null;
                    return;
                }

                _waveInDevice = new WasapiCapture(captureDevice);
                _waveInDevice.DataAvailable += OnWaveInDataAvailable;
                _waveInDevice.RecordingStopped += OnWaveInRecordingStopped;

                Log.Info($"Starting WASAPI monitoring on device: {captureDevice.FriendlyName} (ID: {_monitoringDeviceId}) WaveFormat: {_waveInDevice.WaveFormat} Hz, {_waveInDevice.WaveFormat.Channels} channels, {_waveInDevice.WaveFormat.BitsPerSample}-bit");
                _waveInDevice.StartRecording();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start WASAPI monitoring on device ID: {_monitoringDeviceId ?? deviceId}", ex);
                _waveInDevice?.Dispose();
                _waveInDevice = null;
                _monitoringDeviceId = null;
            }
        }

#if ASIO_SUPPORT
        private void OnAsioAudioAvailable(object? sender, AsioAudioAvailableEventArgs e)
        {
            if (_asioOutDevice == null) return;

            // Get samples as interleaved floats. This is often the easiest way to process.
            float[] samples = e.GetAsInterleavedSamples();

            if (samples.Length == 0)
            {
                CurrentDBFSLevel = -96.0;
                LevelChanged?.Invoke(this, CurrentDBFSLevel);
                return;
            }

            float maxSample = 0f;
            for (int i = 0; i < samples.Length; i++)
            {
                float absSample = Math.Abs(samples[i]);
                if (absSample > maxSample)
                {
                    maxSample = absSample;
                }
            }

            if (maxSample == 0)
            {
                CurrentDBFSLevel = -96.0;
            }
            else
            {
                CurrentDBFSLevel = 20 * Math.Log10(maxSample);
            }
            LevelChanged?.Invoke(this, CurrentDBFSLevel);
        }
#endif

        private void OnWaveInDataAvailable(object? sender, WaveInEventArgs e) // Renamed from OnDataAvailable
        {
            if (_waveInDevice == null || e.BytesRecorded == 0)
            {
                CurrentDBFSLevel = -96.0; // Silence or no data
                LevelChanged?.Invoke(this, CurrentDBFSLevel);
                return;
            }

            float maxSample = 0f;

            if (_waveInDevice.WaveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                for (int index = 0; index < e.BytesRecorded; index += 4) // 4 bytes per float
                {
                    float sample = BitConverter.ToSingle(e.Buffer, index);
                    float absSample = Math.Abs(sample);
                    if (absSample > maxSample)
                    {
                        maxSample = absSample;
                    }
                }
            }
            else if (_waveInDevice.WaveFormat.Encoding == WaveFormatEncoding.Pcm)
            {
                // Handle PCM data (e.g., 16-bit, 24-bit, 32-bit integer)
                if (_waveInDevice.WaveFormat.BitsPerSample == 16)
                {
                    for (int index = 0; index < e.BytesRecorded; index += 2) // 2 bytes per 16-bit sample
                    {
                        short sample = BitConverter.ToInt16(e.Buffer, index);
                        float sampleFloat = sample / 32768f; // Convert to -1.0 to 1.0 range
                        float absSample = Math.Abs(sampleFloat);
                        if (absSample > maxSample)
                        {
                            maxSample = absSample;
                        }
                    }
                }
                else if (_waveInDevice.WaveFormat.BitsPerSample == 24)
                {
                    for (int i = 0; i < e.BytesRecorded / 3; i++)
                    {
                        byte byte1 = e.Buffer[i * 3];
                        byte byte2 = e.Buffer[i * 3 + 1];
                        byte byte3 = e.Buffer[i * 3 + 2];
                        int sample24 = (byte3 << 16) | (byte2 << 8) | byte1;
                        // Sign extend if negative
                        if ((sample24 & 0x800000) != 0) sample24 |= ~0xFFFFFF;
                        float sampleFloat = sample24 / 8388608f; // Convert to -1.0 to 1.0 range
                        float absSample = Math.Abs(sampleFloat);
                        if (absSample > maxSample) maxSample = absSample;
                    }
                }
                else if (_waveInDevice.WaveFormat.BitsPerSample == 32)
                {
                    for (int index = 0; index < e.BytesRecorded; index += 4) // 4 bytes per 32-bit sample
                    {
                        int sample = BitConverter.ToInt32(e.Buffer, index);
                        float sampleFloat = sample / 2147483648f; // Convert to -1.0 to 1.0 range
                        float absSample = Math.Abs(sampleFloat);
                        if (absSample > maxSample)
                        {
                            maxSample = absSample;
                        }
                    }
                }
                // Add other PCM formats as needed
            }
            else
            {
                // Log an unsupported format or handle appropriately
                Log.Warning($"Unsupported wave format for level calculation: {_waveInDevice.WaveFormat.Encoding}");
                CurrentDBFSLevel = -96.0;
                LevelChanged?.Invoke(this, CurrentDBFSLevel);
                return;
            }

            if (maxSample == 0) // Avoid Log10(0)
            {
                CurrentDBFSLevel = -96.0; // Represent silence
            }
            else
            {
                // dBFS = 20 * log10(peakSampleValue / maxPossibleSampleValue)
                // For 32-bit float samples from NAudio, maxPossibleSampleValue is 1.0.
                CurrentDBFSLevel = 20 * Math.Log10(maxSample);
            }
            LevelChanged?.Invoke(this, CurrentDBFSLevel);
            // Log.Debug($"Current dBFS: {CurrentDBFSLevel:F2} (Peak Sample: {maxSample:F4})"); // Can be very noisy
        }

        private void OnWaveInRecordingStopped(object? sender, StoppedEventArgs e) // Renamed from OnRecordingStopped
        {
            Log.Info($"WASAPI Monitoring stopped for device ID: {_monitoringDeviceId}.");
            if (e.Exception != null)
            {
                Log.Error("Recording stopped with an error.", e.Exception);
            }
        }

        public void StopMonitoring()
        {
#if ASIO_SUPPORT
            if (_asioOutDevice != null)
            {
                Log.Info($"Stopping ASIO monitoring on device ID: {_monitoringDeviceId}");
                _asioOutDevice.Stop();
                _asioOutDevice.AudioAvailable -= OnAsioAudioAvailable; // Unsubscribe
                _asioOutDevice.Dispose();
                _asioOutDevice = null;
            }
#endif
            if (_waveInDevice != null)
            {
                Log.Info($"Stopping WASAPI monitoring on device ID: {_monitoringDeviceId}");
                _waveInDevice.StopRecording();
                // Event unsubscription is handled in Dispose or when re-creating _waveInDevice
                _waveInDevice.Dispose();
                _waveInDevice = null;
            }

            if (_monitoringDeviceId != null)
            {
                _monitoringDeviceId = null;
                CurrentDBFSLevel = -96.0; // Reset level
                LevelChanged?.Invoke(this, CurrentDBFSLevel); // Notify level reset
            }
        }

        public void Dispose()
        {
            StopMonitoring(); // Centralized stop logic

            // Unsubscribe from events to prevent memory leaks
            if (LevelChanged != null)
            {
                foreach (Delegate d in LevelChanged.GetInvocationList())
                {
                    LevelChanged -= (EventHandler<double>)d;
                }
            }

#if ASIO_SUPPORT
            // _asioOutDevice is already handled in StopMonitoring()
            // No need to check for AsioCapture anymore as we've switched to AsioOut
#endif
            if (_waveInDevice is WasapiCapture wasapiCaptureInstance)
            {
                // These are typically unsubscribed when _waveInDevice is set to null or re-assigned,
                // but explicit unsubscription here before nullifying is safer if StopMonitoring wasn't called.
                // However, StopMonitoring should handle this.
                // wasapiCaptureInstance.DataAvailable -= OnWaveInDataAvailable;
                // wasapiCaptureInstance.RecordingStopped -= OnWaveInRecordingStopped;
            }
            // _waveInDevice is already handled in StopMonitoring()

            Log.Info("AudioService disposed and monitoring stopped.");
            GC.SuppressFinalize(this); // Prevent finalizer from running if Dispose is called
        }

        // Optional: Add a finalizer if there are unmanaged resources directly owned by this class
        // ~AudioService()
        // {
        //     Dispose(false); // Dispose unmanaged resources
        // }

        // protected virtual void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {
        //         // Dispose managed resources
        //         _capture?.Dispose();
        //     }
        //     // Dispose unmanaged resources
        // }

    } // This is the closing brace for the AudioService class
} // This is the closing brace for the namespace AudioMonitor.Core.Services
