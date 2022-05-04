using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Himesyo.IO;

namespace IAService
{
    public class AppConfig : BaseConfig
    {
        /// <summary>
        /// 监听端口号。
        /// </summary>
        [DefaultValue(1801)]
        public int ListenPort { get; set; } = 1801;
    }
}
