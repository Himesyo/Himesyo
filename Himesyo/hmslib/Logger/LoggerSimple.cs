using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Himesyo.Check;
using Himesyo.Runtime;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Logger
{
    /// <summary>
    /// 简单的日志记录器
    /// </summary>
    public class LoggerSimple
    {
        /// <summary>
        /// 获取可用于同步对 <see cref="LoggerSimple"/> 的访问的对象。
        /// </summary>
        public static object SyncRoot { get; } = new object();
        /// <summary>
        /// 日志记录器是否可用(是否初始化成功。)
        /// </summary>
        public static bool CanWrite
        {
            get
            {
                return loger != null;
            }
        }

        /// <summary>
        /// 是否写入定位符。用于阅读器分析日志数据。默认为 <see langword="false"/> 。
        /// </summary>
        public static bool WriteLocator
        {
            get
            {
                return loger?.writeLocator ?? false;
            }
            set
            {
                if (loger != null)
                {
                    loger.writeLocator = value;
                }
            }
        }
        /// <summary>
        /// 是否写入调用日志方法的名称。它会影响程序运行的效率。默认为 <see langword="true"/> 。
        /// </summary>
        public static bool WriteCallLocation
        {
            get
            {
                return loger?.writeCallLocation ?? false;
            }
            set
            {
                if (loger != null)
                {
                    loger.writeCallLocation = value;
                }
            }
        }
        /// <summary>
        /// 是否写入 <see cref="LogLevel.Debug"/> 等级的日志。默认为 <see langword="true"/> 。
        /// </summary>
        public static bool WriteDebugLevel
        {
            get
            {
                return loger?.writeDebugLevel ?? false;
            }
            set
            {
                if (loger != null)
                {
                    loger.writeDebugLevel = value;
                }
            }
        }
        /// <summary>
        /// 单个文件大小上限。(单位：字节)
        /// </summary>
        public static long SingleLength
        {
            get => loger?.singleLength ?? default;
            set
            {
                if (loger != null)
                {
                    loger.singleLength = value;
                }
            }
        }
        /// <summary>
        /// 文件数量上限
        /// </summary>
        public static int FileMaxCount
        {
            get => loger?.fileMaxCount ?? default;
            set
            {
                if (loger != null)
                {
                    loger.fileMaxCount = value;
                }
            }
        }

        /// <summary>
        /// 当前时间的字符串表示形式。以 yyyy-MM-dd HH:mm:ss.fff 的格式。
        /// </summary>
        public static string Time => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        /// <summary>
        /// 当前线程的 <see cref="Thread.ManagedThreadId"/> 。
        /// </summary>
        public static int TID => Thread.CurrentThread.ManagedThreadId;
        /// <summary>
        /// 调用此方法的方法的名称。类.方法。
        /// </summary>
        public static string CallMethodName => GetMethodName();

        /// <summary>
        /// 使用指定目录和程序名称初始化记录器。
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="logClass"></param>
        /// <exception cref="IOException">日志文件打开失败。</exception>
        public static void Init(string rootPath, string logClass)
        {
            root = Path.GetFullPath(rootPath);
            name = logClass.Replace(" ", "");
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Loger";
            }
            Directory.CreateDirectory(root);
            string pathHash = root.ToLower().ComputeMD5().ToShow();
            eventWait = new EventWaitHandle(true, EventResetMode.AutoReset, $"Loger.{pathHash}.{name}");
            loger = new LoggerSimple();
        }
        /// <summary>
        /// 使用配置对象配置参数。设置前必须先进行初始化。
        /// </summary>
        /// <param name="config"></param>
        public static void SetConfig(ILoggerSimpleConfig config)
        {
            WriteCallLocation = config.WriteCallLocation;
            WriteDebugLevel = config.WriteDebugLevel;
            SingleLength = config.SingleLength;
            FileMaxCount = config.FileMaxCount;
        }
        /// <summary>
        /// 写入消息。
        /// </summary>
        /// <param name="level"></param>
        /// <param name="info"></param>
        public static void Write(LogLevel level, string info)
        {
            if (loger != null)
            {
                try
                {
                    eventWait.WaitOne();
                    loger.stream.Seek(0, SeekOrigin.End);
                    if (WriteDebugLevel || level != LogLevel.Debug)
                    {
                        string location = WriteCallLocation ? $"[[{TID}]{GetMethodName()}]" : $"[{TID}]";
                        if (loger.writeLocator)
                        {
                            loger.writer.Write(newInfo);
                            loger.writer.Write($"{Time} {location} {$"[{ level}]",-7} - ");
                            loger.writer.Write(start);
                            loger.writer.Write(info);
                            loger.writer.Write(end);
                            loger.Check();
                        }
                        else
                        {
                            loger.writer.Write(Environment.NewLine);
                            loger.writer.Write($"{Time} {location} {$"[{level}]",-7} - ");
                            loger.writer.Write(info);
                            loger.Check();
                        }
                    }
                }
                catch (Exception ex)
                {
                    File.AppendAllText(Path.Combine(root, "Loger.ErrorInfo"), $"{Time} [{name}] - {ex}");
                    loger.Open();
                }
                finally
                {
                    eventWait.Set();
                }
            }
        }
        /// <summary>
        /// 仅在 DEBUG 条件下写入调试信息。
        /// </summary>
        /// <param name="info"></param>
        [Conditional("DEBUG")]
        public static void WriteDebugOnly(string info)
        {
            Write(LogLevel.Debug, info);
        }
        /// <summary>
        /// 写入调试信息。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteDebug(string info)
        {
            Write(LogLevel.Debug, info);
        }
        /// <summary>
        /// 写入需要记录的消息。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfo(string info)
        {
            Write(LogLevel.Info, info);
        }
        /// <summary>
        /// 写入需要记录的消息。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteInfo(object info)
        {
            Write(LogLevel.Info, $"{info}");
        }
        /// <summary>
        /// 写入需要记录的警告。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteWarning(string info)
        {
            Write(LogLevel.Warn, info);
        }
        /// <summary>
        /// 写入需要记录的警告。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteWarning(object info)
        {
            Write(LogLevel.Warn, $"{info}");
        }
        /// <summary>
        /// 写入一个异常说明。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteError(string info)
        {
            Write(LogLevel.Error, info);
        }
        /// <summary>
        /// 写入一个异常说明和异常的详细信息。
        /// </summary>
        /// <param name="info"></param>
        /// <param name="ex"></param>
        public static void WriteError(string info, Exception ex)
        {
            Write(LogLevel.Error, $"{ex.GetType().Name} : {ex.Message} - {info}{GetErrorInfo(ex)}");
        }
        /// <summary>
        /// 写入一个异常对象。
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteError(Exception ex)
        {
            Write(LogLevel.Error, $"{ex.GetType().Name} - {ex.Message}{GetErrorInfo(ex)}");
        }
        /// <summary>
        /// 写入一个致命的信息。
        /// </summary>
        /// <param name="info"></param>
        public static void WriteFatal(string info)
        {
            Write(LogLevel.Fatal, info);
        }
        /// <summary>
        /// 关闭已打开的文件。
        /// </summary>
        public static void Close()
        {
            if (loger != null)
            {
                loger.writer?.Dispose();
                loger.stream?.Dispose();
                loger = null;
            }
            eventWait?.Dispose();
            eventWait = null;
        }

        /// <summary>
        /// 获取调用日志的方法名称
        /// </summary>
        /// <returns></returns>
        private static string GetMethodName()
        {
            var method = StackTraceHelper.GetLastCallFrame(typeof(LoggerSimple)).GetMethod();
            return $"{method.ReflectedType.Name}.{method.Name}";
        }
        private static string GetErrorInfo(Exception ex)
        {
            string info;
            if (loger?.writeLocator ?? false)
            {
                info = $"{Environment.NewLine}{new string(error)}{new string(start)}  {ex}{new string(end)}";
            }
            else
            {
                info = $"{Environment.NewLine}  {ex}";
            }
            return info;
        }

        private static readonly char[] newInfo = new char[] { '\r', '\n', '\u0017', '\u0001' };
        private static readonly char[] start = new char[] { '\u0002', '\u0006' };
        private static readonly char[] end = new char[] { '\u0003', '\u0006' };
        private static readonly char[] error = new char[] { '\u001b', '\u0001' };

        private static string root;
        private static string name;
        private static LoggerSimple loger;
        private static EventWaitHandle eventWait;

        private Stream stream;
        private StreamWriter writer;
        private bool writeLocator = false;
        private bool writeCallLocation = false;
        private bool writeDebugLevel = true;

        private long singleLength = 10 * 1024 * 1024;
        private int fileMaxCount = 20;

        private LoggerSimple()
        {
            Open();
        }
        private void Open()
        {
            try
            {
                writer?.Dispose();
                stream?.Dispose();
                eventWait.WaitOne();
                string[] logs = Directory.EnumerateFiles(root, $"{name} ????????-??????.???.log")
                    .OrderBy(logFile => logFile)
                    .ToArray();
                int delLength = logs.Length - 20;
                for (int i = 0; i < delLength; i++)
                {
                    try
                    {
                        File.Delete(logs[i]);
                    }
                    catch { }
                }
                string path = logs.LastOrDefault();
                if (path != null)
                {
                    FileInfo file = new FileInfo(path);
                    if (file.Length < 1000 * 1024 * 1024)
                    {
                        stream = new FileStream(file.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        writer = new StreamWriter(stream, Encoding.UTF8);
                        return;
                    }
                }
                Create();
            }
            catch (Exception ex)
            {
                File.AppendAllText(Path.Combine(root, "Loger.ErrorInfo"), $"{Time} [{name}] - {ex}");
            }
            finally
            {
                eventWait.Set();
            }
        }
        /// <summary>
        /// 创建新日志文件。
        /// </summary>
        private void Create()
        {
            writer?.Dispose();
            stream?.Dispose();
            DateTime time = DateTime.Now;
            while (true)
            {
                string path = GetFilePath(time);
                FileInfo file = new FileInfo(path);
                if (file.Exists && file.Length > 1000 * 1024 * 1024)
                {
                    time = time.AddMilliseconds(1);
                }
                else
                {
                    stream = new FileStream(file.FullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    writer = new StreamWriter(stream, Encoding.UTF8);
                    if (stream.Length == 0)
                    {
                        writer.Write($"# [{name}]");
                        writer.Flush();
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 判断文件是否已超过指定大小。
        /// </summary>
        private void Check()
        {
            writer.Flush();
            if (stream.Length >= 10 * 1024 * 1024)
            {
                Create();
            }
        }
        /// <summary>
        /// 从时间创建文件路径。
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private string GetFilePath(DateTime time)
        {
            return Path.Combine(root, $"{name} {time:yyyyMMdd-HHmmss.fff}.log");
        }
    }

    /// <summary>
    /// 表示此对象包含 <see cref="LoggerSimple"/> 类的配置参数。
    /// </summary>
    public interface ILoggerSimpleConfig
    {
        /// <summary>
        /// 是否写入调用日志方法的名称。它会影响程序运行的效率。
        /// </summary>
        bool WriteCallLocation { get; }

        /// <summary>
        /// 是否写入 <see cref="LogLevel.Debug"/> 等级的日志。
        /// </summary>
        bool WriteDebugLevel { get; }

        /// <summary>
        /// 单个文件大小上限。(单位：字节)
        /// </summary>
        long SingleLength { get; }

        /// <summary>
        /// 文件数量上限
        /// </summary>
        int FileMaxCount { get; }
    }
}
