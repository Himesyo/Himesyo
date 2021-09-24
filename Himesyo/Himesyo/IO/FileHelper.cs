using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Himesyo.IO
{
    /// <summary>
    /// 和文件相关的辅助方法。
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 在系统资源管理器中显示。
        /// </summary>
        /// <param name="path"></param>
        public static void ShowInExplorer(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (File.Exists(path) || Directory.Exists(path))
            {
                Process.Start("explorer.exe", $"/select,{path}");
                return;
            }
            string dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir))
            {
                Process.Start("explorer.exe", dir);
            }
        }
    }
}
