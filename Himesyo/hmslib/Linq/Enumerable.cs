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
        /// 如果输入为 <see langword="null"/> 则返回空集合。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> NullToEmpty<TSource>(this IEnumerable<TSource> source)
        {
            return source ?? EmptyEnumerable<TSource>.Instance;
        }

        /// <summary>
        /// 对序列中的每个元素执行动作。
        /// </summary>
        /// <typeparam name="TSource"><paramref name="source"/> 中的元素的类型。</typeparam>
        /// <param name="source">一个值序列，要对该序列执行动作。</param>
        /// <param name="action">应用于每个元素的动作。</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="action"/> 为 <see langword="null"/> .</exception>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            ExceptionHelper.ThrowNull(source, nameof(source));
            ExceptionHelper.ThrowNull(action, nameof(action));

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
            ExceptionHelper.ThrowNull(source, nameof(source));
            ExceptionHelper.ThrowNull(action, nameof(action));

            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        /// <summary>
        /// 提升类型到 <see cref="IList{T}"/> 。
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IList<TSource> UpList<TSource>(this IEnumerable<TSource> source)
        {
            return source as IList<TSource> ?? source.ToList();
        }
    }

    /// <summary>
    /// 空数组。
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class EmptyEnumerable<TElement>
    {
        /// <summary>
        /// 实例。
        /// </summary>
        public static readonly TElement[] Instance = new TElement[0];
    }

    /// <summary>
    /// 默认选择器。
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class IdentityFunction<TElement>
    {
        /// <summary>
        /// 实例。
        /// </summary>
        public static Func<TElement, TElement> Instance
        {
            get { return x => x; }
        }
    }
}
