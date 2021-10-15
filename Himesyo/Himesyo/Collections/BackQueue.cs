using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Himesyo.Collections
{
    /// <summary>
    /// 表示一个具有固定容量、可随机访问、倒向索引的集合。超过集合容量时，移除最早添加的对象。
    /// </summary>
    public class BackQueue<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        private readonly T[] innerArray;
        private readonly int maxCapacity;
        private int count;
        private int endIndex;
        private int ver = 0;

        /// <summary>
        /// 使用最大容量初始化集合。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="maxCapacity"/> 小于 0 。</exception>
        public BackQueue(int maxCapacity)
        {
            innerArray = new T[maxCapacity];
            this.maxCapacity = maxCapacity;
            endIndex = -1;
            count = 0;
        }

        /// <summary>
        /// 使用指定集合中的元素和最大容量初始化集合。如果集合中的元素数量大于 <paramref name="maxCapacity"/> ，则将超出范围的数量的元素从集合前方移除。
        /// </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="collection"/> 为 <see langword="null"/> 。</exception>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="maxCapacity"/> 小于 0。</exception>
        public BackQueue(IEnumerable<T> collection, int maxCapacity)
            : this(maxCapacity)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (T item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// 获取当前集合的容量。它是固定的。
        /// </summary>
        public int Capacity => maxCapacity;

        /// <summary>
        /// 添加一个集合。
        /// </summary>
        /// <param name="collection"></param>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Add(item);
            }
        }

        /// <summary>
        /// 移除最开始添加的元素。如果集合中没有元素，则什么也不做。
        /// </summary>
        public void Remove()
        {
            if (count > 0)
            {
                count--;
                this[count] = default;
            }
        }
        /// <summary>
        /// 移除指定数量的最开始添加的元素。如果集合中没有元素，则什么也不做。
        /// </summary>
        public void Remove(int count)
        {
            if (count >= Count)
            {
                Clear();
            }
            else if (count > 0)
            {
                this.count--;
                this[count] = default;
            }
        }
        /// <summary>
        /// 移除指定数量的最后添加的元素。如果集合中没有元素，则什么也不做。
        /// </summary>
        public void RemoveLast(int count)
        {
            if (count >= Count)
            {
                Clear();
            }
            else if (count > 0)
            {
                SetCount(this.count - count);
                ver++;
            }
        }

        /// <summary>
        /// 设置集合中的元素数量，将最开始添加的元素舍弃。
        /// </summary>
        /// <param name="newCount"></param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="newCount"/> 值小于 0 或大于当前的元素总数。</exception>
        public void SetCount(int newCount)
        {
            if (newCount < 0 || newCount > count)
                throw new ArgumentOutOfRangeException(nameof(newCount));
            if (newCount == 0)
            {
                Clear();
            }
            else if (newCount != count)
            {
                int max = count;
                count = newCount;
                for (int i = newCount; i < max; i++)
                {
                    this[i] = default;
                }
                ver++;
            }
        }

        /// <summary>
        /// 创建一个新 <see cref="BackQueue{T}"/> 集合，它拥有指定长度并从当前集合复制最后添加的元素。
        /// </summary>
        /// <param name="newCount"></param>
        public BackQueue<T> Copy(int newCount)
        {
            if (newCount < 0)
                throw new ArgumentOutOfRangeException(nameof(newCount), newCount, "不能为负数。");

            BackQueue<T> newQueue = new BackQueue<T>(newCount);
            int length = Math.Min(count, newCount);
            for (int i = length - 1; i >= 0; i--)
            {
                newQueue.Add(this[i]);
            }
            return newQueue;
        }

        /// <summary>
        /// 刷新集合。清除已被移除但依然保留的元素引用。
        /// </summary>
        protected void Refresh()
        {
            if (count == 0)
            {
                innerArray.Initialize();
            }
            else
            {
                int start = GetStartIndex();
                if (start > endIndex)
                {
                    for (int i = endIndex + 1; i < start; i++)
                    {
                        innerArray[i] = default;
                    }
                }
                else
                {
                    for (int i = 0; i < start; i++)
                    {
                        innerArray[i] = default;
                    }
                    for (int i = endIndex + 1; i < maxCapacity; i++)
                    {
                        innerArray[i] = default;
                    }
                }
            }
        }

        #region 私有辅助成员

        private int GetStartIndex()
        {
            if (endIndex >= count - 1)
            {
                return endIndex - count + 1;
            }
            else
            {
                return maxCapacity + endIndex - count + 1;
            }
        }

        #endregion

        #region 接口公共成员

        /// <summary>
        /// 获取一个值，该值指示是否同步对 <see cref="BackQueue{T}"/> 的访问（线程安全）。
        /// </summary>
        /// <result>如果对 <see cref="BackQueue{T}"/> 的访问是同步的（线程安全），则为 <see langword="true"/> ；否则为 <see langword="false"/> 。</result>
        public bool IsSynchronized => false;

        /// <summary>
        /// 获取可用于同步对 <see cref="BackQueue{T}"/> 的访问的对象。
        /// </summary>
        /// <result>可用于同步对 <see cref="BackQueue{T}"/> 的访问的对象。</result>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// 获取指定索引处的元素。0 表示最后添加的元素。
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (index >= count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (index <= endIndex)
                {
                    return innerArray[endIndex - index];
                }
                else
                {
                    return innerArray[count + endIndex - index];
                }
            }
            private set
            {
                if (index <= endIndex)
                {
                    innerArray[endIndex - index] = value;
                }
                else
                {
                    innerArray[count + endIndex - index] = value;
                }
            }
        }

        /// <summary>
        /// 返回列表中的实际数量。
        /// </summary>
        public int Count => count;

        /// <summary>
        /// 将指定元素添加到集合末尾。
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            endIndex++;
            endIndex %= maxCapacity;
            innerArray[endIndex] = item;
            if (count < maxCapacity)
            {
                count++;
            }
            ver++;
        }

        /// <summary>
        /// 清空集合。
        /// </summary>
        public void Clear()
        {
            // 初始化为默认值是为了释放引用。
            innerArray.Initialize();
            endIndex = -1;
            count = 0;
            ver = 0;
        }

        /// <summary>
        /// 确定 <see cref="BackQueue{T}"/> 是否包含特定值。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            foreach (T value in this)
            {
                if (Equals(value, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            int curr = ver;
            if (endIndex >= count - 1)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return innerArray[endIndex - i];
                    if (curr != ver)
                        throw new InvalidOperationException("集合已修改；可能无法执行枚举操作。");
                }
            }
            else
            {
                for (int i = endIndex; i >= 0; i--)
                {
                    yield return innerArray[i];
                    if (curr != ver)
                        throw new InvalidOperationException("集合已修改；可能无法执行枚举操作。");
                }
                int statrIndex = GetStartIndex();
                for (int i = maxCapacity - 1; i >= statrIndex; i--)
                {
                    yield return innerArray[i];
                    if (curr != ver)
                        throw new InvalidOperationException("集合已修改；可能无法执行枚举操作。");
                }
            }
        }

        /// <summary>
        /// 确定 <see cref="BackQueue{T}"/> 中特定项的索引。
        /// </summary>
        /// <param name="item"></param>
        /// <returns>如果在列表中找到，则为 item 的索引；否则为 -1。</returns>
        public int IndexOf(T item)
        {
            int index = -1;
            foreach (T value in this)
            {
                index++;
                if (Equals(value, item))
                {
                    return index;
                }
            }
            return index;
        }

        /// <summary>
        /// 从特定的 <paramref name="arrayIndex"/> 索引处开始，将 <see cref="BackQueue{T}"/> 的元素复制到一个 <see cref="Array"/> 中。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (arrayIndex + count > array.Length)
                throw new ArgumentException("目标数组没有足够的容量容纳集合中的元素。");

            int index = arrayIndex;
            foreach (var item in this)
            {
                array[index++] = item;
            }
        }

        #endregion

        #region IList<T>

        bool IList.IsFixedSize => false;

        T IList<T>.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get => this[index];
            set => throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            Add((T)value);
            return endIndex;
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<T>

        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsReadOnly => false;

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IEnumerable<T>

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
