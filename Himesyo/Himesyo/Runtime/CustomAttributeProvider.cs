using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 为 <see cref="ICustomAttributeProvider"/> 提供扩展。
    /// </summary>
    public static class CustomAttributeProvider
    {
        /// <summary>
        /// 获取特性类型的 <see cref="AttributeUsageAttribute"/> .
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static AttributeUsageAttribute GetUsage<T>()
            where T : Attribute
        {
            Type type = typeof(T);
            AttributeUsageAttribute usage = type.GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                                                .Cast<AttributeUsageAttribute>()
                                                .FirstOrDefault();
            return usage ?? new AttributeUsageAttribute(AttributeTargets.All);
        }
        /// <summary>
        /// 获取特性类型的 <see cref="AttributeUsageAttribute"/> .
        /// </summary>
        /// <param name="type">要获取 <see cref="AttributeUsageAttribute"/> 特性的特性类型。</param>
        /// <returns></returns>
        public static AttributeUsageAttribute GetUsage(this Type type)
        {
            if (typeof(Attribute).IsAssignableFrom(type))
                throw new ArgumentException("参数不是有效的特性类型。", nameof(type));
            AttributeUsageAttribute usage = type.GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                                                .Cast<AttributeUsageAttribute>()
                                                .FirstOrDefault();
            return usage ?? new AttributeUsageAttribute(AttributeTargets.All);
        }
        /// <summary>
        /// 获取特性类型的 <see cref="AttributeUsageAttribute"/> .
        /// </summary>
        /// <param name="attribute">要获取 <see cref="AttributeUsageAttribute"/> 特性的特性。</param>
        /// <returns></returns>
        public static AttributeUsageAttribute GetUsage(this Attribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));
            Type type = attribute.GetType();
            AttributeUsageAttribute usage = type.GetCustomAttributes(typeof(AttributeUsageAttribute), true)
                                                .Cast<AttributeUsageAttribute>()
                                                .FirstOrDefault();
            return usage ?? new AttributeUsageAttribute(AttributeTargets.All);
        }

        /// <summary>
        /// 获取单个特性。
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider provider) where TAttribute : Attribute
        {
            return provider?.GetCustomAttributes(typeof(TAttribute), true).Cast<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// 获取对象 <see cref="DisplayNameAttribute"/> 特性的值。
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string GetDisplayName(this ICustomAttributeProvider provider)
        {
            return provider.GetAttribute<DisplayNameAttribute>()?.DisplayName;
        }
        /// <summary>
        /// 获取对象 <see cref="CategoryAttribute"/> 特性的值。
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string GetCategory(this ICustomAttributeProvider provider)
        {
            return provider.GetAttribute<CategoryAttribute>()?.Category;
        }
        /// <summary>
        /// 获取对象 <see cref="DescriptionAttribute"/> 特性的值。
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string GetDescription(this ICustomAttributeProvider provider)
        {
            return provider.GetAttribute<DescriptionAttribute>()?.Description;
        }
        /// <summary>
        /// 获取对象 <see cref="GuidAttribute"/> 特性的值。
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static string GetGuid(this ICustomAttributeProvider provider)
        {
            return provider.GetAttribute<GuidAttribute>()?.Value;
        }

    }
}
