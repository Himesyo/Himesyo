using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Himesyo
{
    /// <summary>
    /// 表示一个自定义显示的项。它与 <see cref="Value"/> 的值相同。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ShowItem<TValue> : IShowItem
    {
        /// <summary>
        /// 创建指定类型的 <see cref="ShowItem{TValue}"/> 。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IShowItem CreateShowItem(Type type)
        {
            Type root = typeof(ShowItem<>).MakeGenericType(type);
            ConstructorInfo ctor = root.GetConstructor(Type.EmptyTypes);
            return (IShowItem)ctor.Invoke(null);
        }

        /// <summary>
        /// 要显示的对象。
        /// </summary>
        public object Show { get; set; }
        /// <summary>
        /// 实际的值。
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// 格式化字符串。
        /// </summary>
        public string FormatString { get; set; }
        /// <summary>
        /// 格式化器提供程序（包括区域性信息）。
        /// </summary>
        public IFormatProvider FormatProvider { get; set; }
        /// <summary>
        /// 获取或设置自定义获取字符串的函数。它拥有最高的优先级（仅针对 <see cref="ToString()"/>）。
        /// </summary>
        public Func<ShowItem<TValue>, string> Custom { get; set; }
        /// <summary>
        /// 补齐字符串长度。正值右对齐，负值左对齐。
        /// </summary>
        public int ShowLength { get; set; }

        /// <summary>
        /// 获取或设置用户关联的对象数据。
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// 初始化新实例。
        /// </summary>
        public ShowItem()
        {

        }
        /// <summary>
        /// 以指定值初始化新实例。
        /// </summary>
        /// <param name="value"></param>
        public ShowItem(TValue value)
        {
            Value = value;
        }
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
            if (Custom != null)
            {
                return Custom(this);
            }
            else
            {
                object formatObject = Show ?? Value;
                if (FormatProvider != null)
                {
                    ICustomFormatter formatter = (ICustomFormatter)FormatProvider.GetFormat(typeof(ICustomFormatter));
                    if (formatter != null)
                    {
                        return formatter.Format(FormatString, formatObject, FormatProvider);
                    }
                }

                if (formatObject is IFormattable formattable)
                {
                    return formattable.ToString(FormatString, FormatProvider);
                }

                if (formatObject == null)
                {
                    if (ShowLength == 0)
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return new string(' ', Math.Abs(ShowLength));
                    }
                }
                else
                {
                    if (ShowLength == 0)
                    {
                        return formatObject.ToString();
                    }
                    string str = formatObject.ToString() ?? string.Empty;
                    if (ShowLength > 0)
                    {
                        return str.PadLeft(ShowLength);
                    }
                    else
                    {
                        return str.PadRight(-ShowLength);
                    }
                }
            }
        }

        /// <summary>
        /// 指定对象是否与当前对象相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return Value?.Equals(null) ?? true;
            if (obj is IShowItem)
            {
                IShowItem data = obj as IShowItem;
                while (data.Value is IShowItem)
                {
                    data = data.Value as IShowItem;
                }
                if (ReferenceEquals(Value, null) && ReferenceEquals(data.Value, null))
                    return true;
                if (ReferenceEquals(Value, null) || ReferenceEquals(data.Value, null))
                    return false;
                return Value.Equals(data.Value);
            }
            else
            {
                return Value?.Equals(obj) ?? false;
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
            if (ReferenceEquals(left, null))
            {
                return right?.Equals(null) ?? true;
            }
            else
            {
                return left.Equals(right);
            }
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
        /// 从 <typeparamref name="TValue"/> 隐式转换。
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ShowItem<TValue>(TValue value)
        {
            return new ShowItem<TValue>(value, null);
        }
        /// <summary>
        /// 显示转换到 <typeparamref name="TValue"/> 。
        /// </summary>
        /// <param name="item"></param>
        public static explicit operator TValue(ShowItem<TValue> item)
        {
            return item.Value;
        }

        #region IShowItem 接口成员

        Type IShowItem.ValueType => typeof(TValue);
        object IShowItem.Value
        {
            get => Value;
            set => Value = (TValue)value;
        }
        Delegate IShowItem.Custom
        {
            get => Custom;
            set => Custom = (Func<ShowItem<TValue>, string>)value;
        }

        #endregion
    }

    /// <summary>
    /// 表示一个显示项。
    /// </summary>
    public interface IShowItem
    {
        /// <summary>
        /// <see cref="Value"/> 的限制类型。
        /// </summary>
        Type ValueType { get; }
        /// <summary>
        /// 要显示的对象。
        /// </summary>
        object Show { get; set; }
        /// <summary>
        /// 实际的值。
        /// </summary>
        object Value { get; set; }
        /// <summary>
        /// 格式化字符串。
        /// </summary>
        string FormatString { get; set; }
        /// <summary>
        /// 格式化器提供程序（包括区域性信息）。
        /// </summary>
        IFormatProvider FormatProvider { get; set; }
        /// <summary>
        /// 获取或设置自定义获取字符串的函数。它拥有最高的优先级（仅针对 <see cref="ToString()"/>）。
        /// </summary>
        Delegate Custom { get; set; }
        /// <summary>
        /// 补齐字符串长度。正值右对齐，负值左对齐。
        /// </summary>
        int ShowLength { get; set; }
        /// <summary>
        /// 获取或设置用户关联的对象数据。
        /// </summary>
        object Tag { get; set; }
    }
}
