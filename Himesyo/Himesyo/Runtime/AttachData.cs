using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himesyo.Runtime
{
    /// <summary>
    /// 将指定引用类型数据动态的附加到托管对象。
    /// </summary>
    public static class AttachData
    {
        private static readonly DataManager tables = new DataManager();

        /// <summary>
        /// 为指定对象附加数据，此附加数据可随时使用 <see cref="GetData{T}(object)"/> 获取。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="data"></param>
        [Obsolete]
        public static void SetData<T>(this object obj, T data) where T : class
        {
            if (obj == null)
                return;
            var tableManager = tables.GetTable<T>();
            tableManager.Table.Add(obj, data);
        }

        /// <summary>
        /// 获取使用 <see cref="SetData{T}(object, T)"/> 设置的数据。
        /// 如果未设置数据，则自动调用 <typeparamref name="T"/> 的默认构造函数创建对象。
        /// 如果 <typeparamref name="T"/> 没有构造函数，请使用 <see cref="SetCreate{T}(Func{object, T})"/> 附加创建回调。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete]
        public static T GetData<T>(this object obj) where T : class
        {
            if (obj == null)
                return null;
            var tableManager = tables.GetTable<T>();
            if (tableManager.UseCreate)
            {
                return tableManager.Table.GetValue(obj, tableManager.InvokeCreate);
            }
            else
            {
                return tableManager.Table.GetOrCreateValue(obj);
            }
        }

        /// <summary>
        /// 为 <typeparamref name="T"/> 类型附加创建回调。当获取未设置数据的对象的数据时，使用此回调创建默认数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="create"></param>
        [Obsolete]
        public static void SetCreate<T>(Func<object, T> create) where T : class
        {
            var tableManager = tables.GetTable<T>();
            tableManager.UseCreate = create != null;
            tableManager.Create = create;
        }
    }

    internal class DataManager
    {
        private readonly Dictionary<Type, object> tables = new Dictionary<Type, object>();

        public TableManager<T> GetTable<T>() where T : class
        {
            Type type = typeof(T);
            if (tables.TryGetValue(type, out object value))
            {
                return (TableManager<T>)value;
            }
            else
            {
                var newTableManager = new TableManager<T>();
                tables[type] = newTableManager;
                return newTableManager;
            }
        }
    }

    internal class TableManager<T> where T : class
    {
        public bool UseCreate { get; set; }
        public Func<object, T> Create { get; set; }
        public ConditionalWeakTable<object, T> Table { get; } = new ConditionalWeakTable<object, T>();

        public T InvokeCreate(object obj)
        {
            if (Create == null)
            {
                Type type = typeof(T);
                return (T)type.Assembly.CreateInstance(type.FullName);
            }
            else
            {
                return Create?.Invoke(obj);
            }
        }
    }
}
