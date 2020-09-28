using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActivitiesView
{
    class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx, cy;

            public SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left, top, right, bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public override string ToString()
            {
                return "(" + left.ToString() + "," + top.ToString() + "," + right.ToString() + "," + bottom.ToString() + ")"; 
            }
        }

        public const int E_PENDING = unchecked((int)0x8000000a);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern IntPtr DwmRegisterThumbnail(IntPtr hwndDestination, IntPtr hwndSource);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUnregisterThumbnail(IntPtr hThumbnailId);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmQueryThumbnailSourceSize(IntPtr hThumbnail, out SIZE pSize);

        public const uint DWM_TNP_RECTDESTINATION = 0x00000001;
        public const uint DWM_TNP_VISIBLE = 0x00000008;

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public uint dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUpdateThumbnailProperties(IntPtr hThumbnailId, [In] ref DWM_THUMBNAIL_PROPERTIES ptnProperties);

        public const uint SIIGBF_BIGGERSIZEOK = 0x00000001;

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        public interface IShellItemImageFactory
        {
            IntPtr GetImage(SIZE size, uint flags);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.IUnknown)]
        public static extern object SHCreateItemFromParsingName([In] string pszPath, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);
        
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetWindowTextW(IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpString, int nMaxCount);

        public const uint WS_DISABLED = 0x08000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_APPWINDOW = 0x00040000;

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint GetWindowLongW(IntPtr hWnd, int nIndex);

        public delegate bool WNDENUMPROC(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, WNDENUMPROC lpfn, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
