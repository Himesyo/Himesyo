using System.Data;
using System.Data.Common;

using Himesyo.Data;

namespace Himesyo.Data
{
    /// <summary>
    /// 连接结果类型。
    /// </summary>
    public partial class ConnectionResult
    {
        /// <summary>
        /// 连接字符串拼接对象。
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder { get; set; }

        /// <summary>
        /// 数据库连接。
        /// </summary>
        public DbConnection Connection { get; set; }

        /// <summary>
        /// 当前连接是否在打开状态。
        /// </summary>
        public bool IsOpen => Connection?.State == ConnectionState.Open;

        /// <summary>
        /// 使用当前 <see cref="ConnectionStringBuilder"/> 创建新连接。
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateNewConnection()
        {
            ExceptionHelper.ThrowInvalid(ConnectionStringBuilder == null, "连接字符串对象为 null 。");
            DbConnection connection = ConnectionStringBuilder.Create<DbConnection>();
            connection.ConnectionString = ConnectionStringBuilder.ConnectionString;
            return connection;
        }

        /// <summary>
        /// 使用当前 <see cref="ConnectionStringBuilder"/> 打开新连接。
        /// </summary>
        /// <returns></returns>
        public DbConnection OpenNewConnection()
        {
            DbConnection connection = CreateNewConnection();
            try
            {
                connection.Open();
            }
            catch
            {
                connection.Dispose();
                throw;
            }
            return connection;
        }
    }
}
