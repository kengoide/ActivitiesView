using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ActivitiesView
{
    class ThumbnailHost : FrameworkElement
    {
        private HwndSource _destinationHwndSource;
        private IntPtr _hthumbnail;
        private Win32.DWM_THUMBNAIL_PROPERTIES _thumbnailProperties;

        public ThumbnailHost() : base()
        {
            _destinationHwndSource = null;
            _hthumbnail = IntPtr.Zero;
            _thumbnailProperties = new Win32.DWM_THUMBNAIL_PROPERTIES();
            _thumbnailProperties.dwFlags = Win32.DWM_TNP_RECTDESTINATION | Win32.DWM_TNP_VISIBLE;
            _thumbnailProperties.fVisible = true;
        }

        public IntPtr SourceHwnd
        {
            get => (IntPtr)GetValue(SourceHwndProperty);
            set => SetValue(SourceHwndProperty, value);
        }

        public static readonly DependencyProperty SourceHwndProperty =
            DependencyProperty.Register("SourceHwnd", typeof(IntPtr), typeof(ThumbnailHost),
                new PropertyMetadata(IntPtr.Zero, OnSourceHwndChanged));

        private static void OnSourceHwndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ThumbnailHost)?.RegisterThumbnail();
        }

        private void RegisterThumbnail()
        {
            if (_destinationHwndSource == null)
                return;
            if (_hthumbnail != IntPtr.Zero)
            {
                Win32.DwmUnregisterThumbnail(_hthumbnail);
                _hthumbnail = IntPtr.Zero;
            }
            if (SourceHwnd == IntPtr.Zero)
                return;
            _hthumbnail = Win32.DwmRegisterThumbnail(_destinationHwndSource.Handle, SourceHwnd);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_hthumbnail != IntPtr.Zero)
            {
                DpiScale dpi = VisualTreeHelper.GetDpi(this);
                Point screenCoordinate = PointToScreen(new Point(0, 0));
                _thumbnailProperties.rcDestination.left = (int)Math.Round(screenCoordinate.X);
                _thumbnailProperties.rcDestination.top = (int)Math.Round(screenCoordinate.Y);
                _thumbnailProperties.rcDestination.right = (int)Math.Round(screenCoordinate.X + finalSize.Width * dpi.DpiScaleX);
                _thumbnailProperties.rcDestination.bottom = (int)Math.Round(screenCoordinate.Y + finalSize.Height * dpi.DpiScaleY);
                Win32.DwmUpdateThumbnailProperties(_hthumbnail, ref _thumbnailProperties);
            }
            return finalSize;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            // Find the root in Visual tree.
            DependencyObject visual = this;
            DependencyObject parentVisual;
            while ((parentVisual = VisualTreeHelper.GetParent(visual)) != null) {
                visual = parentVisual;
            }
            if (!(visual is Window))
            {
                Debug.WriteLine("Couldn't find a Window in the visual tree. ThumbnailHost must be a descendant of a Window.");
                _destinationHwndSource = null;
                return;
            }

            IntPtr hwnd = new WindowInteropHelper((Window)visual).Handle;
            if (_destinationHwndSource != null && hwnd == _destinationHwndSource.Handle)
            {
                // Root Visual hasn't changed. no-op.
                return;
            }

            if (_destinationHwndSource != null)
            {
                _destinationHwndSource.Dispose();
            }
            _destinationHwndSource = HwndSource.FromHwnd(hwnd);
            RegisterThumbnail();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.LightYellow, null, new Rect(0, 0, ActualWidth, ActualHeight));
        }
    }
}
