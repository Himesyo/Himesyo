using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 调用位置监视器。
    /// </summary>
    public class CallMonitor
    {
        /// <summary>
        /// 此对象关联的停止方法。
        /// </summary>
        public MethodBase StopMethod { get; set; }

        /// <summary>
        /// 默认构造函数。
        /// </summary>
        public CallMonitor() { }

        /// <summary>
        /// 创建一个新对象并设置关联的停止方法。
        /// </summary>
        /// <param name="stopMethod"></param>
        public CallMonitor(MethodBase stopMethod)
        {
            StopMethod = stopMethod;
        }

        /// <summary>
        /// 调用记录。
        /// </summary>
        public List<CallInfo> CallRecord { get; } = new List<CallInfo>();

        /// <summary>
        /// 添加一个调用消息。
        /// </summary>
        /// <param name="callInfo"></param>
        public void Add(CallInfo callInfo)
        {
            CallRecord.Add(callInfo);
        }

        /// <summary>
        /// 分析调用来源。
        /// </summary>
        /// <returns></returns>
        public ILookup<StackTrace, CallInfo> Analyse()
        {
            ILookup<StackTrace, CallInfo> result = CallRecord.ToLookup(
                call => call.StackTrace,
                call => call,
                StackTraceComparer.Comparer);
            return result;
        }

        /// <summary>
        /// 分析调用来源并打印。
        /// </summary>
        /// <returns></returns>
        public string Print(string monitorName)
        {
            if (CallRecord.Count > 0)
            {
                var call = Analyse();
                StringBuilder builder = new StringBuilder();
                builder.Append(monitorName);
                builder.AppendLine(new string('-', 60));
                string path;
                int count;
                foreach (IGrouping<StackTrace, CallInfo> st in call)
                {
                    path = st.Key.ToString();
                    count = st.Count();
                    builder.AppendLine($"【{count}】\r\n{path}");
                }
                builder.AppendLine(new string('-', 80));
                string result = builder.ToString();
                Console.WriteLine(result);
                return result;
            }
            return string.Empty;
        }
    }
}
