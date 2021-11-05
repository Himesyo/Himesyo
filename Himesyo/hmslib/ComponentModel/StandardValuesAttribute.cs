using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 为指定属性设置值列表。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]

    public sealed class StandardValuesAttribute : Attribute
    {
        /// <summary>
        /// 独占下拉列表（只能下拉选择不能自己输入）
        /// </summary>
        public bool StandardValuesExclusive { get; set; }
        /// <summary>
        /// 显示的值列表。实际列表数量为两个数组长度的最小值。
        /// </summary>
        public string[] Keys { get; set; }
        /// <summary>
        /// 实际的值列表。实际列表数量为两个数组长度的最小值。
        /// </summary>
        public object[] Values { get; set; }

        /// <summary>
        /// 列表为空或自己设置值列表。
        /// </summary>
        public StandardValuesAttribute()
        {
            Keys = new string[0];
            Values = new object[0];
        }
        /// <summary>
        /// 设置值列表。
        /// </summary>
        /// <param name="keys">显示的值。</param>
        /// <param name="values">实际值。</param>
        public StandardValuesAttribute(string[] keys, object[] values)
        {
            if (keys == null)
            {
                keys = new string[0];
            }
            if (values == null)
            {
                values = new object[0];
            }

            Keys = keys;
            Values = values;
        }

    }
}
