using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 辅助获取堆栈信息。
    /// </summary>
    public static class StackTraceHelper
    {
        /// <summary>
        /// 获取最后调用指定类型的堆栈函数调用。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stackFrames"></param>
        /// <returns></returns>
        public static StackFrame GetLastCallFrame(this Type type, IEnumerable<StackFrame> stackFrames = null)
        {
            IEnumerable<StackFrame> frames = stackFrames ?? new StackTrace(true).GetFrames();
            bool isFind = false;
            foreach (var frame in frames)
            {
                if (frame.GetMethod().DeclaringType == type)
                {
                    isFind = true;
                }
                else
                {
                    if (isFind)
                    {
                        return frame;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取调用指定类型之前的所有堆栈信息。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static StackTrace GetFramesAfterType(this Type type)
        {
            StackTrace frames = new StackTrace(false);
            bool isFind = false;
            for (int i = 0; i < frames.FrameCount; i++)
            {
                var frame = frames.GetFrame(i);
                if (frame.GetMethod().DeclaringType == type)
                {
                    isFind = true;
                }
                else
                {
                    if (isFind)
                    {
                        return new StackTrace(i, true);
                    }
                }
            }
            return new StackTrace(1, true);
        }
        /// <summary>
        /// 将单个函数调用转换成字符串表现形式。
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static string ToTraceString(this StackFrame frame)
        {
            return new StackTrace(frame).ToString();
        }
    }
}
