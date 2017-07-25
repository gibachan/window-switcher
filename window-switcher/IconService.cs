using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;

namespace window_switcher
{
    public class IconService
    {
        private Dictionary<String, Image> cache = new Dictionary<string, Image>();

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

        public Image GetIconImage(Process process)
        {
            var path = ProcessExecutablePath(process);
            if (cache.ContainsKey(path))
                return cache[path];

            var icon = Icon.ExtractAssociatedIcon(path);
            if (icon != null)
                cache[path] = icon.ToBitmap();
            else
                cache[path] = null;

            return cache[path];
        }
    }
}
