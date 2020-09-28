using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ActivitiesView
{
    class DockItem : DependencyObject
    {
        private string _filePath;

        private static readonly DependencyPropertyKey ImagePropertyKey =
            DependencyProperty.RegisterReadOnly("Image", typeof(BitmapSource), typeof(DockItem), new PropertyMetadata());
        public static readonly DependencyProperty ImageProperty = ImagePropertyKey.DependencyProperty;

        public DockItem(string executableFilePath) : base()
        {
            _filePath = executableFilePath;
            Win32.IShellItemImageFactory imageFactory =
                (Win32.IShellItemImageFactory)Win32.SHCreateItemFromParsingName(
                    _filePath, IntPtr.Zero, typeof(Win32.IShellItemImageFactory).GUID);
            IntPtr hBitmap = imageFactory.GetImage(new Win32.SIZE(256, 256), Win32.SIIGBF_BIGGERSIZEOK);
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions());
            SetValue(ImagePropertyKey, bitmapSource);
        }

        public string FilePath { get => _filePath; }
        public BitmapSource Image { get => (BitmapSource)GetValue(ImageProperty); }

        public void Launch()
        {
            Process.Start(_filePath);
        }
    }
}
