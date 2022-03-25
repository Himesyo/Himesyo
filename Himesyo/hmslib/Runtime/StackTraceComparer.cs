using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 堆栈帧比较器。
    /// </summary>
    public class StackTraceComparer : EqualityComparer<StackTrace>
    {
        /// <summary>
        /// 堆栈帧比较器实例。
        /// </summary>
        public static StackTraceComparer Comparer { get; } = new StackTraceComparer();

        /// <inheritdoc/>
        public override bool Equals(StackTrace x, StackTrace y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x == y) return true;
            var xCallList = x.GetFrames().Select(frame => frame.GetMethod());
            var yCallList = y.GetFrames().Select(frame => frame.GetMethod());
            bool result = xCallList.SequenceEqual(yCallList);
            return result;
        }

        /// <inheritdoc/>
        public override int GetHashCode(StackTrace obj)
        {
            int result = 0;
            foreach (var item in obj.GetFrames())
            {
                result ^= item.GetMethod().GetHashCode();
            }
            return result;
        }
    }
}
