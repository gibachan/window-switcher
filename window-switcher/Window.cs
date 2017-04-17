using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace WinSwitcher
{
    public class Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        const int SW_RESTORE = 0x09;

        public IntPtr WindowHandle { get; private set; }

        public string WindowTitle
        {
            get
            {
                string windowTitle = "";
                int length = GetWindowTextLength(WindowHandle);
                if (length > 0)
                {
                    var builder = new StringBuilder(length + 1);
                    GetWindowText(WindowHandle, builder, builder.Capacity);
                    windowTitle = builder.ToString();
                }
                return windowTitle;
            }
        }

        public Window(IntPtr hWnd)
        {
            WindowHandle = hWnd;
        }

        public Process GetProcess()
        {
            int processId = 0;
            GetWindowThreadProcessId(WindowHandle, out processId);
            return Process.GetProcessById(processId);
        }

        public bool IsTarget(string keyword)
        {
            var p = GetProcess();
            if (p == null)
            {
                return false;
            }

            var s = keyword.ToUpper();
            if (WindowTitle.ToUpper().Contains(s))
            {
                return true;
            }

            if (p.ProcessName.ToUpper().Contains(s))
            {
                return true;
            }

            return false;
        }

        public void Activate()
        {
            if (IsIconic(WindowHandle))
            {
                ShowWindowAsync(WindowHandle, SW_RESTORE);
            }

            SetForegroundWindow(WindowHandle);
        }


    }
}
