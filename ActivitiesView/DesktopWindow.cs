using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiesView {
    class DesktopWindow {
        private readonly IntPtr _hwnd;
        private readonly string _windowText;
        private static readonly StringBuilder stringBuffer = new StringBuilder(256);

        public DesktopWindow(IntPtr hwnd) {
            _hwnd = hwnd;
            Win32.GetWindowTextW(_hwnd, stringBuffer, stringBuffer.Capacity);
            _windowText = stringBuffer.ToString();
        }

        public IntPtr Hwnd { get => _hwnd; }
        public string WindowText { get => _windowText; }

        public void BringToForeground() {
            Win32.SetForegroundWindow(_hwnd);
        }

        public void Close() {
            Win32.PostMessageW(_hwnd, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
