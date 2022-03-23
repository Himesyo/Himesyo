using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
                case MemberDefinitionType.StaticProperty:
                    return type.GetDescriptionsOfProperties(valueType, BindingFlags.Public | BindingFlags.Static);
                default:
                    throw new InvalidEnumArgumentException(nameof(memberDefinitionType), (int)memberDefinitionType, typeof(MemberDefinitionType));
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
                case MemberDefinitionType.StaticProperty:
                    return type.GetDisplayNamesOfProperties(valueType, BindingFlags.Public | BindingFlags.Static);
                default:
                    throw new InvalidEnumArgumentException(nameof(memberDefinitionType), (int)memberDefinitionType, typeof(MemberDefinitionType));
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
            Type root = typeof(ShowItem<>).MakeGenericType(valueType ?? typeof(object));
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
            Type root = typeof(ShowItem<>).MakeGenericType(valueType ?? typeof(object));
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
            Type root = typeof(ShowItem<>).MakeGenericType(valueType ?? typeof(object));
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
            Type root = typeof(ShowItem<>).MakeGenericType(valueType ?? typeof(object));
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

        /// <summary>
        /// 生成一个委托，此委托执行指定枚举类型的按位 <see langword="OR"/> 运算。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Func<TEnum, TEnum, TEnum> EnumOr<TEnum>() 
            where TEnum : Enum, new()
        {
            Type type = typeof(TEnum);
            ParameterExpression argLeft = Expression.Parameter(type);
            ParameterExpression argRight = Expression.Parameter(type);
            Expression valueLeft = Expression.Convert(argLeft, type.GetEnumUnderlyingType());
            Expression valueRight = Expression.Convert(argRight, type.GetEnumUnderlyingType());
            Expression valueResult = Expression.Or(valueLeft, valueRight);
            Expression resultExpression = Expression.Convert(valueResult, type);
            LambdaExpression lambdaExpression = Expression.Lambda(resultExpression, argLeft, argRight);
            Func<TEnum, TEnum, TEnum> result = (Func<TEnum, TEnum, TEnum>)lambdaExpression.Compile();
            return result;
        }

        /// <summary>
        /// 生成一个委托，此委托执行指定枚举类型的按位 <see langword="AND"/> 运算。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Func<TEnum, TEnum, TEnum> EnumAnd<TEnum>()
            where TEnum : Enum, new()
        {
            Type type = typeof(TEnum);
            ParameterExpression argLeft = Expression.Parameter(type);
            ParameterExpression argRight = Expression.Parameter(type);
            Expression valueLeft = Expression.Convert(argLeft, type.GetEnumUnderlyingType());
            Expression valueRight = Expression.Convert(argRight, type.GetEnumUnderlyingType());
            Expression valueResult = Expression.And(valueLeft, valueRight);
            Expression resultExpression = Expression.Convert(valueResult, type);
            LambdaExpression lambdaExpression = Expression.Lambda(resultExpression, argLeft, argRight);
            Func<TEnum, TEnum, TEnum> result = (Func<TEnum, TEnum, TEnum>)lambdaExpression.Compile();
            return result;
        }

        /// <summary>
        /// 生成一个委托，此委托执行指定枚举类型的按位 <see langword="XOR"/> 运算。
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static Func<TEnum, TEnum, TEnum> EnumExclusiveOr<TEnum>()
            where TEnum : Enum, new()
        {
            Type type = typeof(TEnum);
            ParameterExpression argLeft = Expression.Parameter(type);
            ParameterExpression argRight = Expression.Parameter(type);
            Expression valueLeft = Expression.Convert(argLeft, type.GetEnumUnderlyingType());
            Expression valueRight = Expression.Convert(argRight, type.GetEnumUnderlyingType());
            Expression valueResult = Expression.ExclusiveOr(valueLeft, valueRight);
            Expression resultExpression = Expression.Convert(valueResult, type);
            LambdaExpression lambdaExpression = Expression.Lambda(resultExpression, argLeft, argRight);
            Func<TEnum, TEnum, TEnum> result = (Func<TEnum, TEnum, TEnum>)lambdaExpression.Compile();
            return result;
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
        StaticProperty,
    }
}
