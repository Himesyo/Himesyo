using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himesyo.Data
{
    public partial class ConnectionResult
    {
        /// <summary>
        /// <see cref="OpenNewConnection"/> 的异步版本。使用当前 <see cref="ConnectionStringBuilder"/> 打开新连接。
        /// </summary>
        /// <returns></returns>
        public async Task<DbConnection> OpenNewConnectionAsync()
        {
            DbConnection connection = CreateNewConnection();
            try
            {
                await connection.OpenAsync();
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
