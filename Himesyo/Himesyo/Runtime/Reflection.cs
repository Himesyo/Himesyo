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
        /// 获取指定类型所有公共静态字段及其 <see cref="DescriptionAttribute"/> 特性的信息。
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
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
}
