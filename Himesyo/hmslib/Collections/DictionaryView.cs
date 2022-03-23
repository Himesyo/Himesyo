using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Collections
{
    /// <summary>
    /// 提供对 <see cref="DictionaryCollection{TValue}"/> 的显示。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryView<T> : IBindingList
    {
        private static readonly IEnumerator Empty = new object[0].GetEnumerator();

        private DictionaryCollection<T> dataSource;
        private bool locked = true;

        private List<string> keys;

        public DictionaryCollection<T> DataSource
        {
            get => dataSource;
            set
            {
                if (locked)
                    throw new InvalidOperationException("数据源已锁定，不能设置数据源。");

            }
        }
        public bool SupportsChangeNotification => true;
        public ListSortDirection SortDirection { get; set; }

        public DictionaryView(DictionaryCollection<T> dictionary)
        {
            dataSource = dictionary;
            keys = dictionary.Keys.ToList();
        }

        private void ThrowExceptionIfDataSourceIsNull()
        {
            if (dataSource == null)
                throw new InvalidOperationException("数据源为空。");
        }

        public event ListChangedEventHandler ListChanged;

        public int Count
        {
            get
            {
                ThrowExceptionIfDataSourceIsNull();

                return keys.Count;
            }
        }

        public bool Contains(object value)
        {
            ThrowExceptionIfDataSourceIsNull();

            return dataSource.Values.Contains((T)value);
        }

        public IEnumerator GetEnumerator()
        {
            if (dataSource == null)
            {
                return Empty;
            }
            else
            {
                return dataSource.Values.GetEnumerator();
            }
        }

        protected virtual void OnListChanged(ListChangedEventArgs args)
        {
            ListChanged?.Invoke(this, args);
        }

        private void OnItemChanged(ListChangedType changedType, int index)
        {
            ListChangedEventArgs args = new ListChangedEventArgs(changedType, index);
            OnListChanged(args);
        }

        public void AddItem(string key)
        {
            ThrowExceptionIfDataSourceIsNull();

            if (dataSource.ContainsKey(key))
            {
                int index = keys.IndexOf(key);
                if (index == -1)
                {
                    keys.Add(key);
                    index = keys.Count - 1;
                }
                OnItemChanged(ListChangedType.ItemAdded, index);
            }
        }
        public void ResetItem(string key)
        {
            ThrowExceptionIfDataSourceIsNull();

            if (dataSource.ContainsKey(key))
            {
                int index = keys.IndexOf(key);
                if (index > -1)
                {
                    OnItemChanged(ListChangedType.ItemChanged, index);
                }
            }
        }
        public void DeletedItem(string key)
        {
            ThrowExceptionIfDataSourceIsNull();

            if (!dataSource.ContainsKey(key))
            {
                int index = keys.IndexOf(key);
                if (index > -1)
                {
                    keys.RemoveAt(index);
                }
                OnItemChanged(ListChangedType.ItemDeleted, index);
            }
        }
        public void ResetAll()
        {
            keys = dataSource.Keys.ToList();
            OnItemChanged(ListChangedType.Reset, -1);
        }

        #region 绑定支持
        object IList.this[int index]
        {
            get
            {
                return dataSource[keys[index]];
            }
            set
            {
                dataSource[keys[index]] = (T)value;
            }
        }
        #endregion

        #region 固定值
        public object SyncRoot { get; } = new object();
        public bool IsSynchronized => false;
        bool IBindingList.AllowNew => false;
        bool IBindingList.AllowEdit => false;
        bool IBindingList.AllowRemove => false;
        bool IBindingList.SupportsSearching => false;
        bool IBindingList.SupportsSorting => false;
        bool IBindingList.IsSorted => false;
        PropertyDescriptor IBindingList.SortProperty => null;
        bool IList.IsReadOnly => true;
        bool IList.IsFixedSize => true;
        #endregion

        #region 不支持

        object IBindingList.AddNew()
        {
            throw new InvalidOperationException("不支持添加。");
        }

        void IBindingList.AddIndex(PropertyDescriptor property)
        {
            //throw new InvalidOperationException("不支持 AddIndex。");
        }

        void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            //throw new InvalidOperationException("不支持 ApplySort。");
        }

        int IBindingList.Find(PropertyDescriptor property, object key)
        {
            throw new InvalidOperationException("不支持 ApplySort。");
        }

        void IBindingList.RemoveIndex(PropertyDescriptor property)
        {
            //throw new InvalidOperationException("不支持 RemoveIndex。");
        }

        void IBindingList.RemoveSort()
        {
            //throw new InvalidOperationException("不支持 RemoveSort。");
        }

        int IList.Add(object value)
        {
            throw new InvalidOperationException("不支持 Add(object)。");
        }

        void IList.Clear()
        {
            throw new InvalidOperationException("不支持清空。");
        }

        int IList.IndexOf(object value)
        {
            throw new InvalidOperationException("不支持 IndexOf。");
        }

        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException("不支持 Insert。");
        }

        void IList.Remove(object value)
        {
            throw new InvalidOperationException("不支持 Remove。");
        }

        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException("不支持 RemoveAt。");
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new InvalidOperationException("不支持复制。");
        }
        #endregion

    }
}
