using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 为 <see cref="StandardValuesAttribute"/> 特性提供转换器。
    /// </summary>
    public sealed class StandardValuesConverter : TypeConverter
    {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string key)
            {
                StandardValuesAttribute attribute = context.PropertyDescriptor.Attributes[typeof(StandardValuesAttribute)] as StandardValuesAttribute;
                for (int i = 0; i < attribute.Keys.Length && i < attribute.Values.Length; i++)
                {
                    if (attribute.Keys[i] == key)
                    {
                        return attribute.Values[i];
                    }
                }
                Type String = typeof(string);
                if (context.PropertyDescriptor.PropertyType == String)
                {
                    return value;
                }
                MethodInfo method = context.PropertyDescriptor.PropertyType.GetMethod("Parse", new[] { String });
                if (method == null)
                {
                    throw new Exception("无法转换的类型。绑定字段的属性的类型必须是 string 或实现 Parse 方法的类型。");
                }
                else
                {
                    object result = method.Invoke(null, new[] { value });
                    return result;
                }
            }
            throw new InvalidCastException();
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                if (context != null)
                {
                    //Type String = typeof(string);
                    //if (value != null && value is string && context.PropertyDescriptor.PropertyType != String)
                    //{
                    //    MethodInfo method = context.PropertyDescriptor.PropertyType.GetMethod("Parse", new[] { String });
                    //    if (method == null)
                    //    {
                    //        throw new Exception("无法转换的类型。绑定字段的属性的类型必须是 string 或实现 Parse 方法的类型。");
                    //    }
                    //    else
                    //    {
                    //        value = method.Invoke(null, new[] { value });
                    //    }
                    //}
                    StandardValuesAttribute attribute = context.PropertyDescriptor.Attributes[typeof(StandardValuesAttribute)] as StandardValuesAttribute;
                    for (int i = 0; i < attribute.Keys.Length && i < attribute.Values.Length; i++)
                    {
                        if (attribute.Values[i].GetHashCode() == value.GetHashCode()
                            || attribute.Values[i].ToString() == value.ToString())
                        {
                            return attribute.Keys[i];
                        }
                    }
                    if (value == null)
                    {
                        return value;
                    }
                    return value.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return value;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            StandardValuesAttribute attribute = context.PropertyDescriptor.Attributes[typeof(StandardValuesAttribute)] as StandardValuesAttribute;
            return new StandardValuesCollection(attribute.Values);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            StandardValuesAttribute attribute = context.PropertyDescriptor.Attributes[typeof(StandardValuesAttribute)] as StandardValuesAttribute;
            return attribute.StandardValuesExclusive;
        }
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}
