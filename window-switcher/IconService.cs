using System;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;

namespace window_switcher
{
    public class IconService
    {
        static private string ProcessExecutablePath(Process process)
        {
            try
            {
                return process.MainModule.FileName;
            }
            catch
            {
                string query = "SELECT ExecutablePath, ProcessID FROM Win32_Process";
                var searcher = new ManagementObjectSearcher(query);

                foreach (var item in searcher.Get())
                {
                    object id = item["ProcessID"];
                    object path = item["ExecutablePath"];

                    if (path != null && id.ToString() == process.Id.ToString())
                    {
                        return path.ToString();
                    }
                }
            }

            return "";
        }

        static public Image GetIconImage(Process process)
        {
            var path = ProcessExecutablePath(process);
            var icon = Icon.ExtractAssociatedIcon(path);
            if (icon != null)
            {
                var image = icon.ToBitmap();
                return image;
            }
            return null;
        }
    }
}
