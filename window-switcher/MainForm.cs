using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinSwitcher
{
    public partial class MainForm : Form
    {
        // Hotkey
        const int MOD_CONTROL = 0x0002;
        const int MOD_SHIFT = 0x0004;
        const int WM_HOTKEY = 0x0312;
        const int HOTKEY_ID = 0x0001; 

        [DllImport("user32.dll")]
        extern static int RegisterHotKey(IntPtr HWnd, int ID, int MOD_KEY, int KEY);
        [DllImport("user32.dll")]
        extern static int UnregisterHotKey(IntPtr HWnd, int ID);

        private List<Window> filteredWindows = new List<Window>();

        public MainForm()
        {
            InitializeComponent();

            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (RegisterHotKey(this.Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, (int)Keys.OemQuestion) == 0)
            {
                Console.WriteLine("Error to register hotkey.");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }


        private void MainForm_Shown(object sender, EventArgs e)
        {
            TopMost = true;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (windowListBox.Focused == false && windowListBox.Items.Count > 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (windowListBox.SelectedIndex == -1 || windowListBox.SelectedIndex == windowListBox.Items.Count - 1)
                        {
                            windowListBox.SelectedIndex = 0;
                        } 
                        else
                        {
                            windowListBox.SelectedIndex++;
                        }
                        break;
                    case Keys.Up:
                        if (windowListBox.SelectedIndex == -1 || windowListBox.SelectedIndex == 0)
                        {
                            windowListBox.SelectedIndex = windowListBox.Items.Count - 1;
                        } 
                        else
                        {
                            windowListBox.SelectedIndex--;
                        }
                        break;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Return:
                    ActivateWindow();
                    HideForm();
                    break;
                case Keys.Escape:
                    HideForm();
                    break;
            }
        }

        private void windowListBox_TextChanged(object sender, EventArgs e)
        {
            UpdateWindowList();
        }

        private void windowListBox_DoubleClick(object sender, EventArgs e)
        {
            ActivateWindow();
            HideForm();
        }

        private void windowListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (windowListBox.Items.Count > 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        if (windowListBox.SelectedIndex == windowListBox.Items.Count - 1)
                        {
                            e.SuppressKeyPress = true;
                            windowListBox.SelectedIndex = 0;
                        }
                        break;
                    case Keys.Up:
                        if (windowListBox.SelectedIndex == 0)
                        {
                            e.SuppressKeyPress = true;
                            windowListBox.SelectedIndex = windowListBox.Items.Count - 1;
                        }
                        break;
                }
            }
        }

        private void UpdateWindowList()
        {
            WindowManager.Refresh();

            var currentWindow = GetCurrentWindow();
            var keyword = keywordTextBox.Text.Trim();

            filteredWindows.Clear();
            if (keyword.Length == 0)
            {
                foreach (var w in WindowManager.Windows)
                {
                    filteredWindows.Add(w);
                }
            }
            else
            {
                foreach (var w in WindowManager.Windows)
                {
                    if (w.IsTarget(keyword))
                    {
                        filteredWindows.Add(w);
                    }
                }
            }

            windowListBox.BeginUpdate();

            windowListBox.Items.Clear();
            foreach (var w in filteredWindows)
            {
                windowListBox.Items.Add(DisplayText(w));
            }
            if (windowListBox.Items.Count > 0 && windowListBox.SelectedIndex == -1)
            {
                windowListBox.SelectedIndex = 0;
            }

            windowListBox.EndUpdate();
        }

        private string DisplayText(Window w)
        {
            const int MAX_TITLE_LENGTH = 30;

            var text = w.WindowTitle;
            if (text.Length > MAX_TITLE_LENGTH)
            {
                text = text.Substring(0, MAX_TITLE_LENGTH);
            }

            var p = w.GetProcess();
            if (p != null)
            {
                if (text.Length == 0)
                {
                    text = p.ProcessName;
                }
                else
                {
                    text += "/" + p.ProcessName;
                }
            }

            return text;
        }

        private Window GetCurrentWindow()
        {
            int index = windowListBox.SelectedIndex;

            if (index >= 0 && index < filteredWindows.Count)
            {
                return filteredWindows[index];
            }

            return null;
        }

        private void ActivateWindow()
        {
            var w = GetCurrentWindow();
            if (w != null)
            {
                w.Activate();
            }
        }

        private void ShowForm()
        {
            UpdateWindowList();

            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void HideForm()
        {
            Hide();
            keywordTextBox.Text = "";
            windowListBox.SelectedIndex = -1;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if ((int)m.WParam == HOTKEY_ID)
                {
                    ShowForm();
                }
            }
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            HideForm();
        }
    }

}
