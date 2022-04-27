using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himesyo
{
    /// <summary>
    /// 提供与抛出异常、显示异常等辅助方法。
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 当 <paramref name="value"/> 为 <see langword="null"/> 时，抛出 <see cref="ArgumentNullException"/> 异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static void ThrowNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// 当 <paramref name="condition"/> 为 <see langword="true"/> 时，抛出 <see cref="ArgumentException"/> 异常。
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        /// <param name="paramName"></param>
        public static void ThrowArgument(bool condition, string message, string paramName)
        {
            if (condition)
            {
                throw new ArgumentException(message.FormatNull() ?? "值不在预期的范围内。", paramName);
            }
        }

        /// <summary>
        /// 当 <paramref name="condition"/> 为 <see langword="true"/> 时，抛出 <see cref="InvalidOperationException"/> 异常。
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void ThrowInvalid(bool condition, string message)
        {
            if (condition)
            {
                throw new InvalidOperationException(message.FormatNull() ?? "对象的当前状态使该操作无效。");
            }
        }

        /// <summary>
        /// 当 <paramref name="condition"/> 为 <see langword="true"/> 时，抛出 <see cref="IndexOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="message"></param>
        public static void ThrowIndexOut(bool condition, string message)
        {
            if (condition)
            {
                throw new IndexOutOfRangeException(message.FormatNull() ?? "索引超出了数组界限。");
            }
        }

        /// <summary>
        /// 当 <paramref name="condition"/> 为 <see langword="true"/> 时，抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="paramName"></param>
        /// <param name="actualValue">实际值。</param>
        /// <param name="message"></param>
        public static void ThrowArgumentOut(bool condition, string paramName, object actualValue, string message)
        {
            if (condition)
            {
                throw new ArgumentOutOfRangeException(paramName, actualValue, message.FormatNull() ?? "指定的参数已超出有效值的范围。");
            }
        }

        /// <summary>
        /// 返回 <see cref="NotSupportedException"/> 异常。
        /// </summary>
        public static NotSupportedException NotSupported()
        {
            return new NotSupportedException();
        }
        /// <summary>
        /// 抛出 <see cref="NotSupportedException"/> 异常。
        /// </summary>
        /// <param name="message"></param>
        public static void ThrowNotSupported(string message)
        {
            throw new NotSupportedException(message.FormatNull() ?? "不支持所指定的方法。");
        }

        /// <summary>
        /// 返回 <see cref="ObjectDisposedException"/> 异常。
        /// </summary>
        /// <param name="objectName"></param>
        /// <returns></returns>
        public static ObjectDisposedException ObjectDisposed(string objectName)
        {
            return new ObjectDisposedException(objectName);
        }
        /// <summary>
        /// 抛出 <see cref="ObjectDisposedException"/> 异常。
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="message"></param>
        public static void ThrowObjectDisposed(string objectName, string message)
        {
            throw new ObjectDisposedException(objectName, message);
        }
    }
}
