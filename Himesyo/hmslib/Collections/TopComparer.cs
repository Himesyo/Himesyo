/*************************************************
 * 描述：
 * 
 * Author：Himesyo<183147241@qq.com>
 * Date：2021/5/15 16:11:00
 * Create：带有自定义权值的比较器。可用于指定值置顶或置底。
 * ************************************************/

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Himesyo.Collections
{
    /// <summary>
    /// 带有自定义权值的比较器。可用于指定值置顶或置底。比较对象必须重写 <see cref="object.ToString"/> 方法才可以置顶。
    /// </summary>
    public class TopComparer<T> : Comparer<T>, IComparer<T>
    {
        private readonly Dictionary<T, int> weights;

        /// <summary>
        /// 值和权重的字典。值越大表示越靠前。
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="comparer"></param>
        public TopComparer(Dictionary<T, int> weight, IEqualityComparer<T> comparer)
        {
            if (weight.Count > 0)
            {
                weights = new Dictionary<T, int>(weight, comparer);
            }
        }

        /// <summary>
        /// 比较指定对象。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(T x, T y)
        {
            // 注意 权值和排序值是相反的：权值越大 = 排序值越小 = 越靠前

            if (ReferenceEquals(x, null))
            {
                if (ReferenceEquals(y, null) || weights == null)
                {
                    // 均为 null 位置一样
                    return 0;
                }
                else
                {
                    int yWeight;
                    if (weights.TryGetValue(y, out yWeight))
                    {
                        // y 有权值返回权值（注意此时是相对于 x 并且权值与排序值相反）
                        return yWeight;
                    }
                    else
                    {
                        // y 没有权值认为 null 在前面（注意此时是相对于 x）
                        return -1;
                    }
                }
            }

            // 如果权字典为空直接使用默认比较器
            if (weights == null)
            {
                return Default.Compare(x, y);
            }

            int xWeight;
            if (ReferenceEquals(y, null))
            {
                if (weights.TryGetValue(x, out xWeight))
                {
                    // x 有权值返回负 x 的权值（权值越高值越小越在最前）
                    return -xWeight;
                }
                else
                {
                    // x 没有权值使用默认比较器
                    return Default.Compare(x, y);
                }
            }
            else
            {
                int yWeight;
                weights.TryGetValue(x, out xWeight);
                weights.TryGetValue(y, out yWeight);

                if (xWeight == yWeight)
                {
                    // 权值相等再使用默认比较器比较
                    return Default.Compare(x, y);
                }

                // 返回权值差（因为权值与排序值相反所以使用 y 的权值减去 x 的权值）
                return yWeight - xWeight;
            }
        }
    }


    /// <summary>
    /// 带有自定义权值的比较器。可用于指定值置顶或置底。比较对象必须重写 <see cref="object.ToString"/> 方法才可以置顶。
    /// </summary>
    public class TopComparer : Comparer<object>, IComparer<object>
    {
        private readonly Dictionary<object, int> weights;
        /// <summary>
        /// 值和权重的字典。值越大表示越靠前。
        /// </summary>
        /// <param name="weight"></param>
        /// <param name="comparer">为 <see langword="null"/> 时使用默认比较器。</param>
        public TopComparer(Dictionary<object, int> weight, IEqualityComparer<object> comparer)
        {
            if (weight.Count > 0)
            {
                weights = new Dictionary<object, int>(weight, comparer);
            }
        }
        /// <summary>
        /// 比较指定对象。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(object x, object y)
        {
            // 注意 权值和排序值是相反的：权值越大 = 排序值越小 = 越靠前

            if (ReferenceEquals(x, null))
            {
                if (ReferenceEquals(y, null) || weights == null)
                {
                    // 均为 null 位置一样
                    return 0;
                }
                else
                {
                    int yWeight;
                    if (weights.TryGetValue(y, out yWeight))
                    {
                        // y 有权值返回权值（注意此时是相对于 x 并且权值与排序值相反）
                        return yWeight;
                    }
                    else
                    {
                        // y 没有权值认为 null 在前面（注意此时是相对于 x）
                        return -1;
                    }
                }
            }

            // 如果权字典为空直接使用默认比较器
            if (weights == null)
            {
                return Default.Compare(x, y);
            }

            int xWeight;
            if (ReferenceEquals(y, null))
            {
                if (weights.TryGetValue(x, out xWeight))
                {
                    // x 有权值返回负 x 的权值（权值越高值越小越在最前）
                    return -xWeight;
                }
                else
                {
                    // x 没有权值使用默认比较器
                    return Default.Compare(x, y);
                }
            }
            else
            {
                int yWeight;
                weights.TryGetValue(x, out xWeight);
                weights.TryGetValue(y, out yWeight);

                if (xWeight == yWeight)
                {
                    // 权值相等再使用默认比较器比较
                    return Default.Compare(x, y);
                }

                // 返回权值差（因为权值与排序值相反所以使用 y 的权值减去 x 的权值）
                return yWeight - xWeight;
            }
        }
    }
}
