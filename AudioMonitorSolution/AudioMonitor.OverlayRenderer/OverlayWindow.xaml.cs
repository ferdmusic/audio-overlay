using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Interop; // Required for WindowInteropHelper
using System.Runtime.InteropServices; // Required for SetWindowLong etc.

namespace AudioMonitor.OverlayRenderer
{
    public partial class OverlayWindow : Window
    {
        private DispatcherTimer _fadeTimer;
        private double _targetOpacity = 0.1;
        private DateTime _criticalVisibleUntil = DateTime.MinValue;
        private bool _isCritical = false;

        // For click-through
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_NOACTIVATE = 0x08000000;


        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);


        public OverlayWindow()
        {
            InitializeComponent();
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None; // Ensure no standard window chrome
            this.Topmost = true;
            // this.IsHitTestVisible = false; // This is WPF's way, but SetWindowLong is more robust for true click-through
            this.Opacity = 0.1;
            _fadeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _fadeTimer.Tick += FadeTimer_Tick;
            _fadeTimer.Start();

            SourceInitialized += (s, e) =>
            {
                MakeWindowClickThrough();
            };
        }

        private void MakeWindowClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            // SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE); // WS_EX_NOACTIVATE prevents focus
            // For some overlays, WS_EX_TRANSPARENT alone is enough and preferred if you might want to interact with it programmatically later
            // However, for a visual-only overlay, both are good.
            // Let's try with just WS_EX_TRANSPARENT first as it's less restrictive.
            // If mouse events still get "stuck" on transparent parts of the window, then add WS_EX_NOACTIVATE.
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }

        public void SetOverlayColor(Color color, double opacity, bool isCritical)
        {
            OverlayBar.Fill = new SolidColorBrush(color);
            _targetOpacity = opacity;
            if (isCritical)
            {
                _isCritical = true;
                _criticalVisibleUntil = DateTime.Now.AddSeconds(2);
                this.Opacity = 1.0;
            }
            else
            {
                _isCritical = false;
            }
        }

        public void SetOverlayPositionAndSize(Rect rect)
        {
            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Width = rect.Width;
            this.Height = rect.Height;
        }

        private void FadeTimer_Tick(object? sender, EventArgs e)
        {
            if (_isCritical && DateTime.Now < _criticalVisibleUntil)
            {
                this.Opacity = 1.0;
                return;
            }
            else if (_isCritical && DateTime.Now >= _criticalVisibleUntil)
            {
                _isCritical = false;
            }
            // Smooth fade
            this.Opacity += (_targetOpacity - this.Opacity) * 0.2;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO: Multi-Monitor Support, Positionierung, Breite/Höhe dynamisch
        }
    }
}
