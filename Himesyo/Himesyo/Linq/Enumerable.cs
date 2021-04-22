using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SystemEnumerable = System.Linq.Enumerable;

namespace Himesyo.Linq
{
    /// <summary>
    /// 为 <see cref="IEnumerable{T}"/> 提供扩展。
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// 对序列中的每个元素执行动作。
        /// </summary>
        /// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
        /// <param name="source">一个值序列，要对该序列执行动作。</param>
        /// <param name="action">应用于每个元素的动作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="action"/> 为 <see langword="null"/> .</exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
            }
        }
        
        /// <summary>
        /// 对序列中的每个元素执行动作并返回。
        /// </summary>
        /// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
        /// <param name="source">一个值序列，要对该序列执行动作。</param>
        /// <param name="action">应用于每个元素的动作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="action"/> 为 <see langword="null"/> .</exception>
        public static IEnumerable<TSource> ForEachReturn<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }
    }
}
