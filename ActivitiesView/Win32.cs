using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ActivitiesView {
    class Win32 {
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE {
            public int cx, cy;

            public SIZE(int cx, int cy) {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left, top, right, bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public override string ToString() {
                return "(" + left.ToString() + "," + top.ToString() + "," + right.ToString() + "," + bottom.ToString() + ")";
            }
        }

        public const int MAX_PATH = 260;

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
        public struct DWM_THUMBNAIL_PROPERTIES {
            public uint dwFlags;
            public RECT rcDestination;
            public RECT rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmUpdateThumbnailProperties(IntPtr hThumbnailId, [In] ref DWM_THUMBNAIL_PROPERTIES ptnProperties);

        public const uint GENERIC_READ = 0x80000000;
        public const uint OPEN_EXISTING = 3;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr CreateFileW([MarshalAs(UnmanagedType.LPWStr), In] string lpFileName, uint dwDesiredAccess,
            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreateDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
        [StructLayout(LayoutKind.Sequential)]
        public struct FILE_ID_128 {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Identifier;

            public override bool Equals(object obj) {
                if (obj == null || obj.GetType() != typeof(FILE_ID_128))
                    return false;
                FILE_ID_128 other = (FILE_ID_128)obj;
                for (int i = 0; i < 16; ++i) {
                    if (Identifier[i] != other.Identifier[i])
                        return false;
                }
                return true;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct FILE_ID_INFO {
            public ulong VolumeSerialNumber;
            public FILE_ID_128 FileId;

            public override bool Equals(object obj) {
                if (obj == null || obj.GetType() != typeof(FILE_ID_INFO))
                    return false;
                FILE_ID_INFO info = (FILE_ID_INFO)obj;
                return VolumeSerialNumber == info.VolumeSerialNumber && FileId.Equals(info.FileId);
            }
        }
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()

        [DllImport("kernel32.dll")]
        public static extern bool GetFileInformationByHandleEx(IntPtr hFile, uint FileInformationClass, [MarshalAs(UnmanagedType.LPStruct)] out object lpFileInformation, uint dwBufferSize);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint GetModuleFileNameW(IntPtr hModule, [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpFilename, int nSize);

        [ComImport]
        [Guid("00021401-0000-0000-c000-000000000046")]
        public class ShellLink { }

        public const uint SLR_NO_UI = 0x0000;
        public const uint SLGP_SHORTPATH = 0x1;

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214f9-0000-0000-c000-000000000046")]
        public interface IShellLinkW {
            void GetPath([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszFile, int cch, IntPtr pfd, uint fFlags);
            IntPtr GetIDList();
            void SetIDList(IntPtr pidl);
            void GetDescription([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszName, int cch);
            void SetDescription([In] string pszName);
            void GetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszDir, int cch);
            void SetWorkingDirectory([In] string pszDir);
            void GetArguments([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszArgs, int cch);
            void SetArguments([In] string pszArgs);
            ushort GetHotkey();
            void SetHotkey(ushort wHotkey);
            int GetShowCmd();
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder pszIconPath, int cch, out int piIcon);
            void SetIconLocation([In] string pszIconPath, int iIcon);
            void SetRelativePath([In] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hwnd, uint fFlags);
            void SetPath([In] string pszFile);
        }

        public const uint STGM_READ = 0x00000000;

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        public interface IPersistFile /* : IPersist */ {
            // IPersist
            Guid GetClassID();
            // IPersistFile
            void IsDirty();
            void Load([In] string pszFileName, uint dwMode);
            void Save([In] string pszFileName, bool fRemember);
            void SaveCompleted([In] string pszFileName);
            string GetCurFile();
        }

        public const uint SIIGBF_BIGGERSIZEOK = 0x00000001;

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        public interface IShellItemImageFactory {
            IntPtr GetImage(SIZE size, uint flags);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.IUnknown)]
        public static extern object SHCreateItemFromParsingName([In] string pszPath, IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetClassNameW(IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int GetWindowTextW(IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpString, int nMaxCount);

        public const uint WS_DISABLED = 0x08000000;
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_POPUP = 0x80000000;
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        public const uint WS_EX_APPWINDOW = 0x00040000;

        public const int GWLP_HINSTANCE = -6;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern uint GetWindowLongW(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern IntPtr GetWindowLongPtrW(IntPtr hWnd, int nIndex);

        public const int WM_CLOSE = 0x0010;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern bool PostMessageW(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public delegate bool WNDENUMPROC(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, WNDENUMPROC lpfn, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool EnumChildWindows(IntPtr hWndParent, WNDENUMPROC lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
