using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Himesyo
{
    /// <summary>
    /// 表示一个自定义显示的项。它与 <see cref="Value"/> 的值相同。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ShowItem<TValue>
    {
        /// <summary>
        /// 自定义获取字符串的函数。默认为 <see cref="Show"/> 。
        /// </summary>
        public Func<ShowItem<TValue>, string> GetStringFunc { get; set; }
        /// <summary>
        /// 要显示的对象。
        /// </summary>
        public object Show { get; set; }
        /// <summary>
        /// 实际的值。
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// 以指定值与显示对象初始化新实例。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="show"></param>
        public ShowItem(TValue value, object show)
        {
            Value = value;
            Show = show;
        }

        /// <summary>
        /// 获取字符串显示。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (GetStringFunc != null)
            {
                return GetStringFunc(this);
            }
            else
            {
                if (Show != null)
                {
                    return $"{Show}";
                }
                else
                {
                    return Value?.ToString() ?? "";
                }
                //if (Show != null)
                //{
                //    return $"[{Value,6}] {Show}";
                //}
                //else
                //{
                //    return Value?.ToString() ?? "";
                //}
            }
        }

        /// <summary>
        /// 指定对象是否与当前对象相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return Value == null;
            if (obj is ShowItem<TValue>)
            {
                ShowItem<TValue> data = obj as ShowItem<TValue>;
                if (ReferenceEquals(Value, null) && ReferenceEquals(data.Value, null)) return true;
                if (ReferenceEquals(Value, null) || ReferenceEquals(data.Value, null)) return false;
                return Value.Equals(data.Value);
            }
            else
            {
                if (Value == null) return false;
                return Value.Equals(obj);
            }
        }
        /// <summary>
        /// 获取哈希代码。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (ReferenceEquals(Value, null))
            {
                return 0;
            }
            else
            {
                return Value.GetHashCode();
            }
        }
        /// <summary>
        /// 相等比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ShowItem<TValue> left, ShowItem<TValue> right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null)) return true;
            if (ReferenceEquals(left, null)) return ReferenceEquals(right.Value, null);
            if (ReferenceEquals(right, null)) return ReferenceEquals(left.Value, null);
            if (ReferenceEquals(left.Value, null) && ReferenceEquals(right.Value, null)) return true;
            if (ReferenceEquals(left.Value, null) || ReferenceEquals(right.Value, null)) return false;
            return left.Value.Equals(right.Value);
        }
        /// <summary>
        /// 不等比较。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ShowItem<TValue> left, ShowItem<TValue> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 隐式转换到 <typeparamref name="TValue"/> 。
        /// </summary>
        /// <param name="item"></param>
        public static implicit operator TValue(ShowItem<TValue> item)
        {
            return item.Value;
        }
        /// <summary>
        /// 从 <typeparamref name="TValue"/> 隐式转换。
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ShowItem<TValue>(TValue value)
        {
            return new ShowItem<TValue>(value, value?.ToString());
        }
    }
}
