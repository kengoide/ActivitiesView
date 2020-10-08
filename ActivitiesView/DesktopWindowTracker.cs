using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActivitiesView {
    class DesktopWindowTracker {
        private readonly List<DesktopWindow> _windows;

        public DesktopWindowTracker() {
            _windows = new List<DesktopWindow>();
            GCHandle gch = GCHandle.Alloc(this);
            Win32.EnumDesktopWindows(IntPtr.Zero, EnumDesktopProc, GCHandle.ToIntPtr(gch));
            gch.Free();
        }

        public List<DesktopWindow> Windows { get => _windows; }

        private static readonly StringBuilder buffer = new StringBuilder(64);

        private static bool EnumDesktopProc(IntPtr hwnd, IntPtr lParam) {
            Win32.GetClassNameW(hwnd, buffer, buffer.Capacity);
            if (buffer.ToString() == "Windows.UI.Core.CoreWindow")
                return true;
            if (buffer.ToString() == "ApplicationFrameWindow") {
                // UWP application.
                bool isPhantom = true;
                Win32.EnumChildWindows(hwnd, (hwndChild, _) => {
                    Win32.GetClassNameW(hwndChild, buffer, buffer.Capacity);
                    if (buffer.ToString() == "Windows.UI.Core.CoreWindow")
                        isPhantom = false;
                    return true;
                }, IntPtr.Zero);
                if (isPhantom)
                    return true;
            }

            uint style = Win32.GetWindowLongW(hwnd, Win32.GWL_STYLE);
            uint exStyle = Win32.GetWindowLongW(hwnd, Win32.GWL_EXSTYLE);
            bool hasTaskbarButton = true;
            if (Convert.ToBoolean(style & Win32.WS_DISABLED) ||
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
