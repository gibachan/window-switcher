using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace window_switcher
{
    class HotKeyRegister
    {
        // Hotkey
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int WM_HOTKEY = 0x0312;
        const int HOTKEY_ID = 0x0001;

        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr handle, int ID, int MOD_KEY, int KEY);
        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr handle, int ID);

        static public bool IsHotkeyMessage(int msg)
        {
            return msg == WM_HOTKEY;
        }

        static public bool IsHotkeyId(int id)
        {
            return id == HOTKEY_ID;
        }

        static public bool Register(IntPtr handle, int key)
        {
            return RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, key) != 0;
        }

        static public void Unregister(IntPtr handle)
        {
            UnregisterHotKey(handle, HOTKEY_ID);
        }


    }
}
