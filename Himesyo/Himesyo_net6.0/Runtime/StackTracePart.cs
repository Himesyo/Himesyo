using System.Diagnostics;
using System.Linq;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 表示堆栈帧的一部分。
    /// </summary>
    public class StackTracePart : StackTrace
    {
        /// <summary>
        /// 内部包含的堆栈调用列表。
        /// </summary>
        protected StackFrame[] innerFrames;
        /// <inheritdoc/>
        public override int FrameCount => innerFrames.Length;

        /// <summary>
        /// 使用指定 <see cref="StackFrame"/> 数组创建 <see cref="StackTracePart"/> 的新实例。
        /// </summary>
        /// <param name="frames"></param>
        public StackTracePart(params StackFrame[] frames)
        {
            innerFrames = frames ?? new StackFrame[0];
        }

        /// <summary>
        /// 使用指定 <see cref="StackTrace"/> 的一部分创建 <see cref="StackTracePart"/> 的新实例。
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        public StackTracePart(StackTrace stackTrace, int startIndex, int count)
        {
            innerFrames = stackTrace?
                .GetFrames()
                .Skip(startIndex)
                .Take(count)
                .ToArray() ?? new StackFrame[0];
        }

        /// <inheritdoc/>
        public override StackFrame GetFrame(int index)
        {
            return innerFrames[index];
        }
        /// <inheritdoc/>
        public override StackFrame[] GetFrames()
        {
            return innerFrames.ToArray();
        }
    }
}
