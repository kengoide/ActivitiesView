using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActivitiesView
{
    class DesktopWindowTracker
    {
        private ObservableCollection<DesktopWindow> _windows;

        public DesktopWindowTracker()
        {
            _windows = new ObservableCollection<DesktopWindow>();
            GCHandle gch = GCHandle.Alloc(this);
            Win32.EnumDesktopWindows(IntPtr.Zero, EnumProc, GCHandle.ToIntPtr(gch));
            gch.Free();
        }

        public ObservableCollection<DesktopWindow> Windows { get => _windows; }

        private static bool EnumProc(IntPtr hwnd, IntPtr lParam)
        {
            uint style = Win32.GetWindowLongW(hwnd, Win32.GWL_STYLE);
            uint exStyle = Win32.GetWindowLongW(hwnd, Win32.GWL_EXSTYLE);
            bool hasTaskbarButton = true;
            if (Convert.ToBoolean(style & Win32.WS_POPUP) ||
                Convert.ToBoolean(style & Win32.WS_DISABLED) ||
                !Convert.ToBoolean(style & Win32.WS_VISIBLE) ||
                Convert.ToBoolean(exStyle & Win32.WS_EX_TOOLWINDOW))
                hasTaskbarButton = false;
            if (Convert.ToBoolean(exStyle & Win32.WS_EX_APPWINDOW) && Convert.ToBoolean(style & Win32.WS_VISIBLE))
                hasTaskbarButton = true;
            if (!hasTaskbarButton)
                return true;

            DesktopWindowTracker this_ = (DesktopWindowTracker)GCHandle.FromIntPtr(lParam).Target;
            this_._windows.Add(new DesktopWindow(hwnd));
            return true;
        }
    }
}
