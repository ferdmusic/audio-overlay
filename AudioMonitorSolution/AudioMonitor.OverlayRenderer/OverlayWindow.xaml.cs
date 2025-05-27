using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace AudioMonitor.OverlayRenderer
{
    public partial class OverlayWindow : Window
    {
        private DispatcherTimer _fadeTimer;
        private double _targetOpacity = 0.1;
        private DateTime _criticalVisibleUntil = DateTime.MinValue;
        private bool _isCritical = false;

        public OverlayWindow()
        {
            InitializeComponent();
            this.AllowsTransparency = true;
            this.Topmost = true;
            this.IsHitTestVisible = false;
            this.Opacity = 0.1;
            _fadeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            _fadeTimer.Tick += FadeTimer_Tick;
            _fadeTimer.Start();
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
