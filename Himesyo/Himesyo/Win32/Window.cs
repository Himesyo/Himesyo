using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

using static Himesyo.Win32.WindowsApi;

namespace Himesyo.Win32
{
    /// <summary>
    /// 提供与窗体有关的 Win32 API
    /// </summary>
    public static class Windows
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

        /// <summary>
        /// 对指定句柄发送消息。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam = default, string lParam = default)
        {
            return WindowsApi.SendMessage(hWnd, msg, wParam, lParam);
        }

        /// <summary>
        /// 对指定句柄发送消息。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, StringBuilder lParam)
        {
            return WindowsApi.SendMessage(hWnd, msg, wParam, lParam);
        }

        /// <summary>
        /// 对指定句柄发送消息。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref IntPtr lParam)
        {
            return WindowsApi.SendMessage(hWnd, msg, wParam, ref lParam);
        }

        /// <summary>
        /// 对指定句柄发送消息。异步。
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public static bool PostMessage(HandleRef hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            return WindowsApi.PostMessage(hWnd, msg, wParam, lParam);
        }
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

    internal static class WindowsApi
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        /// <summary>
        /// windowapi 通过句柄显示或隐藏窗体函数
        /// </summary>
        /// <param name="hWnd">窗体句柄</param>
        /// <param name="cmdShow">显示类型（0：隐藏窗体，1：默认大小窗体，2：最小化窗体，3：最大化窗体）</param>
        /// <returns>返回成功或失败</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindowAsync", SetLastError = true)]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        #region SendMessage

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        #endregion

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
