#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Logger
{
    /// <summary>
    /// 日志等级。
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 全等级。记录全部日志。
        /// </summary>
        All = 0,
        /// <summary>
        /// 诊断等级。
        /// </summary>
        Diag = 0x000001,
        /// <summary>
        /// 调试等级。 
        /// </summary>
        Debug = 0x000010,
        /// <summary>
        /// 运行信息等级。
        /// </summary>
        Info = 0x000100,
        /// <summary>
        /// 警告等级。
        /// </summary>
        Warn = 0x001000,
        /// <summary>
        /// 错误等级。
        /// </summary>
        Error = 0x010000,
        /// <summary>
        /// 致命等级。
        /// </summary>
        Fatal = 0x100000,
        /// <summary>
        /// 无等级。表示不记录日志。
        /// </summary>
        None = 0xFFFFFFF
    }
}
