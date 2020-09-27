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
    class DockItem
    {
        private string _filePath;
        private BitmapSource _bitmapSource;
        
        public DockItem(string executableFilePath)
        {
            _filePath = executableFilePath;
            Win32.IShellItemImageFactory imageFactory =
                (Win32.IShellItemImageFactory)Win32.SHCreateItemFromParsingName(
                    _filePath, IntPtr.Zero, typeof(Win32.IShellItemImageFactory).GUID);
            IntPtr hBitmap = imageFactory.GetImage(new Win32.SIZE(256, 256), Win32.SIIGBF_BIGGERSIZEOK);
            _bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions());
        }

        public string FilePath { get => _filePath; }
        public BitmapSource Image { get => _bitmapSource; }

        public void Launch()
        {
            Process.Start(_filePath);
        }
    }
}
