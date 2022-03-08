using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Himesyo.Linq;

namespace Himesyo.IO
{
    /// <summary>
    /// 提供对路径字符串相关的一些操作。是对 <see cref="Path"/> 的一个扩展。
    /// </summary>
    public static class PathConvert
    {
        /// <summary>
        /// 通过替换文件名中不可出现的字符来创建安全文件名称。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetSafeFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "_";

            string str = new string(Path.GetInvalidFileNameChars());
            string safeName = Regex.Replace(name, $"[{Regex.Escape(str)}]+", "_");
            return safeName;
        }

        /// <summary>
        /// 返回指定路径字符串的绝对路径。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="path"></param>
        /// <returns>指定路径字符串的绝对路径</returns>
        public static string GetFullPath(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            return GetFullPathNotValidated(path, Environment.CurrentDirectory);
        }

        /// <summary>
        /// 获取指定路径字符串相对于另一路径的绝对路径。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="sourcePath">另一绝对路径</param>
        /// <returns>指定路径字符串相对于另一路径的绝对路径</returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> 或 <paramref name="sourcePath"/> 为 <see langword="null"/> .</exception>
        public static string GetRelativePathFullPath(string path, string sourcePath)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (sourcePath == null)
                throw new ArgumentNullException(nameof(sourcePath));

            return GetFullPathNotValidated(path, sourcePath);
        }

        /// <summary>
        /// 返回指定路径字符串相对于另一目录路径的相对路径。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path">指定路径</param>
        /// <param name="sourcePath">另一目录路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> 或 <paramref name="sourcePath"/> 为 <see langword="null"/> .</exception>
        /// <exception cref="ArgumentException">两个路径不在同一盘符下。</exception>
        public static string GetRelativePath(string path, string sourcePath)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (sourcePath == null)
                throw new ArgumentNullException(nameof(sourcePath));

            path = GetFullPathNotValidated(path, Environment.CurrentDirectory);
            sourcePath = GetFullPathNotValidated(sourcePath, Environment.CurrentDirectory);

            if (char.ToUpperInvariant(path[0]) != char.ToUpperInvariant(sourcePath[0]))
                throw new ArgumentException("指定路径和目录路径必须在同一个盘符下。");

            return GetRelativePathNotValidated(path, sourcePath);
        }

        /// <summary>
        /// 返回指定路径字符串的目录信息。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string path)
        {
            if (path == null)
                return null;

            path = path.Replace('/', '\\');
            int index = path.LastIndexOf('\\');
            if (index == -1)
            {
                return string.Empty;
            }
            else
            {
                return path.Substring(0, index);
            }
        }

        /// <summary>
        /// 返回指定路径字符串的文件名和扩展名。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns></returns>
        public static string GetFileName(string path)
        {
            if (path == null)
                return null;

            path = path.Replace('/', '\\');
            int index = path.LastIndexOf('\\');
            if (index == -1)
            {
                return path;
            }
            else
            {
                return path.Substring(index + 1);
            }
        }

        /// <summary>
        /// 返回不具有扩展名的指定路径字符串的文件名。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            string fileName = GetFileName(path);
            int index = fileName.LastIndexOf('.');
            if (index == -1)
            {
                return fileName;
            }
            else
            {
                return fileName.Substring(0, index);
            }
        }

        /// <summary>
        /// 返回指定的路径字符串的扩展名。
        /// <para>注意：该方法是对字符串的处理，未对输入路径进行验证。</para>
        /// </summary>
        /// <param name="path">路径字符串</param>
        /// <returns></returns>
        public static string GetExtension(string path)
        {
            path = GetFileName(path);
            int index = path.LastIndexOf('.');
            if (index == -1 || index == path.Length - 1)
            {
                return "";
            }
            else
            {
                return path.Substring(index + 1);
            }
        }

        /// <summary>
        /// 获取绝对路径的实现。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private static string GetFullPathNotValidated(string path, string sourcePath)
        {
            if (Regex.IsMatch(path, @"^[a-z0-9_\*\?]*:", RegexOptions.IgnoreCase))
                return path;

            if (sourcePath.Length <= 1 || sourcePath[1] != ':')
                sourcePath = GetFullPath(path);

            var sourcePathArr = sourcePath.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var pathArr = path.Split(new[] { '/', '\\' }, StringSplitOptions.None);
            Stack<string> result = new Stack<string>(Math.Min(sourcePathArr.Length + pathArr.Length, 128));
            sourcePathArr.ForEach(d => result.Push(d));
            bool isTop = true;
            foreach (var item in pathArr.Select(d => d.Trim()))
            {
                if (isTop)
                {
                    isTop = false;
                    if (string.IsNullOrEmpty(item))
                    {
                        result.Clear();
                        result.Push(sourcePathArr[0]);
                        continue;
                    }
                }

                if (string.IsNullOrEmpty(item) || item == ".")
                    continue;

                if (item == "..")
                {
                    if (result.Count > 1)
                    {
                        result.Pop();
                    }
                }
                else
                {
                    result.Push(item);
                }
            }

            if (result.Count == 1)
            {
                return $"{result.Pop()}\\";
            }
            else
            {
                return string.Join("\\", result.Reverse());
            }
        }

        /// <summary>
        /// 获取相对路径的实现。两个路径都是绝对路径。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        private static string GetRelativePathNotValidated(string path, string sourcePath)
        {
            //path = GetFullPathNotValidated(path, Environment.CurrentDirectory);
            //sourcePath = GetFullPathNotValidated(path, Environment.CurrentDirectory);

            char[] splitChar = new[] { '/', '\\' };
            string[] pathArr = path.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            string[] sourcePathArr = sourcePath.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

            Stack<string> result = new Stack<string>();
            int length = Math.Min(pathArr.Length, sourcePathArr.Length);
            for (int i = 0; i < length; i++)
            {
                if (!string.Equals(pathArr[i], sourcePathArr[i]))
                {
                    for (int j = i; j < sourcePathArr.Length; j++)
                    {
                        result.Push("..");
                    }
                    for (int j = i; j < pathArr.Length; j++)
                    {
                        result.Push(pathArr[j]);
                    }
                    break;
                }
            }
            if (result.Count == 0)
            {
                if (pathArr.Length == sourcePathArr.Length)
                {
                    result.Push(".");
                }
                else if (pathArr.Length > sourcePathArr.Length)
                {
                    for (int i = sourcePathArr.Length; i < pathArr.Length; i++)
                    {
                        result.Push(pathArr[i]);
                    }
                }
                else if (pathArr.Length < sourcePathArr.Length)
                {
                    for (int i = pathArr.Length; i < sourcePathArr.Length; i++)
                    {
                        result.Push("..");
                    }
                }
            }
            return string.Join("\\", result.Reverse());
        }
    }
}
