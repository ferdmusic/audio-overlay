# Audio Level Monitor

<p align="center">
  <img src="AudioMonitorSolution/image.ico" alt="Audio Level Monitor Icon" width="128"/>
</p>

**Version:** 1.2

## 1. What it Does

This app warns you in real time if your microphone input levels on Windows 11 are too high. You set the limits. Warnings appear as a visual overlay on your screen's edge, with an optional sound alert. The app is built for speed and easy maintenance, providing quick, clear feedback to help you maintain audio quality.

**Who is this for?** Musicians, podcasters, streamers, or anyone using an audio interface (e.g., Focusrite Scarlett) on Windows 11 who needs reliable, low-impact audio level monitoring.

---

## 2. Core Features

*   **Device Selection:** Pick your audio interface and input channel. Automatic detection of compatible devices.
*   **Real-time Metering:** See live audio levels in dBFS.
*   **Custom Warnings:** Define level stages ("Safe," "Caution," "Too Loud") with dBFS thresholds and corresponding overlay colors (e.g., green to yellow to red).
*   **Visual Overlay:** A thin, customizable bar on your chosen monitor edge shows levels with color and transparency changes. Critical levels remain visible for at least two seconds.
*   **Acoustic Alert:** Optional sine wave sound when levels exceed your set limit.
*   **Settings UI:** Manage all configurations, including autostart.
*   **Background Operation:** Runs efficiently in the background with a system tray icon for quick access.
*   **Persistent Settings:** Your configurations are saved.

---

## Language Settings

The AudioMonitor application supports multiple languages for the user interface. Currently available languages are:

*   English (Default)
*   German

To change the language:
1.  Open the **Settings** window.
2.  Locate the **Language** dropdown menu.
3.  Select your preferred language (English or Deutsch).
4.  Click **Save Settings**.
5.  **Important:** You will need to restart AudioMonitor for the language change to take full effect.

---

## 3. Performance & Design

*   **Performance:**
    *   **Low Latency:** Visual/acoustic response aims for <<50ms.
    *   **Minimal CPU/Memory Usage:** Designed to not impact other applications.
    *   **Smooth Visuals:** Fluid overlay animations.
    *   **GPU Accelerated Rendering:** Uses DirectX interop with WPF for efficiency.
*   **Code Quality:**
    *   **Modular:** Clear separation into Core, OverlayRendering, and UI logic.
    *   **Readable & Maintainable:** Follows C# conventions, with SOLID principles.
    *   **Testable:** Core components are designed for automated testing.

---

## 4. Tech Stack

*   **.NET / C#**
*   **WPF:** For the user interface and overlay rendering (`OverlayWindow.xaml`).
*   **NAudio (or similar):** For audio processing (ASIO/WASAPI interaction, `AudioService.cs`).

---

## 5. Project Structure

*   `AudioMonitor.Core`: Core logic (audio analysis via `LevelAnalyzer.cs`, settings via `SettingsService.cs`).
*   `AudioMonitor.OverlayRenderer`: Handles the visual overlay display.
*   `AudioMonitor.UI`: Main application, settings window (`App.xaml`), and autostart (`AutostartService.cs`).

---

## 6. Getting Started

(Build and run instructions to be added.)

**Prerequisites:**
*   Windows 11
*   .NET SDK (see `global.json`)
*   Compatible audio interface + drivers

---

## 7. Configuration

Access all settings through the app menu: audio device, warning levels/colors, overlay position/behavior, acoustic alerts, and autostart.

---

## 8. Use Cases

1.  **Initial Setup:** Configure audio source, levels, overlay, and autostart.
2.  **Monitoring:** Observe overlay changes and hear alerts as levels fluctuate.
3.  **Adjust Settings:** Modify thresholds as needed via the settings menu.

---

## 9. Out of Scope (V1.0)

*   Audio recording/editing.
*   Advanced audio analysis (spectrograms).
*   Network features.
*   Non-Windows 11 OS support.
*   Multi-language (currently German-focused, with future internationalization in mind).

---

## 10. Contributing

(Contribution guidelines to be added. Please adhere to existing code style.)

---

## 11. License

(License information to be added.)
