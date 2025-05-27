using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace AudioMonitor.UI
{
    public partial class App : Application
    {
        private NotifyIcon? _trayIcon;
        private MainWindow? _mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _mainWindow = new MainWindow();
            _mainWindow.Show(); // Show main window on startup
            SetupTray();
        }

        private void SetupTray()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = SystemIcons.Information;
            _trayIcon.Visible = true;
            _trayIcon.Text = "AudioMonitor";

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Show Settings", null, (s, e) => ShowSettings());
            contextMenu.Items.Add("Enable/Disable Monitoring", null, (s, e) => ToggleMonitoring());
            contextMenu.Items.Add("Exit", null, (s, e) => ExitApp());
            _trayIcon.ContextMenuStrip = contextMenu;
            _trayIcon.DoubleClick += (s, e) => ShowSettings();
        }

        private void ShowSettings()
        {
            if (_mainWindow == null) return;
            _mainWindow.Show();
            _mainWindow.WindowState = System.Windows.WindowState.Normal;
            _mainWindow.Activate();
        }

        private void ToggleMonitoring()
        {
            if (_mainWindow is not null)
                _mainWindow.ToggleMonitoring();
        }

        private void ExitApp()
        {
            _trayIcon?.Dispose();
            Shutdown();
        }
    }
}
