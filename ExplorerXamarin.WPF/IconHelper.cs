using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ExplorerXamarin.WPF
{
    public static class IconHelper
    {
        public static Icon CreateIcon(FileSystemInfo info)
        {
            uint attrs = 0;
            if ((info.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                attrs |= Win32.FILE_ATTRIBUTE_DIRECTORY;
            }
            else
            {
                attrs |= Win32.FILE_ATTRIBUTE_NORMAL;
            }
            var flags = Win32.SHGFI_ICON | Win32.SHGFI_USEFILEATTRIBUTES | Win32.SHGFI_SHELLICONSIZE;

            var shInfo = new SHFILEINFO();
            Win32.SHGetFileInfo(
                info.FullName,
                attrs,
                ref shInfo, (uint)Marshal.SizeOf(shInfo),
                flags);

            return Icon.FromHandle(shInfo.hIcon);
        }

        public static BitmapSource CreateBitmapSourceFromIcon(Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                new Int32Rect(0, 0, icon.Width, icon.Height),
                BitmapSizeOptions.FromEmptyOptions());
        }

        [StructLayout(LayoutKind.Sequential)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        class Win32
        {
            public const uint SHGFI_ICON = 0x100;
            public const uint SHGFI_SHELLICONSIZE = 0x4;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

            public const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
            public const uint FILE_ATTRIBUTE_NORMAL = 0x80;

            [DllImport("shell32.dll")]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }
    }
}
