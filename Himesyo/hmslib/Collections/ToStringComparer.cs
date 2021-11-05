/*************************************************
 * 描述：
 * 
 * Author：Himesyo<183147241@qq.com>
 * Date：2021/5/15 16:11:00
 * Create：将值转化为 string 类型再进行比较的比较器。
 * ************************************************/

using System;
using System.Collections.Generic;

namespace Himesyo.Collections
{
    /// <summary>
    /// 将值转化为 <see langword="string"/> 类型再进行比较的比较器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ToStringComparer<T> : EqualityComparer<T>
    {
        /// <summary>
        /// 转换为字符串后比较使用的比较器。如果为 <see langword="null"/> ，使用默认比较器。
        /// </summary>
        public IEqualityComparer<string> StringComparer { get; set; }

        /// <summary>
        /// 字符串值的获取器。如果为 <see langword="null"/> ，使用 <see cref="object.ToString"/> 获取。
        /// </summary>
        public Func<T, string> StringValueGetter { get; set; }

        /// <summary>
        /// 使用指定的值获取器和比较器判断是否相等。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, null) && ReferenceEquals(y, null))
                return true;

            IEqualityComparer<string> stringComparer = StringComparer ?? EqualityComparer<string>.Default;

            string xValue;
            string yValue;
            if (StringValueGetter == null)
            {
                xValue = x?.ToString();
                yValue = y?.ToString();
            }
            else
            {
                xValue = StringValueGetter(x);
                yValue = StringValueGetter(y);
            }

            return stringComparer.Equals(xValue, yValue);
        }
        /// <summary>
        /// 使用指定的值获取器获取字符串的哈希代码。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override int GetHashCode(T obj)
        {
            string value;
            if (StringValueGetter == null)
            {
                value = obj?.ToString();
            }
            else
            {
                value = StringValueGetter(obj);
            }
            IEqualityComparer<string> stringComparer = StringComparer ?? EqualityComparer<string>.Default;
            return stringComparer.GetHashCode(value);
        }
    }
}
