using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Himesyo
{
    /// <summary>
    /// 跨线程异步传递消息。如果多个位置同时接受消息将阻塞。
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class FixedThread<TResult> : IDisposable
    {
        private readonly object sync = new object();
        private TaskCompletionSource<TResult> source;
        private ConcurrentQueue<TResult> results = new ConcurrentQueue<TResult>();

        /// <summary>
        /// 是否含有暂存的值。如果有值，<see cref="WaitMessage()"/> 方法将直接返回。
        /// </summary>
        public bool HasValue => results.Count > 0;

        /// <summary>
        /// 当前是否在挂起状态。 
        /// </summary>
        public bool IsSuspend { get; private set; } = false;

        /// <summary>
        /// 是否已调用 <see cref="Dispose"/> 方法。
        /// </summary>
        [Browsable(false)]
        public bool IsDispose { get; private set; } = false;

        /// <summary>
        /// 释放完成后触发。
        /// </summary>
        public event EventHandler Disposed;
        /// <summary>
        /// 接收到结果前触发。已挂起时也会触发。
        /// </summary>
        public event EventHandler<ResultReceivingEventArgs<TResult>> ResultReceiving;

        /// <summary>
        /// 重置队列。清除所有未处理的消息。
        /// </summary>
        public void Clear()
        {
            results = new ConcurrentQueue<TResult>();
        }

        /// <summary>
        /// 挂起消息接收，不再接收新结果。直到调用 <see cref="Resume"/> 恢复消息接收。
        /// </summary>
        public void Suspend()
        {
            IsSuspend = true;
        }

        /// <summary>
        /// 恢复由 <see cref="Suspend"/> 方法挂起的消息接收。
        /// </summary>
        public void Resume()
        {
            IsSuspend = false;
        }

        /// <summary>
        /// 设置一个结果，如果已等待则通知。
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(TResult result)
        {
            var args = new ResultReceivingEventArgs<TResult>(result, IsSuspend, IsDispose);
            OnResultReceiving(args);
            if (args.Cancel || IsSuspend || IsDispose)
            {
                return;
            }
            lock (sync)
            {
                if (source == null)
                {
                    results.Enqueue(result);
                }
                else
                {
                    var temp = source;
                    source = null;
                    temp.SetResult(result);
                }
            }
        }

        /// <summary>
        /// 获取一个消息。如果 <see cref="IsDispose"/> 为 <see langword="true"/>，则直接返回 <see langword="default"/>(<typeparamref name="TResult"/>) 。
        /// </summary>
        /// <returns></returns>
        public Task<TResult> WaitMessage()
        {
            if (IsDispose)
            {
                return Task.FromResult<TResult>(default);
            }
            lock (sync)
            {
                if (results.TryDequeue(out TResult result))
                {
                    source = null;
                    return Task.FromResult(result);
                }
                else
                {
                    source = new TaskCompletionSource<TResult>();
                    return source.Task;
                }
            }
        }

        /// <summary>
        /// 获取一个消息。如果 <see cref="IsDispose"/> 为 <see langword="true"/>，则直接返回 <see langword="default"/>(<typeparamref name="TResult"/>) 。
        /// </summary>
        /// <returns></returns>
        public Task<TResult> WaitMessage(CancellationToken token)
        {
            if (IsDispose)
            {
                return Task.FromResult<TResult>(default);
            }
            lock (sync)
            {
                if (results.TryDequeue(out TResult result))
                {
                    source = null;
                    return Task.FromResult(result);
                }
                else
                {
                    TaskCompletionSource<TResult> completionSource = new TaskCompletionSource<TResult>();
                    token.Register(() => completionSource.TrySetResult(default));
                    source = completionSource;
                    return source.Task;
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            IsDispose = true;
            results = new ConcurrentQueue<TResult>();
            source?.TrySetResult(default);
            source = null;
            Disposed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 引发 <see cref="ResultReceiving"/> 事件。
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnResultReceiving(ResultReceivingEventArgs<TResult> args)
        {
            ResultReceiving?.Invoke(this, args);
        }
    }

    /// <summary>
    /// 接收到消息触发的事件的参数。
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ResultReceivingEventArgs<TResult> : CancelEventArgs
    {
        /// <summary>
        /// 接收到的结果。
        /// </summary>
        public TResult Result { get; }
        /// <summary>
        /// 当前是否在挂起状态。
        /// </summary>
        public bool IsSuspend { get; }
        /// <summary>
        /// 当前是否已释放。
        /// </summary>
        public bool IsDispose { get; }

        /// <summary>
        /// 使用所有必须属性初始化新实例。
        /// </summary>
        /// <param name="result">结果</param>
        /// <param name="isSuspend">是否已挂起</param>
        /// <param name="isDispose">是否已释放</param>
        public ResultReceivingEventArgs(TResult result, bool isSuspend, bool isDispose)
        {
            Result = result;
            IsSuspend = isSuspend;
            IsDispose = isDispose;
        }
    }
}
