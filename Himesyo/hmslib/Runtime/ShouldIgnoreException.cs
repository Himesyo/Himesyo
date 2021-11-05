using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 表示一个应该被忽略的异常。
    /// </summary>
    [Serializable]
    public class ShouldIgnoreException : Exception, ISerializable
    {
        /// <summary>
        /// 表示此对象在创建时是否应输出信息。
        /// </summary>
        public static bool CreateShow { get; set; }
        /// <summary>
        /// 获取或设置标准输出流。它默认为控制台输出。
        /// </summary>
        public static TextWriter OutputStream { get; set; }

        static ShouldIgnoreException()
        {
            CreateShow = true;
            OutputStream = Console.Error;
        }

        /// <summary>
        /// 标识符。
        /// </summary>
        public int Identity { get; private set; }
        /// <summary>
        /// 打印日志类型。
        /// </summary>
        public ShouldIgnoreLog Log { get; private set; }
        /// <summary>
        /// 获取构造时的信息。
        /// </summary>
        public string ShowInfo { get; }

        /// <summary>
        /// 表示一个应该被忽略的异常。创建时输出所有信息。
        /// </summary>
        public ShouldIgnoreException()
            : this("被忽略的异常。", null, 0, ShouldIgnoreLog.All) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。创建时输出所有信息。
        /// </summary>
        public ShouldIgnoreException(string message)
            : this(message, null, 0, ShouldIgnoreLog.All) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。创建时输出所有信息。
        /// </summary>
        public ShouldIgnoreException(string message, Exception innerException)
            : this(message, innerException, 0, ShouldIgnoreLog.All) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。可以指定标识号 identity 和创建时输出的信息。
        /// </summary>
        public ShouldIgnoreException(int identity, ShouldIgnoreLog log)
            : this("被忽略的异常。", null, identity, log) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。可以指定标识号 identity 并在创建时输出所有信息。
        /// </summary>
        public ShouldIgnoreException(string message, int identity)
            : this(message, null, identity, ShouldIgnoreLog.All) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。可以指定标识号 identity 和创建时输出的信息。
        /// </summary>
        public ShouldIgnoreException(string message, int identity, ShouldIgnoreLog log)
            : this(message, null, identity, log) { }
        /// <summary>
        /// 表示一个应该被忽略的异常。可以指定标识号 identity 和创建时输出的信息。
        /// </summary>
        public ShouldIgnoreException(string message, Exception inner, int identity, ShouldIgnoreLog log)
            : base(message, inner)
        {
            Identity = identity;
            Log = log;

            ShowInfo = GetInfo(log);
            if (CreateShow && OutputStream != null)
            {
                OutputStream.WriteLine(ShowInfo);
            }
        }

        private string GetInfo(ShouldIgnoreLog log)
        {
            StringBuilder msg = new StringBuilder("发生 ShouldIgnoreException 类型的异常。", 255);
            if (log.HasFlag(ShouldIgnoreLog.Message))
            {
                msg.AppendFormat("{0}", Message);
                msg.SureEndString("。");
            }
            if (log.HasFlag(ShouldIgnoreLog.Identity))
            {
                msg.AppendFormat("identity: {0}。", Identity);
            }
            if (Log.HasFlag(ShouldIgnoreLog.FullInnerException))
            {
                Exception exception = InnerException;
                while (exception != null)
                {
                    msg.SureEndString(Environment.NewLine);
                    msg.AppendFormat("        - 导致此异常的 {0} 信息：", exception.GetType().FullName);
                    msg.AppendFormat("{0}", exception.Message);
                    msg.SureEndString("。");
                    exception = exception.InnerException;
                }
            }
            else if (log.HasFlag(ShouldIgnoreLog.InnerException) && InnerException != null)
            {
                msg.AppendFormat("InnerException：{0}", InnerException.Message);
                msg.SureEndString("。");
            }
            if (log.HasFlag(ShouldIgnoreLog.StackTrace))
            {
                msg.SureEndString(Environment.NewLine);
                msg.Append(StackTraceHelper.GetFramesAfterType(typeof(ShouldIgnoreException)).ToString());
            }
            else if (log.HasFlag(ShouldIgnoreLog.Method))
            {
                msg.Append(StackTraceHelper.GetLastCallFrame(typeof(ShouldIgnoreException)).ToTraceString());
                msg.TrimEnd("\r\n", false);
                msg.SureEndString("。");
            }
            string value = msg.ToString();
            return value;
        }

        /// <summary>
        /// 序列化构造函数。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ShouldIgnoreException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Identity = info.GetInt32("ShouldIgnoreException.Identity");
            Log = (ShouldIgnoreLog)info.GetInt32("ShouldIgnoreException.Log");
            ShowInfo = info.GetString("ShouldIgnoreException.ShowInfo");
        }
        /// <summary>
        /// 使用将目标对象序列化所需的数据填充 <see cref="SerializationInfo"/> 。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ShouldIgnoreException.Identity", Identity);
            info.AddValue("ShouldIgnoreException.Log", (int)Log);
            info.AddValue("ShouldIgnoreException.ShowInfo", ShowInfo);
        }
    }

    /// <summary>
    /// 显示的日志类型。
    /// </summary>
    [Flags]
    public enum ShouldIgnoreLog
    {
        /// <summary>
        /// 无指定内容。
        /// </summary>
        None = 0x00,
        /// <summary>
        /// 输出标志号。
        /// </summary>
        Identity = 0x001,
        /// <summary>
        /// 输出消息文本。
        /// </summary>
        Message = 0x002,
        /// <summary>
        /// 输出调用方法。
        /// </summary>
        Method = 0x004,

        /// <summary>
        /// 输出 <see cref="Exception.InnerException"/> 。
        /// </summary>
        InnerException = 0x010,
        /// <summary>
        /// 输出所有 <see cref="Exception.InnerException"/> 。
        /// </summary>
        FullInnerException = 0x020,

        /// <summary>
        /// 输出调用堆栈信息。
        /// </summary>
        StackTrace = 0x100,

        /// <summary>
        /// 输出全部信息。
        /// </summary>
        All = 0x127
    }
}
