# Audio Level Monitor

<p align="center">
  <img src="AudioMonitorSolution/image.ico" alt="Audio Level Monitor Icon" width="128"/>
</p>

**Version:** 1.2

## 1. What it Does

This app warns you in real time if your microphone input levels on Windows 11 are too high. You set the limits. You see warnings as a visual overlay on your screen's edge. You can also choose to hear a sound. This app is built to be fast and easy to maintain.

A main goal is to give you quick, clear, and light feedback on your input levels. This helps you keep your audio quality high. The app is built to be stable and easy to update.

**Who is this for?** If you make music, podcasts, or stream, and you use an audio interface (like a Focusrite Scarlett) on Windows 11, this app can help. It's for people who want a reliable way to monitor audio levels without slowing down their computer.

---

## 2. Features

*   **Choose Your Audio Device (FA01):**
    *   You can pick your specific audio interface and input channel.
    *   The app automatically finds your connected compatible audio devices (e.g., Focusrite).
    *   If you have many devices, you can choose the one you want.
*   **See Audio Levels Live (FA02):**
    *   The app constantly checks the audio level of the microphone you choose.
    *   Levels are shown in dBFS (Decibels relative to Full Scale).
*   **Set Warning Levels and Colors (FA03):**
    *   You can create different level stages (like "Safe," "Caution," "Too Loud").
    *   You set a dBFS limit for each stage.
    *   The visual overlay will change colors based on these stages (for example, green to yellow to red).
    *   Your settings are saved.
*   **Visual Overlay Warning (FA04):**
    *   **Look and Place:** A thin bar appears on the edge of your monitors. You choose which edge. It shows your current level with changing colors. It updates smoothly and won't slow down your system.
    *   **Changes Transparency:** When levels are low, the bar is almost invisible. As levels rise, it becomes easier to see and changes color.
    *   **When Levels are Critical:** If your audio hits the "Too Loud" level, the bar stays clearly visible in that color (like red) for at least two seconds. This happens even if the level drops quickly.
    *   **Overlay Options:** You can change how thick the bar is and pick the monitor edge.
*   **Sound Warning (FA05):**
    *   You can turn on a sound warning. It plays a short sine wave when levels pass a limit you set.
    *   You can turn this sound on or off. You can also set its volume.
*   **Settings Menu (FA06):**
    *   A menu lets you:
        *   Choose your audio device.
        *   Set up levels and colors.
        *   Change overlay settings.
        *   Turn sound warnings on/off.
        *   Set the app to start with Windows.
        *   Start/stop the app or send it to the system tray.
*   **Runs in Background / System Tray / Autostart (FA07):**
    *   The app uses few resources while running in the background.
    *   A system tray icon provides quick access and status indication.
    *   You can set the app to start when Windows starts.
*   **Saves Your Settings (FA08):**
    *   The app remembers all your choices for the next time you start it.

---

## 3. How it Performs and How It Is Built

*   **Performance (Very Important):**
    *   **Fast Response:** You won't notice a delay between a sound event and the warning (aiming for much less than 50ms).
    *   **Low CPU Use:** The app uses very little CPU power, especially when just monitoring. Even with warnings active, it won't slow down your other apps (like recording software or games).
    *   **Uses Memory Well:** It needs little memory and won't have memory leaks.
    *   **Smooth Visuals:** Overlay changes (color, transparency) are smooth.
    *   **Smart Rendering:** The overlay uses your GPU when it makes sense (for example, with DirectX and WPF/WinUI) to keep CPU use low.
    *   **Doesn't Wake CPU Often:** Background work only uses the CPU when needed. It prefers event-based actions.
*   **Code Quality (Important):**
    *   **Organized Code:** The code is split into clear parts (like audio engine, UI, overlay, settings). Each part does its job well.
    *   **Easy to Read:** The code is clear and consistent. It follows Microsoft C# Coding Conventions. Comments explain why something is done, not just what.
    *   **Testable:** Key parts (like audio processing, level math, warning logic) can be tested automatically.
    *   **SOLID Design:** SOLID principles are used for flexible and maintainable code.
    *   **No Magic Values:** Named constants or config files are used instead of hardcoded numbers or text.
    *   **Manages Resources:** The app frees up resources it no longer needs (especially for audio and graphics).
    *   **Async Code:** `async`/`await` is used correctly to keep the UI responsive.
*   **Easy to Use:** Simple to set up and understand, even with complex tech behind it. Clear feedback from the overlay and optional sound.
*   **Reliable:** The app runs stably for long periods. It measures levels accurately. It handles errors well (like if an audio device disconnects) and informs you instead of crashing.
*   **Works With:**
    *   **OS:** Windows 11.
    *   **Audio Drivers:** Works well with audio interface drivers (ASIO is best for low delay, WASAPI is an option).
    *   **Multiple Monitors:** Works correctly and fast with several monitors of different sizes and settings.

---

## 4. Tech Used

*   **.NET / C#:** The project is built with the .NET framework.
*   **WPF/WinUI:** Used for the UI and overlay, based on `.xaml` files and the need for GPU use.
*   **NAudio:** Used for audio tasks and ASIO/WASAPI.

---

## 5. How the Project is Organized

The solution has these main projects:

*   `AudioMonitor.Core`: Handles audio processing, level analysis, and settings.
*   `AudioMonitor.OverlayRenderer`: Draws the visual overlay.
*   `AudioMonitor.UI`: Shows the main app window, settings screen, and system tray functions.

---

## 6. How to Get Started

(Details on how to build and run the project will be added here.)

### You Need

*   Windows 11
*   .NET SDK (check `global.json` or project files for the version)
*   A compatible audio interface and its drivers (for example, a Focusrite Scarlett)

### Install & Run

(Instructions for cloning, building, and running the app will be added here.)

---

## 7. How to Configure

You can change many settings in the app's menu:
*   Audio input device and channel
*   Warning levels and their colors
*   Overlay position and how it acts
*   Sound warning choices
*   Autostart setting

The app saves all your settings.

---

## 8. Examples of Use

*   **UC01: First Setup:** Choose your audio source, set levels, pick overlay style, turn on sound warnings, and set autostart.
*   **UC02: Levels Too High:** See the overlay change (transparency, color) and hear a sound when audio levels go up and cross your set limits.
*   **UC03: Change Warning Levels:** Go to settings and change the limits.
*   **UC04: Turn Monitoring On/Off Quickly:** Use the tray icon or settings to enable/disable monitoring or sound warnings.

---

## 9. Not Included (for V1.0)

*   Recording or editing audio in this app.
*   Advanced audio analysis tools (like a spectrogram).
*   Network features.
*   Support for operating systems other than Windows 11.
*   Multiple languages (Mainly German; more might be added later).

---

## 10. How to Contribute

(Details will be added here. For now, please follow the current code style.)

---

## 11. License

(License information will be added here.)
