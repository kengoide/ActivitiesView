using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivitiesView {
    class DesktopWindow {
        private readonly IntPtr _hwnd;
        private readonly string _windowText;
        private readonly Win32.FILE_ID_INFO _moduleFileId;
        private static readonly StringBuilder stringBuffer = new StringBuilder(Win32.MAX_PATH);

        public DesktopWindow(IntPtr hwnd) {
            _hwnd = hwnd;
            Win32.GetWindowTextW(_hwnd, stringBuffer, stringBuffer.Capacity);
            _windowText = stringBuffer.ToString();

            IntPtr hInstance = Win32.GetWindowLongPtrW(_hwnd, Win32.GWLP_HINSTANCE);
            Win32.GetModuleFileNameW(hInstance, stringBuffer, stringBuffer.Capacity);
            IntPtr hFile = Win32.CreateFileW(stringBuffer.ToString(), Win32.GENERIC_READ, 0, IntPtr.Zero, Win32.OPEN_EXISTING, 0, IntPtr.Zero);
            Win32.GetFileInformationByHandleEx(hFile, )
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
