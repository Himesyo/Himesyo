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

        /// <summary>
        /// 打开文件或目录。
        /// </summary>
        /// <param name="path"></param>
        public static void Open(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            if (File.Exists(path) || Directory.Exists(path))
            {
                Process.Start(path);
                return;
            }
        }

        #region 目录结构

        /// <summary>
        /// 获取指定目录下所有文件夹和文件的目录结构。
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static string GetDirectoryStructure(string root)
        {
            StringBuilder result = new StringBuilder(256);
            List<bool> levelState = new List<bool>();
            APath(root, 0, levelState, result);
            return result.ToString();
        }
        private static void APath(string root, int currentLevel, List<bool> levelState, StringBuilder result)
        {
            string rootName = Path.GetFileName(root);
            result.AppendLine(rootName);
            string[] dirs = Directory.GetDirectories(root);
            string[] files = Directory.GetFiles(root);
            string prefix = GetPrefix(currentLevel, levelState);
            if (files.Length == 0)
            {
                for (int i = 0; i < dirs.Length; i++)
                {
                    bool continuation = i < dirs.Length - 1;
                    SetPrefixState(currentLevel, continuation, levelState);
                    string c = continuation ? "┣" : "┗";
                    result.Append(prefix).Append(c);
                    APath(dirs[i], currentLevel + 1, levelState, result);
                }
            }
            else
            {
                string c = "┣";
                for (int i = 0; i < dirs.Length; i++)
                {
                    SetPrefixState(currentLevel, true, levelState);
                    result.Append(prefix).Append(c);
                    APath(dirs[i], currentLevel + 1, levelState, result);
                }
                for (int i = 0; i < files.Length; i++)
                {
                    bool continuation = i < files.Length - 1;
                    c = continuation ? "┣" : "┗";
                    result.Append(prefix).Append(c);
                    string name = Path.GetFileName(files[i]);
                    result.AppendLine(name);
                }
            }
        }
        private static string GetPrefix(int currentLevel, List<bool> levelState)
        {
            if (currentLevel == 0)
                return string.Empty;

            for (int i = levelState.Count; i < currentLevel; i++)
            {
                levelState.Add(false);
            }
            string[] prefix = new string[currentLevel];
            for (int i = 0; i < prefix.Length; i++)
            {
                prefix[i] = levelState[i] ? "┃" : "  ";
            }
            return string.Join(string.Empty, prefix);
        }
        private static void SetPrefixState(int level, bool value, List<bool> levelState)
        {
            for (int i = levelState.Count; i <= level; i++)
            {
                levelState.Add(false);
            }
            levelState[level] = value;
        }

        #endregion
    }
}
