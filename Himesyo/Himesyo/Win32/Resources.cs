using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Himesyo.Win32
{
    /// <summary>
    /// 提供与资源有关的 Win32 API。
    /// </summary>
    public class Resources
    {
        /// <summary>
        /// 从动态链接库中导出图标。
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Icon ExtractIcon(string fileName, int index)
        {
            try
            {
                IntPtr hIcon = ResourcesApi.ExtractIcon(IntPtr.Zero, fileName, index);
                if (hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(hIcon);
                    return icon;
                }
            }
            catch { }
            return null;
        }
    }

    internal class ResourcesApi
    {
        [DllImport("Shell32.dll")] //调用系统动态链接库
        public static extern IntPtr ExtractIcon(IntPtr handle, string fileName, int index);//获取句柄,shell32.dll文件位置,shell32图片的序号 全部提取就0-277

    }
}
