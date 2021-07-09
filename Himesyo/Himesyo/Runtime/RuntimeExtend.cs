using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
        /// 是否是系统方法。
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool IsSystem(this MethodBase method)
        {
            string fullName = method.ReflectedType.FullName;
            return fullName.StartsWith("System.") || fullName.StartsWith("Microsoft.");
        }
    }
}
