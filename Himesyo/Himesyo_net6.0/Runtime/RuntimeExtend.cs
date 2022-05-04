using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Himesyo.Runtime.Extend
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class RuntimeExtend
    {
        /// <summary>
        /// 显示方法全名
        /// </summary>
        /// <param name="method"></param>
        /// <param name="includeParameters">是否包含参数信息</param>
        /// <param name="prefix">输出前缀</param>
        /// <returns></returns>
        public static string Show(this MethodBase method, bool includeParameters = true, string prefix = null)
        {
            string paras = string.Empty;
            if (includeParameters)
            {
                string generic = method.IsGenericMethod ? $"<{string.Join(", ", method.GetGenericArguments().Select(t => t.Name))}>" : string.Empty;
                paras = $"{generic}({string.Join(", ", method.GetParameters().Select(info => info.ParameterType.Name))})";
            }
            Type reflectedType = method.ReflectedType;
            string result = $"{prefix}{reflectedType.FullName}.{method.Name}{paras}";
            return result;
        }

        /// <summary>
        /// 显示类型名称。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Show(this Type type)
        {
            if (type == null)
                return string.Empty;

            if (type.IsGenericType)
            {
                Type def = type.GetGenericTypeDefinition();
                if (def == typeof(Nullable<>))
                {
                    return $"{type.GetGenericArguments().First().Show()}?";
                }
                var arguments = type.GetGenericArguments();
                string argShow = string.Join(", ", arguments.Select(t => t.Show()));
                if (arguments.Length > 1 && def.FullName!.StartsWith("System.ValueTuple`"))
                {
                    return $"({argShow})";
                }
                string name = Regex.Replace(type.Name, @"`.+$", "");
                return $"{name}<{argShow}>";
            }
            else
            {
                return type.Name;
            }
        }

    }
}
