using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyShell
{
    public static class Win32
    {
        #region Variables
        public const int SHGFI_ICON = 0x100;
        public const int SHGFI_SMALLICON = 0x1;
        public const int SHGFI_LARGEICON = 0x0;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_SETREDRAW = 0xB;
        public const int FILE_ATTRIBUTE_NORMAL = 0x80;
        public static string[] imageType = new string[] { "jpeg", "jpg", "png", "bmp" };
        #endregion

        public struct Rect
        {
            public int Left, Top, Right, Bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out Rect lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("kernel32.dll")]
        public static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName, [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        public static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
           out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);

        public enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }

        public static class HWND
        {
            public static IntPtr
            NoTopMost = new IntPtr(-2),
            TopMost = new IntPtr(-1),
            Top = new IntPtr(0),
            Bottom = new IntPtr(1);
        }

        public static class SWP
        {
            public static readonly uint
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }


        public enum WindowShowStyle : uint
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            Maximize = 3,
            ShowNormalNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActivate = 7,
            ShowNoActivate = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimized = 11
        }

        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rectangle rcNormalPosition;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public enum SHGetFileInfoConstants : int
        {
            SHGFI_ICON = 0x100,                // get icon
            SHGFI_DISPLAYNAME = 0x200,         // get display name
            SHGFI_TYPENAME = 0x400,            // get type name
            SHGFI_ATTRIBUTES = 0x800,          // get attributes
            SHGFI_ICONLOCATION = 0x1000,       // get icon location
            SHGFI_EXETYPE = 0x2000,            // return exe type
            SHGFI_SYSICONINDEX = 0x4000,       // get system icon index
            SHGFI_LINKOVERLAY = 0x8000,        // put a link overlay on icon
            SHGFI_SELECTED = 0x10000,          // show icon in selected state
            SHGFI_ATTR_SPECIFIED = 0x20000,    // get only specified attributes
            SHGFI_JUMBO = 0x4,                 // get jumbo icon
            SHGFI_EXTRALARGE = 0x2,            // get extra large icon
            SHGFI_LARGEICON = 0x0,             // get large icon
            SHGFI_SMALLICON = 0x1,             // get small icon
            SHGFI_OPENICON = 0x2,              // get open icon
            SHGFI_SHELLICONSIZE = 0x4,         // get shell size icon
            SHGFI_PIDL = 0x8,                  // pszPath is a pidl
            SHGFI_USEFILEATTRIBUTES = 0x10,    // use passed dwFileAttribute
            SHGFI_ADDOVERLAYS = 0x000000020,   // apply the appropriate overlays
            SHGFI_OVERLAYINDEX = 0x000000040   // Get the index of the overlay
        }

        public static Icon GetLargeIconForExtension(string extension)
        {
            // Get the small icon and clone it, as we MUST destroy the handle when we are done.
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr ptr = SHGetFileInfo(
                extension,
                FILE_ATTRIBUTE_NORMAL,
                ref shinfo, (uint)Marshal.SizeOf(shinfo),
                (int)(
                SHGetFileInfoConstants.SHGFI_ICON |
                SHGetFileInfoConstants.SHGFI_LARGEICON |
                SHGetFileInfoConstants.SHGFI_USEFILEATTRIBUTES |
                SHGetFileInfoConstants.SHGFI_TYPENAME
                ));
            Icon icon = (Icon)Icon.FromHandle(shinfo.hIcon).Clone();
            DestroyIcon(shinfo.hIcon);
            return icon;
        }

        [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int NetServerEnum(
            string ServerNane, // must be null
            int dwLevel,
            ref IntPtr pBuf,
            int dwPrefMaxLen,
            out int dwEntriesRead,
            out int dwTotalEntries,
            int dwServerType,
            string domain, // null for login domain
            out int dwResumeHandle
            );

        [DllImport("Netapi32", SetLastError = true), SuppressUnmanagedCodeSecurity]
        public static extern int NetApiBufferFree(IntPtr pBuf);

        [StructLayout(LayoutKind.Sequential)]
        public struct _SERVER_INFO_100
        {
            internal int sv100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string sv100_name;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfoA")]
        public static extern int SystemParametersInfo(int uiAction, int uiParam, ref Rect pvParam, int fWinIni);

        public static Bitmap GetBigImage(string path, Size size)
        {
            Bitmap bmp = null;
            string ext = Path.GetExtension(path);

            if (Directory.Exists(path))
            {
                bool isLocalDrive = false;
                bool isRemoveableDrive = false;
                bool isHDD = false;

                foreach (DriveInfo drive in DriveInfo.GetDrives().Where(d=>d.IsReady))
                {
                    if (drive.Name == path){
                        if (path == Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)))
                        {
                            isLocalDrive = true;
                        }
                        else if (drive.DriveType == DriveType.Removable)
                        {
                            isRemoveableDrive = true;
                        }
                        else
                        {
                            isHDD = true;
                        }
                    }

                    if (isLocalDrive || isRemoveableDrive || isHDD)
                        break;
                }

                /*if (isLocalDrive)
                    bmp = new Bitmap(MDCDesktop.Properties.Resources.LHDD, size);
                else if (isRemoveableDrive)
                    bmp = new Bitmap(MDCDesktop.Properties.Resources.RHDD, size);
                else if (isHDD)
                    bmp = new Bitmap(MDCDesktop.Properties.Resources.HDD, size);
                else
                {*/
                    SHFILEINFO shf = new SHFILEINFO();
                    SHGetFileInfo(path, 0x00000010, ref shf, (uint)Marshal.SizeOf(shf), (uint)SHGetFileInfoConstants.SHGFI_ICON | (uint)SHGetFileInfoConstants.SHGFI_LARGEICON);
                    Icon ico = (Icon)Icon.FromHandle(shf.hIcon).Clone();
                    DestroyIcon(shf.hIcon);
                    bmp = ico.ToBitmap();
                    ico.Dispose();
                //}
            }
            else if (ext == ".url")
            {
                List<string> lines = new List<string>();

                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        lines.Add(line);
                }

                foreach (string line in lines)
                {
                    if (line.Contains("IconFile="))
                    {
                        Icon ico = Icon.ExtractAssociatedIcon(line.Substring(9, line.Length - 9));
                        bmp = ico.ToBitmap();
                        ico.Dispose();
                        break;
                    }
                }
            }
            else if (ext == ".exe")
            {
                Icon ico = Icon.ExtractAssociatedIcon(path);
                bmp = ico.ToBitmap();
                ico.Dispose();
            }
            else
            {
                if (imageType.Contains(ext.Replace(".", "")))
                {
                    Image img = Image.FromFile(path);
                    bmp = new Bitmap(img, new Size(size.Width - 20, 80));
                    img.Dispose();
                }
                else
                {
                    Icon ico = GetLargeIconForExtension(ext);
                    bmp = ico.ToBitmap();
                    ico.Dispose();
                }
            }

            return bmp == null ? new Bitmap(size.Width, size.Height) : bmp;
        }

        public static Bitmap GetSmallImage(string path, Size size, int fp)
        {
            Bitmap bmp;
            string ext = System.IO.Path.GetExtension(path);

            if (ext == ".exe")
            {
                Icon ico = Icon.ExtractAssociatedIcon(path);
                bmp = ico.ToBitmap();
                ico.Dispose();
            }
            else
            {
                if (imageType.Contains(ext.Replace(".", "")))
                {
                    Image img = Image.FromFile(path);
                    bmp = new Bitmap(img, new Size(size.Width - 20, 80));
                    img.Dispose();
                }
                else
                {
                    Icon ico = GetLargeIconForExtension(ext);
                    bmp = ico.ToBitmap();
                    ico.Dispose();
                }
            }

            return new Bitmap(bmp, (imageType.Contains(ext.Replace(".", "")) ? new Size((fp / 5), 80) : bmp.Size));
        }

        public static bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }
    }
}
