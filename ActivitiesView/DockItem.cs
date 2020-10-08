using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ActivitiesView
{
    class DockItem : DependencyObject
    {
        private readonly string _filePath;
        private readonly string _displayName;
        private readonly string _description;
        private readonly ProcessStartInfo _processStartInfo;

        private static readonly DependencyPropertyKey ImagePropertyKey =
            DependencyProperty.RegisterReadOnly("Image", typeof(BitmapSource), typeof(DockItem), new PropertyMetadata());
        public static readonly DependencyProperty ImageProperty = ImagePropertyKey.DependencyProperty;

        public DockItem(string executableFilePath, string arguments, string workingDirectory,
                        string displayName, string description, int showCommand) : base()
        {
            _filePath = executableFilePath;
            _displayName = displayName;
            _description = description;
            _processStartInfo = new ProcessStartInfo(executableFilePath, arguments);
            _processStartInfo.WorkingDirectory = workingDirectory;

            Task<IntPtr>.Run(() =>
            {
                Win32.IShellItemImageFactory imageFactory =
                    (Win32.IShellItemImageFactory)Win32.SHCreateItemFromParsingName(
                        _filePath, IntPtr.Zero, typeof(Win32.IShellItemImageFactory).GUID);
                while (true)
                {
                    try
                    {
                        return imageFactory.GetImage(new Win32.SIZE(256, 256), Win32.SIIGBF_BIGGERSIZEOK);
                    }
                    catch (COMException e) when (e.HResult == Win32.E_PENDING)
                    {
                        Thread.Sleep(10);
                    }
                }
            }).ContinueWith(task =>
            {
                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    task.Result, IntPtr.Zero, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions());
                SetValue(ImagePropertyKey, bitmapSource);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public string FilePath { get => _filePath; }
        public string DisplayName { get => _displayName; }
        public string Description { get => _description; }
        public BitmapSource Image { get => (BitmapSource)GetValue(ImageProperty); }

        public void Launch()
        {
            Process.Start(_processStartInfo);
        }
    }
}
