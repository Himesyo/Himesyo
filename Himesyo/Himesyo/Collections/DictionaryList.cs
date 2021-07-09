using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Himesyo.Collections
{
    /// <summary>
    /// 对 <see cref="DictionaryCollection{TValue}"/> 实现数据绑定。
    /// <para>请订阅 <see cref="DictionaryList{TValue}.CreateAndAdd"/> 事件并将新项添加到 <see cref="DictionaryCollection{TValue}"/> 。</para>
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Obsolete("请考虑改用 DictionaryView<T> 类型。")]
    public class DictionaryList<TValue> : BindingList<TValue>, IBindingList
    {
        /// <summary>
        /// 创建新项时触发。请创建新项并添加到 <see cref="DictionaryCollection{TValue}"/> 集合。
        /// </summary>
        public event AddingNewEventHandler CreateAndAdd
        {
            add { base.AddingNew += value; }
            remove { base.AddingNew -= value; }
        }

        /// <summary>
        /// 关联的 <see cref="DictionaryCollection{TValue}"/> 。
        /// </summary>
        protected DictionaryCollection<TValue> InnerDictionary { get; }
        /// <summary>
        /// 使用指定 <see cref="DictionaryCollection{TValue}"/> 初始化 <see cref="DictionaryList{TValue}"/> 类的新实例。
        /// </summary>
        /// <param name="dictionary"></param>
        public DictionaryList(DictionaryCollection<TValue> dictionary)
            : base(dictionary.Values.ToList())
        {
            InnerDictionary = dictionary;
            InnerDictionary.ItemChanged -= Dictionary_ItemChanged;
            InnerDictionary.ItemChanged += Dictionary_ItemChanged;
        }

        // Private helper method
        private object FireAddingNew()
        {
            AddingNewEventArgs e = new AddingNewEventArgs(null);
            OnAddingNew(e);
            return e.NewObject;
        }
        private void Dictionary_ItemChanged(object sender, ItemChangedEventArgs<TValue> e)
        {
            switch (e.Type)
            {
                case ChangedType.Add:
                    Add(e.ChangedValue);
                    break;
                case ChangedType.Remove:
                    Remove(e.ChangedValue);
                    break;
                case ChangedType.Update:
                    Remove(e.OldValue);
                    Add(e.ChangedValue);
                    break;
                case ChangedType.Clear:
                    Clear();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 刷新指定值。
        /// </summary>
        /// <param name="value"></param>
        public void RefreshValue(TValue value)
        {
            int index = Items.IndexOf(value);
            if (index > -1)
            {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
            }
        }

        /// <summary>
        /// 将指定的项插入到列表中的指定索引位置。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, TValue item)
        {
            base.InsertItem(index, item);
        }

        /// <summary>
        /// 将新项添加到集合末尾。
        /// </summary>
        /// <returns></returns>
        protected override object AddNewCore()
        {
            // Create new item and add it to list
            object newItem = FireAddingNew();
            // Return new item to caller
            return newItem;
        }

        /// <summary>
        /// 移除位于指定索引处的项。
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            TValue value = Items[index];
            base.RemoveItem(index);
            InnerDictionary.RemoveWhere(pair => Equals(pair.Value, value), 1);
        }

    }
}
