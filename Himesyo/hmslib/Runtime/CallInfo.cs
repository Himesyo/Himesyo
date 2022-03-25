using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 表示一个调用信息。
    /// </summary>
    public class CallInfo
    {
        /// <summary>
        /// 在当前位置创建调用信息。
        /// </summary>
        /// <returns></returns>
        public static CallInfo Create()
        {
            CallInfo callInfo = new CallInfo()
            {
                StackTrace = new StackTrace(1),
                CallTime = DateTime.Now
            };
            return callInfo;
        }

        /// <summary>
        /// 在当前位置创建调用信息，它仅追踪到指定的方法。
        /// </summary>
        /// <param name="stopMethod"></param>
        /// <returns></returns>
        public static CallInfo Create(MethodBase stopMethod)
        {
            StackTrace stackTrace = new StackTrace(1);
            if (stopMethod != null)
            {
                var all = stackTrace.GetFrames();
                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i].GetMethod() == stopMethod)
                    {
                        stackTrace = new StackTracePart(all.Take(i).ToArray());
                    }
                }
            }
            CallInfo callInfo = new CallInfo()
            {
                StackTrace = stackTrace,
                CallTime = DateTime.Now
            };
            return callInfo;
        }

        /// <summary>
        /// 调用堆栈信息。
        /// </summary>
        public StackTrace StackTrace { get; set; }
        /// <summary>
        /// 创建调用信息的时间。
        /// </summary>
        public DateTime CallTime { get; set; }
        /// <summary>
        /// 此次调用所关联的数据。
        /// </summary>
        public Dictionary<string, object> Data { get; } = new Dictionary<string, object>();

    }
}
