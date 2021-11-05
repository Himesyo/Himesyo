using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Himesyo.Data
{
    /// <summary>
    /// 为数据库相关提供常用扩展
    /// </summary>
    public static class DatabaseExtend
    {
        /// <summary>
        /// 创建数据库相关类型。
        /// </summary>
        /// <param name="relevantType"></param>
        /// <param name="createType"></param>
        /// <returns></returns>
        public static object Create(Type relevantType, string createType)
        {
            string name;
            if (typeof(DbConnectionStringBuilder).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("ConnectionStringBuilder", "");
            }
            else if (typeof(DbConnection).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("Connection", "");
            }
            else if (typeof(DbCommand).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("Command", "");
            }
            else if (typeof(DbDataAdapter).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("DataAdapter", "");
            }
            else if (typeof(DbDataReader).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("DataReader", "");
            }
            else if (typeof(DbParameter).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("Parameter", "");
            }
            else if (typeof(DbTransaction).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("Transaction", "");
            }
            else if (typeof(DbDataRecord).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("DataRecord", "");
            }
            else if (typeof(DbCommandBuilder).IsAssignableFrom(relevantType))
            {
                name = relevantType.Name.Replace("CommandBuilder", "");
            }
            else
            {
                throw new ArgumentException($"无法从参数推断出数据库类型。关联类型：{relevantType.FullName}, 创建类型：{createType}。");
            }
            string createFullName = $"{relevantType.Namespace}.{name}{createType}";

            object obj = relevantType.Assembly.CreateInstance(createFullName);
            return obj;
        }

        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbConnection relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbConnectionStringBuilder relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbCommand relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbCommandBuilder relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbDataAdapter relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbDataReader relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbParameter relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbTransaction relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }
        /// <summary>
        /// 创建指定类型的数据库对象。<typeparamref name="TDatabase"/> 应是数据库基类型之一。
        /// </summary>
        /// <typeparam name="TDatabase"></typeparam>
        /// <param name="relevantObj"></param>
        /// <returns></returns>
        public static TDatabase Create<TDatabase>(this DbDataRecord relevantObj)
            where TDatabase : class
        {
            string name = typeof(TDatabase).Name;
            if (name.StartsWith("Db"))
            {
                return (TDatabase)Create(relevantObj.GetType(), name.Substring(2));
            }
            else
            {
                return default;
            }
        }

        /// <summary>
        /// 是否存在指定列。
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool ExistColumn(this DbDataReader dataReader, string columnName)
        {
            int index = dataReader.GetOrdinal(columnName);
            return index > -1;
        }
        /// <summary>
        /// 获取指定列的值。
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static TValue GetValue<TValue>(this DbDataReader dataReader, string columnName)
        {
            if (dataReader.ExistColumn(columnName))
            {
                object value = dataReader[columnName];
                if (value == null || value == DBNull.Value)
                {
                    return default;
                }
                return (TValue)value;
            }
            return default;
        }
    }
}
