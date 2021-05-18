using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Himesyo.Collections;
using Himesyo.Runtime.Extend;

using TState = System.Threading.ThreadState;

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

    /// <summary>
    /// 线程诊断器
    /// </summary>
    [DebuggerDisplay("Diagnotor {GetShow(Thread)} [{diagnosisResult}]")]
    public class Diagnotor
    {
        private static readonly TState[] invalidStates = new TState[]
        {
            TState.Stopped,
            TState.Aborted,
            TState.AbortRequested,
            TState.Unstarted
        };

        private static readonly MethodInfo methodRun;
        static Diagnotor()
        {

        }

        private Timer timer;
        private DiagnotorConfig runConfig;
        private BackQueue<RunInfo> runInfos;
        private BackQueue<RunInfoChange> runInfoChange;

        private int tally = 0;

        public event EventHandler<OutputEventArgs> Output;
        public event EventHandler<StagnationEventArgs> Stagnation;
        public Thread Thread { get; }
        public DiagnotorConfig Config { get; }

        public Diagnotor(Thread thread, DiagnotorConfig config)
        {
            Thread = thread;
            Config = config ?? new DiagnotorConfig();
            runConfig = Config.Check();
            runInfos = new BackQueue<RunInfo>(runConfig.StackSize);
            runInfoChange = new BackQueue<RunInfoChange>(runConfig.StackSize);

            Thread.SetData(this);
        }

        public void Run()
        {
            timer = new Timer(Update, runConfig, 0, runConfig.Interval);
        }
        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }
        protected void OnOutput(OutputEventArgs args)
        {
            Output?.Invoke(this, args);
        }
        protected void OnStagnation(StagnationEventArgs args)
        {
            Stagnation?.Invoke(this, args);
        }


        private string diagnosisResult = "未含有诊断结果";

        private bool atUpdate = false;
        private void Update(object state)
        {
            if (atUpdate)
                return;

            atUpdate = true;
            DiagnotorConfig config = state as DiagnotorConfig ?? new DiagnotorConfig();
            try
            {
                StackTrace stackTrace = GetStackTrace();
                RunInfo runInfo = new RunInfo(this, stackTrace);
                runInfos.Add(runInfo);

                Diagnosis();
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    string show = GetShow(Thread);
                    OnOutput(new OutputEventArgs(OutputType.Error, $"{show} {ex}", Thread));
                });
            }
            finally
            {
                atUpdate = false;
            }
        }

        RunInfo nullStart = null;
        int nullCount = 0;
        RunInfo nullEnd = null;
        RunInfo upValue = null;
        RunInfo upInfo = null;

        RunInfo methodStartInfo = null;

        private void Diagnosis()
        {
            if (runInfos == null || runInfos.Count < 5)
                return;

            for (int i = 0, length = runInfos.Count; i < length; i++)
            {
                RunInfo info = runInfos[i];

                if (info == upInfo)
                    break;

                if (info.StackTrace == null)
                {
                    if (nullStart == null)
                    {
                        nullStart = info;
                    }
                    nullCount++;
                    nullEnd = info;
                }
                if (runConfig.GetStackTraceErrorCount != -1 && nullCount > 0 && nullCount % runConfig.GetStackTraceErrorCount == 0)
                {
                    int time = nullEnd.Runtime - nullStart.Runtime;
                    if (runConfig.GetStackTraceErrorTime != -1 && time > runConfig.GetStackTraceErrorTime)
                    {
                        string show = GetShow(Thread);
                        OnOutput(new OutputEventArgs(OutputType.Warn, $"{show}  已连续 {nullCount} 次(共计 {time} 毫秒)获取堆栈信息失败。", Thread));
                    }
                }
                if (info.StackTrace != null)
                {
                    nullStart = null;
                    nullCount = 0;
                    nullEnd = null;

                    if (upValue != null)
                    {
                        RunInfoChange change = new RunInfoChange(info, upValue);
                        runInfoChange.Add(change);

                        FrameInfo currentframeInfo = change.Change[0];
                        MethodBase method = null;
                        if (currentframeInfo == null)
                        {
                            OnOutput(new OutputEventArgs(OutputType.Warn, $"未获取到堆栈列表。{change.NewRunInfo}。", Thread));
                        }
                        else
                        {
                            method = currentframeInfo.StackFrame.GetMethod();
                            OnOutput(new OutputEventArgs(OutputType.Deta, $"当前位于 {method.Show()} ({change.NewRunInfo})", Thread));
                        }

                        if (change.Change.Length < 3)
                        {
                            if (methodStartInfo != null && method != null
                                && method.Name != "WaitMessage")
                            {
                                int time = change.NewRunInfo.Runtime - methodStartInfo.Runtime;
                                if (time > 3000)
                                {
                                    OnOutput(new OutputEventArgs(OutputType.Warn, $"{method.Show()} 方法已超时。", Thread));
                                    OnStagnation(new StagnationEventArgs(method, change.NewRunInfo.StackTrace, Thread));
                                    methodStartInfo = null;
                                }
                            }
                        }
                        else
                        {
                            methodStartInfo = change.NewRunInfo;
                            OnOutput(new OutputEventArgs(OutputType.Deta, $"详细调用步骤。{change.NewRunInfo}\r\n{Expand(change)}", Thread));
                        }
                    }

                    upValue = info;
                }
            }

            upInfo = runInfos[0];
        }

        private string Expand(RunInfoChange change)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 1, length = change.Change.Length; i < length; i++)
            {
                FrameInfo frame = change.Change[i];
                MethodBase method = frame.StackFrame.GetMethod();
                string action = frame.IsInvoke ? "调用" : "离开";
                string msg = $"    {action} {method.Show()}";
                builder.AppendLine(msg);
            }
            return builder.ToString();
        }

        private StackTrace GetStackTrace()
        {
#pragma warning disable CS0618 // 类型或成员已过时 Thread.Suspend(); Thread.Resume();
            if (Thread == null || invalidStates.Contains(Thread.ThreadState))
                return null;

            StackTrace result = null;
            try
            {
                Thread.Suspend();
                if (Thread.ThreadState == TState.Running)
                    return null;

                result = new StackTrace(Thread, false);
            }
            catch (Exception ex)
            {
                Task.Factory.StartNew(() =>
                {
                    string show = GetShow(Thread);
                    OnOutput(new OutputEventArgs(OutputType.Error, $"{show} {ex}", Thread));
                });
            }
            finally
            {
                try
                {
                    Thread.Resume();
                }
                catch { }
            }
            return result;
#pragma warning restore CS0618 // 类型或成员已过时
        }
        private string GetShow(Thread thread)
        {
            string show = thread.Name;
            if (string.IsNullOrWhiteSpace(show))
            {
                show = thread.ManagedThreadId.ToString();
            }
            else
            {
                show = $"{show} ({thread.ManagedThreadId})";
            }
            return show;
        }

        [DebuggerDisplay("RunInfo [Tally = {Tally}, Runtime = {Runtime}, HasStackTrace = {StackTrace != null}]")]
        private class RunInfo
        {
            public Diagnotor Parent { get; }
            public int Tally { get; }
            public int Runtime { get; }
            public StackTrace StackTrace { get; }

            public RunInfo(Diagnotor diagnotor, StackTrace stackTrace)
            {
                Parent = diagnotor;
                Tally = diagnotor.tally++;
                Runtime = Environment.TickCount;
                StackTrace = stackTrace;
            }

            public override string ToString()
            {
                return $"计数 {Tally}, 时间：{Runtime} ( {Parent.Thread.ManagedThreadId} {Parent.Thread.Name})";
            }
        }

        [DebuggerDisplay("FrameInfo [{DebuggerDisplay()}]")]
        private class FrameInfo
        {
            public StackFrame StackFrame { get; }
            public bool IsInvoke { get; }

            public FrameInfo(StackFrame stackFrame, bool isInvoke)
            {
                StackFrame = stackFrame;
                IsInvoke = isInvoke;
            }

            private string DebuggerDisplay()
            {
                string invoke = IsInvoke ? "Invoke" : string.Empty;
                MethodBase method = StackFrame.GetMethod();
                return $"{invoke,6} {method.Show()}";
            }
        }

        [DebuggerDisplay("RunInfoChange [ChangeCount = {Change.Length}, NewRunInfo = {NewRunInfo}]")]
        private class RunInfoChange
        {
            public RunInfo NewRunInfo { get; }
            public RunInfo OldRunInfo { get; }
            public FrameInfo[] Change { get; }

            public RunInfoChange(RunInfo newRunInfo, RunInfo oldRunInfo)
            {
                NewRunInfo = newRunInfo;
                OldRunInfo = oldRunInfo;
                Change = GetChange(newRunInfo.StackTrace, oldRunInfo.StackTrace);
            }

            private FrameInfo[] GetChange(StackTrace newStackTrace, StackTrace oldStackTrace)
            {
                StackFrame[] newFrames = newStackTrace.GetFrames();
                StackFrame[] oldFrames = oldStackTrace.GetFrames();

                Stack<FrameInfo> result = new Stack<FrameInfo>();
                if (newFrames.Length > 0)
                {
                    result.Push(new FrameInfo(newFrames[0], true));
                }
                else
                {
                    result.Push(null);
                }
                for (int i = 0; ; i++)
                {
                    if (i == newFrames.Length)
                    {
                        for (int length = oldFrames.Length; i < length; i++)
                        {
                            result.Push(new FrameInfo(oldFrames[length - i - 1], false));
                        }
                        break;
                    }
                    else if (i == oldFrames.Length)
                    {
                        for (int length = newFrames.Length; i < length; i++)
                        {
                            result.Push(new FrameInfo(newFrames[length - i - 1], true));
                        }
                        break;
                    }
                    else if (newFrames[newFrames.Length - i - 1].GetMethod() != oldFrames[oldFrames.Length - i - 1].GetMethod())
                    {
                        for (int j = i, length = oldFrames.Length; j < length; j++)
                        {
                            result.Push(new FrameInfo(oldFrames[length - j - 1], false));
                        }
                        for (int j = i, length = newFrames.Length; j < length; j++)
                        {
                            result.Push(new FrameInfo(newFrames[length - j - 1], true));
                        }
                        break;
                    }
                }
                return result.Reverse().ToArray();
            }

        }
    }

    public class OutputEventArgs : EventArgs
    {
        public OutputType Type { get; }
        public string Value { get; }
        public Thread Thread { get; }
        public OutputEventArgs(OutputType outputType, string value, Thread thread)
        {
            Type = outputType;
            Value = value;
            Thread = thread;
        }
    }
    public class StagnationEventArgs : EventArgs
    {
        public MethodBase Method { get; }
        public Thread Thread { get; }
        public StackTrace StackTrace { get; }
        public StagnationEventArgs(MethodBase method, StackTrace stackTrace, Thread thread)
        {
            Method = method;
            StackTrace = stackTrace;
            Thread = thread;
        }
    }

    public enum OutputType
    {
        /// <summary>
        /// 普通信息。
        /// </summary>
        Info,
        /// <summary>
        /// 详细信息。
        /// </summary>
        Deta,
        /// <summary>
        /// 警告。发现了一些不正常的情况。
        /// </summary>
        Warn,
        /// <summary>
        /// 错误。遇到了异常情况。
        /// </summary>
        Error
    }

    /// <summary>
    /// 线程诊断配置
    /// </summary>
    [DebuggerDisplay("DiagnotorConfig [StackSize = {StackSize}, Interval = {Interval}]")]
    public class DiagnotorConfig
    {
        /// <summary>
        /// 其值在 10 - 1,000,000 之间。默认为 60 。
        /// </summary>
        public int StackSize { get; set; } = 60;

        /// <summary>
        /// 检测间隔。其值在 100 - 1,000,000 之间。默认为 10,000 。
        /// </summary>
        public int Interval { get; set; } = 10000;

        /// <summary>
        /// 连续获取堆栈信息失败的报警数量。其值在 5 - 1,000 之间。默认为 10 。如果不需要报警，请设置为 -1 。
        /// </summary>
        public int GetStackTraceErrorCount { get; set; } = 10;

        /// <summary>
        /// 连续获取堆栈信息失败的报警时间。其值在 1,000 - 10,000,000 之间。默认为 10,000 。如果不需要报警，请设置为 -1 。
        /// </summary>
        public int GetStackTraceErrorTime { get; set; } = 10000;

        public DiagnotorConfig Clone()
        {
            DiagnotorConfig other = (DiagnotorConfig)MemberwiseClone();
            return other;
        }

        public DiagnotorConfig Check()
        {
            DiagnotorConfig other = Clone();

            if (other.StackSize < 10)
            {
                other.StackSize = 10;
            }
            if (other.StackSize > 1000000)
            {
                other.StackSize = 1000000;
            }

            if (other.Interval < 100)
            {
                other.Interval = 100;
            }
            if (other.Interval > 1000000)
            {
                other.Interval = 1000000;
            }

            if (other.GetStackTraceErrorCount != -1)
            {
                if (other.GetStackTraceErrorCount < 5)
                {
                    other.GetStackTraceErrorCount = 5;
                }
                if (other.GetStackTraceErrorCount > 1000)
                {
                    other.GetStackTraceErrorCount = 1000;
                }
            }

            if (other.GetStackTraceErrorTime != -1)
            {
                if (other.GetStackTraceErrorTime < 1000)
                {
                    other.GetStackTraceErrorTime = 1000;
                }
                if (other.GetStackTraceErrorTime > 10000000)
                {
                    other.GetStackTraceErrorTime = 10000000;
                }
            }


            return other;
        }
    }
}
