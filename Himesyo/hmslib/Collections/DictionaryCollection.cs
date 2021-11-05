using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.Data;
using System.ComponentModel;

namespace Himesyo.Collections
{
    /// <summary>
    /// 可 Xml 序列化的键为 <see langword="string"/> 类型的字典集合。<typeparamref name="TValue"/> 必须可进行 Xml 序列化。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class DictionaryCollection<TValue>
        : IDictionary<string, TValue>
        , IXmlSerializable, ISerializable, IDeserializationCallback
        , IListSource
        , ICollection<KeyValuePair<string, TValue>>
        , IEnumerable<KeyValuePair<string, TValue>>
        , IDictionary, ICollection, IEnumerable
        , ICloneable
    {
        private Dictionary<string, TValue> dictionary;

        /// <summary>
        /// 在项列表改变时通知。
        /// </summary>
        public event ItemChangedEventHandler<TValue> ItemChanged;

        /// <summary>
        /// 用于显示此字典的默认视图。
        /// </summary>
        public DictionaryView<TValue> DefaultView { get; }

        /// <summary>
        /// 初始化具有默认大小的新实例。
        /// </summary>
        public DictionaryCollection()
        {
            dictionary = new Dictionary<string, TValue>();
            DefaultView = new DictionaryView<TValue>(this);
        }
        /// <summary>
        /// 使用指定字典集合初始化新实例。
        /// </summary>
        /// <param name="pairs"></param>
        public DictionaryCollection(IDictionary<string, TValue> pairs)
        {
            dictionary = new Dictionary<string, TValue>(pairs);
            DefaultView = new DictionaryView<TValue>(this);
        }
        /// <summary>
        /// 序列化构造函数
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DictionaryCollection(SerializationInfo info, StreamingContext context)
        {
            Type type = typeof(Dictionary<string, TValue>);
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.CreateInstance | BindingFlags.Instance, null, new[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            dictionary = (Dictionary<string, TValue>)constructor.Invoke(new object[] { info, context });
            DefaultView = new DictionaryView<TValue>(this);
        }

        /// <summary>
        /// 根据指定谓词移除所有符合条件的项。
        /// </summary>
        /// <param name="match"></param>
        public void RemoveWhere(Predicate<KeyValuePair<string, TValue>> match)
        {
            foreach (var pair in dictionary.ToList())
            {
                if (match(pair))
                {
                    Remove(pair.Key);
                }
            }
        }
        /// <summary>
        /// 根据指定谓词移除最多数量个符合条件的项。
        /// </summary>
        /// <param name="match"></param>
        /// <param name="count"></param>
        public void RemoveWhere(Predicate<KeyValuePair<string, TValue>> match, int count)
        {
            int needRemoveCount = count;
            foreach (var pair in dictionary.ToList())
            {
                if (needRemoveCount <= 0)
                {
                    return;
                }
                if (match(pair))
                {
                    Remove(pair.Key);
                    needRemoveCount--;
                }
            }
        }

        /// <summary>
        /// 创建副本。如果 <typeparamref name="TValue"/> 实现 <see cref="ICloneable"/> ，则使用 <see cref="ICloneable.Clone"/> 创建 <typeparamref name="TValue"/> 的副本。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            var other = (DictionaryCollection<TValue>)base.MemberwiseClone();
            if (typeof(ICloneable).IsAssignableFrom(typeof(TValue)))
            {
                other.dictionary = dictionary.ToDictionary(item => item.Key, item => (TValue)((ICloneable)item.Value)?.Clone());
            }
            else
            {
                other.dictionary = dictionary.ToDictionary(item => item.Key, item => item.Value);
            }
            other.ItemChanged = (ItemChangedEventHandler<TValue>)ItemChanged?.Clone();
            return other;
        }

        private void EventHelper(ChangedType changedType, string key, TValue changedValue, TValue oldValue)
        {
            ItemChangedEventArgs<TValue> eventArgs = new ItemChangedEventArgs<TValue>(changedType, key, changedValue, oldValue);
            OnItemChanged(eventArgs);
            switch (changedType)
            {
                case ChangedType.Add:
                    DefaultView.AddItem(key);
                    break;
                case ChangedType.Remove:
                    DefaultView.DeletedItem(key);
                    break;
                case ChangedType.Update:
                    DefaultView.ResetItem(key);
                    break;
                case ChangedType.Clear:
                    DefaultView.ResetAll();
                    break;
            }
        }
        /// <summary>
        /// 触发 <see cref="ItemChanged"/> 事件。
        /// </summary>
        /// <param name="eventArgs"></param>
        protected virtual void OnItemChanged(ItemChangedEventArgs<TValue> eventArgs)
        {
            ItemChanged?.Invoke(this, eventArgs);
        }

        #region IDictionary<string, TValue> 接口成员

        /// <summary>
        /// 根据键获取值，如果未找到指定键则返回 <see langword="null"/> .
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[string key]
        {
            get
            {
                TValue value;
                dictionary.TryGetValue(key, out value);
                return value;
            }
            set
            {
                TValue oldValue;
                if (dictionary.TryGetValue(key, out oldValue))
                {
                    ((IDictionary<string, TValue>)dictionary)[key] = value;
                    EventHelper(ChangedType.Update, key, value, oldValue);
                }
                else
                {
                    dictionary.Add(key, value);
                    EventHelper(ChangedType.Add, key, value, oldValue);
                }
            }
        }

        /// <summary>
        /// 键集合
        /// </summary>
        public ICollection<string> Keys => ((IDictionary<string, TValue>)dictionary).Keys;

        /// <summary>
        /// 值集合
        /// </summary>
        public ICollection<TValue> Values => ((IDictionary<string, TValue>)dictionary).Values;

        /// <summary>
        /// 实际包含的元素数。
        /// </summary>
        public int Count => ((ICollection<KeyValuePair<string, TValue>>)dictionary).Count;

        /// <summary>
        /// 是否只读。
        /// </summary>
        public bool IsReadOnly => ((ICollection<KeyValuePair<string, TValue>>)dictionary).IsReadOnly;

        /// <summary>
        /// 是否固定大小。
        /// </summary>
        bool IDictionary.IsFixedSize => ((IDictionary)dictionary).IsFixedSize;

        /// <summary>
        /// 获取一个可用于同步对 <see cref="DictionaryCollection{TValue}"/> 的访问的对象。
        /// </summary>
        public object SyncRoot => ((ICollection)dictionary).SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)dictionary).IsSynchronized;

        object IDictionary.this[object key]
        {
            get { return this[(string)key]; }
            set { this[(string)key] = (TValue)value; }
        }

        /// <summary>
        /// 添加新对象。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(string key, TValue value)
        {
            ((IDictionary<string, TValue>)dictionary).Add(key, value);
            EventHelper(ChangedType.Add, key, value, default(TValue));
        }

        /// <summary>
        /// 添加新对象。
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<string, TValue> item)
        {
            ((ICollection<KeyValuePair<string, TValue>>)dictionary).Add(item);
            EventHelper(ChangedType.Add, item.Key, item.Value, default(TValue));
        }

        /// <summary>
        /// 清空集合。
        /// </summary>
        public void Clear()
        {
            ((ICollection<KeyValuePair<string, TValue>>)dictionary).Clear();
            EventHelper(ChangedType.Clear, null, default(TValue), default(TValue));
        }

        /// <summary>
        /// 指定键值对是否存在。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, TValue> item)
        {
            return ((ICollection<KeyValuePair<string, TValue>>)dictionary).Contains(item);
        }

        /// <summary>
        /// 指定键是否存在。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, TValue>)dictionary).ContainsKey(key);
        }

        /// <summary>
        /// 将集合内的所有元素复制到指定数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 获取枚举器。
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, TValue>>)dictionary).GetEnumerator();
        }

        /// <summary>
        /// 移除指定键。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            TValue value = this[key];
            bool result = ((IDictionary<string, TValue>)dictionary).Remove(key);
            if (result)
            {
                EventHelper(ChangedType.Remove, key, value, value);
            }
            return result;
        }

        /// <summary>
        /// 移除指定对象。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            bool result = ((ICollection<KeyValuePair<string, TValue>>)dictionary).Remove(item);
            if (result)
            {
                EventHelper(ChangedType.Remove, item.Key, item.Value, item.Value);
            }
            return result;
        }

        /// <summary>
        /// 尝试获取值。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out TValue value)
        {
            return ((IDictionary<string, TValue>)dictionary).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }

        #endregion

        #region IXmlSerializable 接口成员

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (!reader.IsEmptyElement)
            {
                XmlSerializer xml = new XmlSerializer(typeof(TValue));
                reader.Read();
                while (reader.LocalName == "item")
                {
                    string key = reader.GetAttribute("key");
                    reader.Read();
                    object value = xml.Deserialize(reader);
                    if (value is TValue)
                    {
                        dictionary.Add(key, (TValue)value);
                    }
                    reader.ReadEndElement();
                }
                reader.ReadEndElement();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(TValue));
            foreach (var item in dictionary)
            {
                writer.WriteStartElement("item");
                writer.WriteAttributeString("key", item.Key);
                if (item.Value != null)
                {
                    xml.Serialize(writer, item.Value);
                }
                writer.WriteEndElement();
            }
            //throw new NotImplementedException();
        }

        #endregion

        #region IListSource 接口成员
        IList IListSource.GetList()
        {
            return DefaultView;
        }

        bool IListSource.ContainsListCollection => false;
        #endregion

        #region IDictionary ICollection ISerializable IDeserializationCallback 显示接口实现

        ICollection IDictionary.Keys => ((IDictionary)dictionary).Keys;

        ICollection IDictionary.Values => ((IDictionary)dictionary).Values;

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)dictionary).Contains(key);
        }

        void IDictionary.Add(object key, object value)
        {
            Add((string)key, (TValue)value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IDictionary)dictionary).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Remove((string)key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)dictionary).CopyTo(array, index);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ((ISerializable)dictionary).GetObjectData(info, context);
        }

        void IDeserializationCallback.OnDeserialization(object sender)
        {
            ((IDeserializationCallback)dictionary).OnDeserialization(sender);
        }

        #endregion

    }

    /// <summary>
    /// 当用户修改 <see cref="DictionaryCollection{TValue}"/> 集合时所调用的事件处理程序类。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">包含事件数据的 <see cref="ItemChangedEventArgs{TValue}"/></param>
    public delegate void ItemChangedEventHandler<TValue>(object sender, ItemChangedEventArgs<TValue> e);

    /// <summary>
    /// 为 <see cref="DictionaryCollection{TValue}.ItemChanged"/> 事件提供数据。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ItemChangedEventArgs<TValue> : EventArgs
    {
        /// <summary>
        /// 改变的类型
        /// </summary>
        public ChangedType Type { get; }
        /// <summary>
        /// 改变的键值。如果是 <see cref="ChangedType.Clear"/> ，则为 <see langword="null"/> 。
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// 改变的值。如果是 <see cref="ChangedType.Clear"/> ，则为 <see langword="default"/> 。
        /// </summary>
        public TValue ChangedValue { get; }
        /// <summary>
        /// 原来的值。如果是 <see cref="ChangedType.Add"/> ，则为 <see langword="default"/> 。
        /// </summary>
        public TValue OldValue { get; }

        /// <summary>
        /// 使用指定值初始化新实例。
        /// </summary>
        /// <param name="changedType"></param>
        /// <param name="key"></param>
        /// <param name="changedValue"></param>
        /// <param name="oldValue"></param>
        public ItemChangedEventArgs(ChangedType changedType, string key, TValue changedValue, TValue oldValue)
        {
            Type = changedType;
            Key = key;
            ChangedValue = changedValue;
            OldValue = oldValue;
        }
    }

    /// <summary>
    /// <see cref="DictionaryCollection{TValue}"/> 项改变时的改变类型。
    /// </summary>
    public enum ChangedType
    {
        /// <summary>
        /// 添加。
        /// </summary>
        Add,
        /// <summary>
        /// 移除。
        /// </summary>
        Remove,
        /// <summary>
        /// 更新。
        /// </summary>
        Update,
        /// <summary>
        /// 清空。
        /// </summary>
        Clear
    }
}
