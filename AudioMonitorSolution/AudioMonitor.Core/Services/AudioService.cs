// In AudioMonitor.Core/Services/AudioService.cs
using NAudio.Wave;
using NAudio.CoreAudioApi; // For MMDeviceEnumerator
using System;
using System.Collections.Generic;
using System.Linq;
using AudioMonitor.Core.Logging;

#if ASIO_SUPPORT
using NAudio.Wave.Asio;
#endif

namespace AudioMonitor.Core.Services
{
    public class AudioDevice
    {
        public string Name { get; set; }
        public string Id { get; set; } // Using string DeviceID from MMDevice
        public bool IsFocusrite { get; set; }

        public override string ToString() => Name;
    }

    public class AudioService : IDisposable
    {
        private WasapiCapture? _capture; // Made nullable
        private string? _monitoringDeviceId; // Made nullable

        public string? CurrentDeviceId => _monitoringDeviceId; // Added public getter

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
                for(int i=0; i < mmDevices.Count; i++)
                {
                    var device = mmDevices[i];
                    bool isFocusrite = device.FriendlyName.ToLowerInvariant().Contains("focusrite");
                    devices.Add(new AudioDevice {
                        Name = device.FriendlyName,
                        Id = device.ID, 
                        IsFocusrite = isFocusrite
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
                var asioDriverNames = AsioOut.GetDriverNames();
                Log.Info($"Found {asioDriverNames.Length} ASIO drivers.");
                for (int i = 0; i < asioDriverNames.Length; i++)
                {
                    string driverName = asioDriverNames[i];
                    bool isFocusrite = driverName.ToLowerInvariant().Contains("focusrite");
                    devices.Add(new AudioDevice {
                        Name = $"ASIO: {driverName}",
                        Id = $"ASIO:{driverName}",
                        IsFocusrite = isFocusrite
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

        public void StartMonitoring(string deviceId)
        {
            if (_capture != null)
            {
                Log.Info("Monitoring is already active. Stop it before starting a new session.");
                return;
            }

#if ASIO_SUPPORT
            if (deviceId != null && deviceId.StartsWith("ASIO:"))
            {
                string driverName = deviceId.Substring(5);
                try
                {
                    var asio = new AsioOut(driverName);
                    _capture = asio;
                    asio.AudioAvailable += (s, e) =>
                    {
                        // e.Samples is float[]
                        float maxSample = 0f;
                        foreach (var sample in e.Samples)
                        {
                            float absSample = Math.Abs(sample);
                            if (absSample > maxSample) maxSample = absSample;
                        }
                        if (maxSample == 0)
                            CurrentDBFSLevel = -96.0;
                        else
                            CurrentDBFSLevel = 20 * Math.Log10(maxSample);
                        LevelChanged?.Invoke(this, CurrentDBFSLevel);
                    };
                    asio.Play();
                    Log.Info($"Started ASIO monitoring on driver: {driverName}");
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to start ASIO monitoring on driver: {driverName}", ex);
                    return;
                }
            }
#endif
            _monitoringDeviceId = deviceId;
            
            try
            {
                var enumerator = new MMDeviceEnumerator();
                MMDevice captureDevice = null;

                if (string.IsNullOrEmpty(deviceId)) // Default device
                {
                    captureDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Console);
                    if (captureDevice == null) {
                         Log.Error("No default capture device found.");
                         return;
                    }
                    _monitoringDeviceId = captureDevice.ID; // Store the actual ID
                     Log.Info($"No device ID specified, attempting to use default capture device: {captureDevice.FriendlyName} (ID: {_monitoringDeviceId})");
                }
                else
                {
                    captureDevice = enumerator.GetDevice(deviceId);
                }

                if (captureDevice == null)
                {
                    Log.Error($"Capture device with ID '{deviceId}' not found.");
                    return;
                }

                _capture = new WasapiCapture(captureDevice);
                _capture.DataAvailable += OnDataAvailable;
                _capture.RecordingStopped += OnRecordingStopped;
                
                Log.Info($"Starting monitoring on device: {captureDevice.FriendlyName} (ID: {_monitoringDeviceId}) WaveFormat: {_capture.WaveFormat} Hz, {_capture.WaveFormat.Channels} channels, {_capture.WaveFormat.BitsPerSample}-bit");
                _capture.StartRecording();
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to start monitoring on device ID: {deviceId}", ex);
                _capture?.Dispose(); // Clean up if start failed
                _capture = null;
            }
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e) // sender made nullable
        {
            if (e.BytesRecorded == 0)
            {
                CurrentDBFSLevel = -96.0; // Silence or no data
                LevelChanged?.Invoke(this, CurrentDBFSLevel);
                return;
            }

            float maxSample = 0f;
            // Assuming 32-bit float samples from WasapiCapture
            for (int index = 0; index < e.BytesRecorded; index += 4)
            {
                float sample = BitConverter.ToSingle(e.Buffer, index);
                float absSample = Math.Abs(sample);
                if (absSample > maxSample)
                {
                    maxSample = absSample;
                }
            }

            if (maxSample > 1.0f) maxSample = 1.0f; // Clamp to avoid issues if source is > 0dBFS (though unlikely with float)
            
            if (maxSample == 0)
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

        private void OnRecordingStopped(object? sender, StoppedEventArgs e) // sender made nullable
        {
            Log.Info($"Monitoring stopped for device ID: {_monitoringDeviceId}.");
            if (e.Exception != null)
            {
                Log.Error($"Recording stopped due to an exception on device ID: {_monitoringDeviceId}.", e.Exception);
            }
            // No need to dispose here, Dispose method will handle it or if starting new capture.
        }

        public void StopMonitoring()
        {
            if (_capture != null)
            {
                Log.Info($"Requesting to stop monitoring for device ID: {_monitoringDeviceId}.");
                _capture.StopRecording();
                // Dispose will be called from the main Dispose method or when starting a new capture.
                // For immediate cleanup if desired:
                // DisposeCaptureResources(); 
            }
        }
        
        private void DisposeCaptureResources()
        {
            if (_capture != null)
            {
                _capture.DataAvailable -= OnDataAvailable;
                _capture.RecordingStopped -= OnRecordingStopped;
                _capture.Dispose();
                _capture = null;
                Log.Info($"Capture resources disposed for device ID: {_monitoringDeviceId}.");
                _monitoringDeviceId = null;
                CurrentDBFSLevel = -96.0; // Reset level
                LevelChanged?.Invoke(this, CurrentDBFSLevel);
            }
        }

        public void Dispose()
        {
            DisposeCaptureResources();
        }
    }
}
