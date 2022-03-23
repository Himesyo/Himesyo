using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Himesyo.IO
{
    /// <summary>
    /// 提供枚举文件的方法。
    /// </summary>
    public class Files
    {
        /// <summary>
        /// 发生错误后调用的委托。
        /// </summary>
        public event EnumerateErrorEventHandler ErrorHandler;

        /// <summary>
        /// 要枚举文件的路径，支持通配符。
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// 要与 <see cref="RootPath"/> 中的文件名称匹配的搜索字符串。
        /// </summary>
        public string SearchPattern { get; set; }
        /// <summary>
        /// 指定是搜索当前目录，还是搜索当前目录及其所有子目录。
        /// </summary>
        public SearchOption SearchOption { get; set; }

        /// <summary>
        /// 使用默认参数创建 <see cref="Files"/> 类型的新实例。
        /// </summary>
        public Files()
        {
            SearchPattern = "*";
        }

        /// <summary>
        /// 使用指定路径创建 <see cref="Files"/> 类型的新实例。
        /// </summary>
        public Files(string path) : this()
        {
            RootPath = path;
        }

        /// <summary>
        /// 使用指定参数创建 <see cref="Files"/> 类型的新实例。
        /// </summary>
        public Files(string path, string searchPattern, SearchOption searchOption = default, EnumerateErrorEventHandler errorHandler = default)
        {
            RootPath = path;
            SearchPattern = searchPattern;
            SearchOption = searchOption;
            ErrorHandler = errorHandler;
        }

        /// <summary>
        /// 开始枚举文件
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> EnumerateFiles()
        {
            if (File.Exists(RootPath))
            {
                yield return RootPath;
            }
            else if (Directory.Exists(RootPath))
            {
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(RootPath, SearchPattern, SearchOption);
                }
                catch (Exception ex)
                {
                    ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(RootPath, ex));
                }
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        yield return file;
                    }
                }
            }
            else
            {
                string fullpath = PathConvert.GetFullPath(RootPath);
                List<string> roots = new List<string>();
                bool isAllFile = false;
                string[] dirs = fullpath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (dirs[0][0] == '*' || dirs[0][0] == '?')
                {
                    try
                    {
                        roots.AddRange(Environment.GetLogicalDrives());
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs("逻辑驱动器", ex));
                        yield break;
                    }
                }
                else if (Directory.Exists($"{dirs[0]}\\"))
                {
                    roots.Add($"{dirs[0]}\\");
                }
                for (int i = 1; i < dirs.Length; i++)
                {
                    string dir = dirs[i];
                    if (dir == "**")
                    {
                        isAllFile = true;
                        break;
                    }
                    List<string> newRoots = new List<string>();
                    foreach (var root in roots)
                    {
                        try
                        {
                            foreach (var newRoot in Directory.EnumerateDirectories(root, dir))
                            {
                                newRoots.Add(newRoot);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                    }
                    roots = newRoots;
                }
                if (isAllFile)
                {
                    string rootPath = SearchOption == SearchOption.AllDirectories ? Path.Combine(fullpath, "**") : fullpath;
                    string strRegexPath = Regex.Escape(Path.Combine(rootPath.Replace('/', '\\'), SearchPattern));
                    strRegexPath = strRegexPath.Replace(@"\\\*\*\\", @"\\.*\\").Replace(@"\*", @"[^/\\]*").Replace(@"\?", @"[^/\\]?");
                    Regex regexPath = new Regex($"^{strRegexPath}$", RegexOptions.IgnoreCase);
                    foreach (var root in roots)
                    {
                        string[] files = null;
                        try
                        {
                            files = Directory.GetFiles(root, SearchPattern, SearchOption.AllDirectories);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                if (regexPath.IsMatch(file))
                                {
                                    yield return file;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var root in roots)
                    {
                        string[] files = null;
                        try
                        {
                            files = Directory.GetFiles(root, SearchPattern, SearchOption);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                        if (files != null)
                        {
                            foreach (var file in files)
                            {
                                yield return file;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 开始枚举目录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> EnumerateDirectories()
        {
            if (File.Exists(RootPath))
            {
                yield break;
            }
            else if (Directory.Exists(RootPath))
            {
                string[] directories = null;
                try
                {
                    directories = Directory.GetDirectories(RootPath, SearchPattern, SearchOption);
                }
                catch (Exception ex)
                {
                    ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(RootPath, ex));
                }
                if (directories != null)
                {
                    foreach (var directory in directories)
                    {
                        yield return directory;
                    }
                }
            }
            else
            {
                string fullpath = PathConvert.GetFullPath(RootPath);
                List<string> roots = new List<string>();
                bool isAllFile = false;
                string[] dirs = fullpath.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (dirs[0][0] == '*' || dirs[0][0] == '?')
                {
                    try
                    {
                        roots.AddRange(Environment.GetLogicalDrives());
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs("逻辑驱动器", ex));
                        yield break;
                    }
                }
                else if (Directory.Exists($"{dirs[0]}\\"))
                {
                    roots.Add($"{dirs[0]}\\");
                }
                for (int i = 1; i < dirs.Length; i++)
                {
                    string dir = dirs[i];
                    if (dir == "**")
                    {
                        isAllFile = true;
                        break;
                    }
                    List<string> newRoots = new List<string>();
                    foreach (var root in roots)
                    {
                        try
                        {
                            foreach (var newRoot in Directory.EnumerateDirectories(root, dir))
                            {
                                newRoots.Add(newRoot);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                    }
                    roots = newRoots;
                }
                if (isAllFile)
                {
                    string rootPath = SearchOption == SearchOption.AllDirectories ? Path.Combine(fullpath, "**") : fullpath;
                    string strRegexPath = Regex.Escape(Path.Combine(rootPath.Replace('/', '\\'), SearchPattern));
                    strRegexPath = strRegexPath.Replace(@"\\\*\*\\", @"\\.*\\").Replace(@"\*", @"[^/\\]*").Replace(@"\?", @"[^/\\]?");
                    Regex regexPath = new Regex($"^{strRegexPath}$", RegexOptions.IgnoreCase);
                    foreach (var root in roots)
                    {
                        string[] directories = null;
                        try
                        {
                            directories = Directory.GetDirectories(root, SearchPattern, SearchOption.AllDirectories);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                        if (directories != null)
                        {
                            foreach (var directory in directories)
                            {
                                if (regexPath.IsMatch(directory))
                                {
                                    yield return directory;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (var root in roots)
                    {
                        string[] directories = null;
                        try
                        {
                            directories = Directory.GetDirectories(root, SearchPattern, SearchOption);
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler?.Invoke(this, new EnumerateErrorEventArgs(root, ex));
                        }
                        if (directories != null)
                        {
                            foreach (var directory in directories)
                            {
                                yield return directory;
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 为 <see cref="Files.ErrorHandler"/> 事件提供委托类型。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EnumerateErrorEventHandler(object sender, EnumerateErrorEventArgs e);

    /// <summary>
    /// 为 <see cref="Files.ErrorHandler"/> 事件委托类型 <see cref="EnumerateErrorEventHandler"/> 提供事件数据。
    /// </summary>
    public class EnumerateErrorEventArgs : EventArgs
    {
        /// <summary>
        /// 引发异常的路径。
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// 异常对象。
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// 使用指定路径信息和异常对象初始化 <see cref="EnumerateErrorEventArgs"/> 类型的新实例。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exception"></param>
        public EnumerateErrorEventArgs(string path, Exception exception)
        {
            Path = path;
            Exception = exception;
        }
    }
}
