using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Himesyo.Win32
{
    /// <summary>
    /// 提供与窗体有关的 Win32 API
    /// </summary>
    public static class Window
    {
        /// <summary>
        /// 查找指定窗体的句柄。
        /// </summary>
        /// <param name="lpClassName">窗体类名。可为 null 。</param>
        /// <param name="lpWindowName">窗体标题</param>
        /// <returns></returns>
        public static IntPtr FindWindow(string lpClassName, string lpWindowName)
        {
            return FindWindowA(lpClassName, lpWindowName);
        }

        /// <summary>
        /// 显示窗体
        /// </summary>
        public static void ShowWindow(IntPtr handle, ShowWindowType showType)
        {
            ShowWindowAsync(handle, (int)showType);
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        /// <summary>
        /// windowapi 通过句柄显示或隐藏窗体函数
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="cmdShow">显示类型（0：隐藏窗体，1：默认大小窗体，2：最小化窗体，3：最大化窗体）</param>
        /// <returns>返回成功或失败</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindowAsync", SetLastError = true)]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        [DllImport("user.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(
            IntPtr hWnd,   // handle to destination window
            int Msg,    // message
            int wParam, // first message parameter
            int lParam  // second message parameter
        );

    }

    /// <summary>
    /// 显示窗体类型
    /// </summary>
    public enum ShowWindowType
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Hide,
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        /// <summary>
        /// 最小化
        /// </summary>
        Min,
        /// <summary>
        /// 最大化
        /// </summary>
        Max
    }
}
