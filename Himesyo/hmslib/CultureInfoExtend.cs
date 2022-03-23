using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Himesyo
{
    /// <summary>
    /// 对区域性设置提供扩展。
    /// </summary>
    public class CultureInfoExtend 
    {
        /// <summary>
        /// 当前库统一设置。
        /// </summary>
        public static CultureInfo Unified { get; }

        static CultureInfoExtend()
        {
            CultureInfo unified = new CultureInfo("zh-cn");
            unified.DateTimeFormat.DateSeparator = "-";
            Unified = unified;
        }
    }
}
