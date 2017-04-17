using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace WinSwitcher
{
    public class WindowManager
    {
        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        [DllImport("user32")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        static public List<Window> Windows = new List<Window>();

        static public void Refresh()
        {
            Windows.Clear();
            EnumWindows(EnumerateWindow, IntPtr.Zero);
        }

        static private bool EnumerateWindow(IntPtr hWnd, IntPtr lParam)
        {
            if (IsWindowVisible(hWnd) && HasWindowTitle(hWnd))
            {
                Windows.Add(new Window(hWnd));
            }
            return true;
        }

        static private bool HasWindowTitle(IntPtr hWnd)
        {
            return GetWindowTextLength(hWnd) > 0 ? true : false;
        }
    }
}
