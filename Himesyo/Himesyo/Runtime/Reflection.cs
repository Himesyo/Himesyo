using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 提供与反射有关的方法和扩展。
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// 获取公共无参数构造函数。
        /// </summary>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            return constructor;
        }

        /// <summary>
        /// 此类型是否重写了 <see cref="object.ToString"/> 方法。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsOverrideToString(this Type type)
        {
            MethodInfo methodInfo = type.GetMethod("ToString", Type.EmptyTypes);
            return methodInfo.DeclaringType != typeof(object);
        }

        /// <summary>
        /// 通过预定义类型获取成员列表。部分预定义类型可能具有自动修正效果。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberDefinitionType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDescriptions(this Type type, MemberDefinitionType memberDefinitionType, Type valueType = null)
        {
            switch (memberDefinitionType)
            {
                case MemberDefinitionType.Enum:
                    return type.GetDescriptionsOfFields(type, BindingFlags.Public | BindingFlags.Static);
                case MemberDefinitionType.Constant:
                    return type.GetDescriptionsOfFields(valueType, BindingFlags.Public | BindingFlags.Static);
                case MemberDefinitionType.StateProperty:
                    return type.GetDescriptionsOfProperties(valueType, BindingFlags.Public | BindingFlags.Static);
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberDefinitionType));
            }
        }

        /// <summary>
        /// 通过预定义类型获取成员列表。部分预定义类型可能具有自动修正效果。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="memberDefinitionType"></param>
        /// <param name="valueType"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDisplayNames(this Type type, MemberDefinitionType memberDefinitionType, Type valueType = null)
        {
            switch (memberDefinitionType)
            {
                case MemberDefinitionType.Enum:
                    return type.GetDescriptionsOfFields(type, BindingFlags.Public | BindingFlags.Static);
                case MemberDefinitionType.Constant:
                    return type.GetDisplayNamesOfFields(valueType, BindingFlags.Public | BindingFlags.Static);
                case MemberDefinitionType.StateProperty:
                    return type.GetDisplayNamesOfProperties(valueType, BindingFlags.Public | BindingFlags.Static);
                default:
                    throw new ArgumentOutOfRangeException(nameof(memberDefinitionType));
            }
        }

        /// <summary>
        /// 获取类型字段及其 <see cref="DescriptionAttribute"/> 特性的信息。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueType">值的类型，如果为 null 表示获取所有类型。</param>
        /// <param name="flags"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDescriptionsOfFields(this Type type, Type valueType, BindingFlags flags, object instance = null)
        {
            var fields = type.GetFields(flags);
            List<IShowItem> result = new List<IShowItem>(fields.Length);
            Type root = typeof(ShowItem<>).MakeGenericType(type);
            ConstructorInfo ctor = root.GetConstructor(Type.EmptyTypes);
            foreach (var field in fields)
            {
                if (valueType == null || valueType.IsAssignableFrom(field.FieldType))
                {
                    IShowItem item = (IShowItem)ctor.Invoke(null);
                    item.Value = field.GetValue(instance);
                    item.Show = field.GetDescription();
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取类型字段及其 <see cref="DisplayNameAttribute"/> 特性的信息。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueType">值的类型，如果为 null 表示获取所有类型。</param>
        /// <param name="flags"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDisplayNamesOfFields(this Type type, Type valueType, BindingFlags flags, object instance = null)
        {
            var fields = type.GetFields(flags);
            List<IShowItem> result = new List<IShowItem>(fields.Length);
            Type root = typeof(ShowItem<>).MakeGenericType(type);
            ConstructorInfo ctor = root.GetConstructor(Type.EmptyTypes);
            foreach (var field in fields)
            {
                if (valueType == null || valueType.IsAssignableFrom(field.FieldType))
                {
                    IShowItem item = (IShowItem)ctor.Invoke(null);
                    item.Value = field.GetValue(instance);
                    item.Show = field.GetDisplayName();
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取类型属性及其 <see cref="DescriptionAttribute"/> 特性的信息。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueType">值的类型，如果为 null 表示获取所有类型。</param>
        /// <param name="flags"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDescriptionsOfProperties(this Type type, Type valueType, BindingFlags flags, object instance = null)
        {
            var properties = type.GetProperties(flags);
            List<IShowItem> result = new List<IShowItem>(properties.Length);
            Type root = typeof(ShowItem<>).MakeGenericType(type);
            ConstructorInfo ctor = root.GetConstructor(Type.EmptyTypes);
            foreach (var property in properties)
            {
                if (property.GetIndexParameters().Length == 0
                    && (valueType == null || valueType.IsAssignableFrom(property.PropertyType)))
                {
                    IShowItem item = (IShowItem)ctor.Invoke(null);
                    item.Value = property.GetValue(instance, null);
                    item.Show = property.GetDescription();
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取类型属性及其 <see cref="DisplayNameAttribute"/> 特性的信息。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="valueType">值的类型，如果为 null 表示获取所有类型。</param>
        /// <param name="flags"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static List<IShowItem> GetDisplayNamesOfProperties(this Type type, Type valueType, BindingFlags flags, object instance = null)
        {
            var properties = type.GetProperties(flags);
            List<IShowItem> result = new List<IShowItem>(properties.Length);
            Type root = typeof(ShowItem<>).MakeGenericType(type);
            ConstructorInfo ctor = root.GetConstructor(Type.EmptyTypes);
            foreach (var property in properties)
            {
                if (property.GetIndexParameters().Length == 0
                    && (valueType == null || valueType.IsAssignableFrom(property.PropertyType)))
                {
                    IShowItem item = (IShowItem)ctor.Invoke(null);
                    item.Value = property.GetValue(instance, null);
                    item.Show = property.GetDisplayName();
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定类型所有公共静态字段及其 <see cref="DescriptionAttribute"/> 特性的信息。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        [Obsolete("请改用 GetDescriptionsOfFields 或 GetDescriptionsOfProperties。")]
        public static List<ShowItem<TValue>> GetDescriptions<TValue>()
        {
            Type type = typeof(TValue);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            List<ShowItem<TValue>> result = new List<ShowItem<TValue>>(fields.Length);
            foreach (var field in fields)
            {
                if (type.IsAssignableFrom(field.FieldType))
                {
                    ShowItem<TValue> item = new ShowItem<TValue>((TValue)field.GetValue(null), field.GetDescription());
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取指定类型所有公共静态字段及其 <see cref="DisplayNameAttribute"/> 特性的信息。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        [Obsolete("请改用 GetDisplayNamesOfFields 或 GetDisplayNamesOfProperties。")]
        public static List<ShowItem<TValue>> GetDisplayNames<TValue>()
        {
            Type type = typeof(TValue);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            List<ShowItem<TValue>> result = new List<ShowItem<TValue>>(fields.Length);
            foreach (var field in fields)
            {
                if (type.IsAssignableFrom(field.FieldType))
                {
                    ShowItem<TValue> item = new ShowItem<TValue>((TValue)field.GetValue(null), field.GetDisplayName());
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 创建对指定泛型类型或泛型方法的引用。
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="typeGenericTypes"></param>
        /// <param name="methodName"></param>
        /// <param name="methodGenericTypes"></param>
        /// <param name="methodArgumentTypes"></param>
        /// <returns></returns>
        public static MethodInfo CreateMethod(Type baseType, Type[] typeGenericTypes, string methodName, Type[] methodGenericTypes, Type[] methodArgumentTypes)
        {
            if (baseType.IsGenericType)
            {
                baseType = baseType.GetGenericTypeDefinition().MakeGenericType(typeGenericTypes);
            }
            MethodInfo info = baseType.GetMethod(methodName, methodArgumentTypes);
            if (info != null && info.IsGenericMethod)
            {
                info = info.MakeGenericMethod(methodGenericTypes);
            }
            return info;
        }

    }

    /// <summary>
    /// 预定义的成员类型。
    /// </summary>
    public enum MemberDefinitionType
    {
        /// <summary>
        /// 枚举类型值。
        /// </summary>
        Enum,
        /// <summary>
        /// 常量值。
        /// </summary>
        Constant,
        /// <summary>
        /// 静态属性值。
        /// </summary>
        StateProperty,
    }
}
