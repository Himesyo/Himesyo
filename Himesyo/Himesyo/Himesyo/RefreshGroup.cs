using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Himesyo
{
    /// <summary>
    /// 表示一组刷新列表。可以添加或移除指定刷新项，但不允许获取和操作其他项。
    /// </summary>
    public sealed class RefreshGroup : ICollection<IRefreshItem>, IRefreshItem
    {
        /// <summary>
        /// 为指定对象添加关联刷新对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="refreshItem"></param>
        public static void Add(object obj, IRefreshItem refreshItem)
        {
            RefreshGroup group = neetRefreshObjectCollection.GetOrCreateValue(obj);
            group.Add(refreshItem);
        }
        /// <summary>
        /// 移除指定对象关联的指定刷新对象。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="refreshItem"></param>
        public static void Remove(object obj, IRefreshItem refreshItem)
        {
            if (neetRefreshObjectCollection.TryGetValue(obj, out RefreshGroup group))
            {
                group.Remove(refreshItem);
            }
        }
        /// <summary>
        /// 移除指定对象的所有关联对象。如果重新添加，将重新创建组。慎用。
        /// </summary>
        /// <param name="obj"></param>
        [Obsolete]
        public static void RemoveAll(object obj)
        {
            neetRefreshObjectCollection.Remove(obj);
        }
        /// <summary>
        /// 刷新与指定对象关联的刷新对象。
        /// </summary>
        /// <param name="obj"></param>
        public static void StartRefresh(object obj)
        {
            if (neetRefreshObjectCollection.TryGetValue(obj, out RefreshGroup group))
            {
                group.Refresh(obj);
            }
        }

        private static ConditionalWeakTable<object, RefreshGroup> neetRefreshObjectCollection = new ConditionalWeakTable<object, RefreshGroup>();
        private readonly IList<IRefreshItem> collection = new List<IRefreshItem>();
        int ICollection<IRefreshItem>.Count => throw new InvalidOperationException("不允许的操作。");
        bool ICollection<IRefreshItem>.IsReadOnly => false;

        /// <summary>
        /// 刷新组内所有对象。
        /// </summary>
        /// <param name="refreshObject"></param>
        public void Refresh(object refreshObject)
        {
            foreach (var item in collection)
            {
                item.Refresh(refreshObject);
            }
        }
        /// <summary>
        /// 获取表示组内是否含有项的值。
        /// </summary>
        /// <returns></returns>
        public bool Any()
        {
            return collection.Count > 0;
        }
        /// <summary>
        /// 添加指定刷新项。
        /// </summary>
        /// <param name="item"></param>
        public void Add(IRefreshItem item)
        {
            collection.Add(item);
        }
        /// <summary>
        /// 确定指定刷新项是否存在。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(IRefreshItem item)
        {
            return collection.Contains(item);
        }
        /// <summary>
        /// 移除指定刷新项。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(IRefreshItem item)
        {
            return collection.Remove(item);
        }

        void ICollection<IRefreshItem>.Clear()
        {
            throw new InvalidOperationException("不允许的操作。");
        }
        void ICollection<IRefreshItem>.CopyTo(IRefreshItem[] array, int arrayIndex)
        {
            throw new InvalidOperationException("不允许的操作。");
        }
        IEnumerator<IRefreshItem> IEnumerable<IRefreshItem>.GetEnumerator()
        {
            throw new InvalidOperationException("不允许的操作。");
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new InvalidOperationException("不允许的操作。");
        }
    }

    /// <summary>
    /// <see cref="IRefreshItem"/> 的简单实现。刷新时执行指定的委托。
    /// </summary>
    public class RefreshItem : IRefreshItem
    {
        private readonly RefreshAction refreshAction;

        /// <summary>
        /// 使用刷新时执行的委托初始化 <see cref="RefreshItem"/> 的新实例。
        /// </summary>
        /// <param name="action"></param>
        public RefreshItem(RefreshAction action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            refreshAction = action;
        }

        /// <summary>
        /// 立即刷新
        /// </summary>
        /// <param name="refreshObject">与刷新关联的对象。如果没有这样的对象，则为 <see langword="null"/> 。</param>
        public void Refresh(object refreshObject)
        {
            refreshAction(refreshObject);
        }
    }

    /// <summary>
    /// 表示一个刷新动作。
    /// </summary>
    /// <param name="refreshObject">与此动作关联的对象</param>
    public delegate void RefreshAction(object refreshObject);

    /// <summary>
    /// 表示一个可以刷新的项。使用时注意不要循环触发。
    /// </summary>
    public interface IRefreshItem
    {
        /// <summary>
        /// 立即刷新
        /// </summary>
        /// <param name="refreshObject">与刷新关联的对象。如果没有这样的对象，则为 <see langword="null"/> 。</param>
        void Refresh(object refreshObject);
    }
}
