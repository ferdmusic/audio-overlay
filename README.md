<p align="center">
  <img src="AudioMonitorSolution/image.ico" alt="Audio Level Monitor Icon" width="128"/>
</p>

# 🎵 Audio Overlay

A simple Windows application for audio monitoring and overlay functionality.

## 📥 Download

**[Latest Release (v1.0.0)](https://github.com/ferdmusic/audio-overlay/releases/latest)** - Download `AudioMonitor-Windows-x64.exe`

## 🛡️ Security Notice - SmartScreen Warning

**tl;dr: This app is safe, but Windows doesn't know that yet.**

When you download and run this app, Windows will probably show you a scary red warning saying "Windows protected your PC" (or in German: "Der Computer wurde durch Windows geschützt"). **This is a false positive** - the app is totally safe.

### Why does this happen?
- Windows SmartScreen doesn't trust apps without expensive certificates ($250-700/year)
- New apps need to build "reputation" over time
- It's super common for indie/open-source software

### Proof it's safe
🔗 **[VirusTotal Scan Results](https://www.virustotal.com/gui/file/2575644b699e69f107f43817cdb0e7f73c53e1787ce216af7ddbf40ebdd8daf5/detection)** - Only 2/71 security vendors flag it (clear false positive)

### How to run it anyway
**Option 1:** Click "More info" → "Run anyway" when the warning pops up

**Option 2:** Right-click the `.exe` file → Properties → Check "Unblock" → OK

## 🚀 What does it do?

Audio Overlay is a Windows application designed for real-time monitoring of your microphone's input levels. It's particularly useful for ensuring your audio doesn't clip or stay too quiet, providing immediate visual feedback. While it works with any compatible audio input, it has been developed with Focusrite Scarlett interfaces in mind.

Key features:
* **Visual Level Meter:** Displays a configurable colored bar overlay on the edge of your monitor(s). The color and opacity of the bar change dynamically based on the audio input level relative to defined thresholds.
* **Configurable Thresholds:** Set dBFS levels for "Safe," "Warning," and "Critical" audio states to customize the visual feedback.
* **Multi-Monitor Support:** Overlays can be displayed on all connected monitors.
* **Acoustic Alerts:** Optionally, enable an acoustic sine wave warning when audio levels reach the "Critical" threshold.
* **Audio Source Flexibility:** Supports both WASAPI and ASIO (if ASIO drivers are present and `ASIO_SUPPORT` was enabled at compile-time) audio inputs.
* **Customization:** Adjust overlay position, thickness, acoustic warning volume, and application language (English/German).
* **Autostart:** Configure the application to start automatically with Windows.
* **Tray Control:** Minimizes to the system tray with options to show the main window, open settings, reset settings, or exit the application.

The application aims to provide immediate, precise, and resource-efficient audio level feedback.

## 💻 System Requirements

* **Operating System:** Windows (x64)
* **.NET:** .NET 8.0 Runtime (the provided executable is self-contained, but .NET 8.0 is the target framework)
* **Audio Interface:** Any Windows-compatible audio input device (e.g., Focusrite Scarlett, other microphones).
* **Drivers:** WASAPI (standard Windows audio) or ASIO drivers (for lower latency, if supported by your device and `ASIO_SUPPORT` was enabled at compile-time).

## 🔧 Usage

1.  **Download and Run:**
    * Download `AudioMonitor-Windows-x64.exe` from the [Latest Release](https://github.com/ferdmusic/audio-overlay/releases/latest).
    * Run the executable. You might encounter the SmartScreen warning mentioned above; follow the steps to run it.

2.  **Main Window & Tray Icon:**
    * The application may open a small main window or start minimized to the system tray.
    * The main window allows for quick audio device selection and shows the current monitoring status.
    * Right-click the tray icon for options:
        * **Show:** Opens the main application window.
        * **Settings:** Opens the settings window.
        * **Reset Settings:** Resets all settings to their default values after confirmation.
        * **Exit:** Closes the application completely.
    * The tray icon's tooltip also indicates the current status (e.g., Monitoring, Paused, Critical).

3.  **Configuring Settings:**
    * Open the Settings window via the tray icon menu or a button in the main window (if available).
    * **Audio Input Device:** Select your microphone or audio interface from the list. *See Known Issues below.*
    * **Overlay Position:** Choose which edge of the screen(s) the overlay bar will appear on (Top, Bottom, Left, Right).
    * **Overlay Thickness:** Set the thickness of the overlay bar in pixels.
    * **dBFS Thresholds:** Define the audio levels (in dBFS) for "Safe," "Warning," and "Critical" states. The overlay will change color based on these.
    * **Acoustic Warnings:**
        * Enable or disable an audible beep/sine wave when the audio level hits "Critical".
        * Adjust the volume of this warning sound.
    * **Autostart with Windows:** Check this to have Audio Overlay launch when you log into Windows.
    * **Language:** Choose between English and German. A restart might be prompted or required for the change to fully take effect.
    * Click **Save Settings** to apply your changes.

4.  **Monitoring:**
    * Once configured, the overlay bar(s) will appear on the selected edge of your screen(s).
    * The bar's color and opacity will change based on the live audio input level relative to your set thresholds. If levels become critical, the overlay becomes fully opaque and remains so for at least two seconds.

## ✨ Known Issues

* **Device Selection in Settings:** Audio device selection made within the Settings window might not apply correctly or immediately. For reliable audio device switching, please use the device selector dropdown on the main application window.

## 🐛 Issues?

If you run into problems:
- Check the [Known Issues](#known-issues) section above.
- Check the [Issues](https://github.com/ferdmusic/audio-overlay/issues) page on GitHub.
- Create a new issue if your problem isn't listed.
- Please include your Windows version, what audio device you are using, and what you were trying to do when the problem occurred.

## 📜 License

This project is provided with the intention of being freely used, modified, and distributed. The author expresses no specific restrictions ("I don't care what people do with the software").
If a formal declaration is desired for your purposes, consider this project under a highly permissive open-source license like **The Unlicense** or **MIT License**. The project owner may choose to add a formal `LICENSE` file in the future.

## ⭐ Support

If this helped you out, consider giving it a star on GitHub! It helps other people find the project.

---

*Made with ☕ by [ferdmusic](https://github.com/ferdmusic)*
