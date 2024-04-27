using System;
using System.Collections;
using System.Collections.Generic;

namespace CBK.Framework.Collections
{
    /// <summary>
    /// 可观察列表
    /// </summary>
    public sealed class ObservableList<T> : IList<T>, ICollection
    {
        private const int DefaultCapacity = 4;

        public event Action OnChanged;
        public int Count => m_List.Count;
        public bool IsReadOnly => ((ICollection<T>)m_List).IsReadOnly;
        public bool IsSynchronized => ((ICollection)m_List).IsSynchronized;
        public object SyncRoot => ((ICollection)m_List).SyncRoot;

        public T this[int index]
        {
            get => m_List[index];
            set
            {
                m_List[index] = value;
                MarkListChanged();
            }
        }

        private readonly List<T> m_List;
        private readonly BatchOperator m_BatchOperator;
        private bool m_IsChanged;

        /// <summary>
        /// 开始批量操作 批量操作完成需要调用EndBatch或者Dispose
        /// </summary>
        /// <returns></returns>
        public BatchOperator BeginBatch()
        {
            return m_BatchOperator.BeginBatch();
        }

        /// <summary>
        /// 结束批量操作
        /// </summary>
        public void EndBatch()
        {
            m_BatchOperator.EndBatch();
        }

        public IEnumerator GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        /// <summary>
        /// 返回列表中是否包含指定元素
        /// </summary>
        public bool Contains(T item)
        {
            m_List.Sort();
            return m_List.Contains(item);
        }

        /// <summary>
        /// 获取元素索引
        /// </summary>
        public int IndexOf(T item)
        {
            return m_List.IndexOf(item);
        }

        /// <summary>
        /// 在指定索引处插入元素
        /// </summary>
        public void Insert(int index, T item)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException();

            // 如果索引超出列表范围则直接添加到末尾
            if (index > m_List.Count)
                m_List.Add(item);
            else
                m_List.Insert(index, item);

            MarkListChanged();
        }

        /// <summary>
        /// 移除指定索引处的元素
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= m_List.Count)
                throw new ArgumentOutOfRangeException();

            m_List.RemoveAt(index);
            MarkListChanged();
        }

        /// <summary>
        /// 添加元素到列表中
        /// </summary>
        public void Add(T item)
        {
            m_List.Add(item);
            MarkListChanged();
        }

        #region [AddRange]

        /// <summary>
        /// 添加指定集合到列表中
        /// </summary>
        public void AddRange(IEnumerable<T> enumerable)
        {
            var hasChanged = false;

            foreach (var item in enumerable)
            {
                m_List.Add(item);
                hasChanged = true;
            }

            if (!hasChanged)
                return;

            MarkListChanged();
        }

        /// <summary>
        /// 添加指定集合到列表中
        /// </summary>
        public void AddRange(ICollection<T> collection)
        {
            if (collection.Count == 0)
                return;

            foreach (var item in collection)
                m_List.Add(item);

            MarkListChanged();
        }

        /// <summary>
        /// 添加指定集合到列表中
        /// </summary>
        public void AddRange(IEnumerable<T> enumerable, Func<T, bool> filter)
        {
            var hasChanged = false;
            
            foreach (var item in enumerable)
            {
                if (!filter(item)) 
                    continue;
                
                m_List.Add(item);
                hasChanged = true;
            }
            
            if (!hasChanged)
                return;
            
            MarkListChanged();
        }
        
        /// <summary>
        /// 添加指定集合到列表中
        /// </summary>
        public void AddRange(ICollection<T> collection, Func<T, bool> filter)
        {
            if (collection.Count == 0)
                return;

            var hasChanged = false;
            
            foreach (var item in collection)
            {
                if (!filter(item)) 
                    continue;
                
                m_List.Add(item);
                hasChanged = true;
            }
            
            if (!hasChanged)
                return;
            
            MarkListChanged();
        }

        #endregion

        /// <summary>
        /// 按照指定规则排序
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            if (m_List.Count == 0)
                return;

            // 这里不去关心列表是否真的触发了顺序变更
            m_List.Sort(comparison);
            MarkListChanged();
        }

        /// <summary>
        /// 按照元素的默认规则排序
        /// </summary>
        public void Sort()
        {
            if (m_List.Count == 0)
                return;

            // 这里不去关心列表是否真的触发了顺序变更
            m_List.Sort();
            MarkListChanged();
        }

        /// <summary>
        /// 按照指定规则排序
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            if (m_List.Count == 0)
                return;

            // 这里不去关心列表是否真的触发了顺序变更
            m_List.Sort(comparer);
            MarkListChanged();
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        public bool Remove(T item)
        {
            var result = m_List.Remove(item);
            if (result)
            {
                MarkListChanged();
            }

            return result;
        }

        /// <summary>
        /// 清空列表
        /// </summary>
        public void Clear()
        {
            if (m_List.Count == 0)
                return;

            m_List.Clear();
            MarkListChanged();
        }

        /// <summary>
        /// 复制列表到数组
        /// </summary>
        public void CopyTo(T[] array, int arrayIndex)
        {
            m_List.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 复制列表到数组
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)m_List).CopyTo(array, index);
        }

        /// <summary>
        /// 构造函数 允许指定初始容量
        /// </summary>
        public ObservableList(int capacity = DefaultCapacity)
        {
            m_List = new List<T>(capacity);
            m_BatchOperator = new BatchOperator(this);
        }

        /// <summary>
        /// 标记列表变更
        /// </summary>
        private void MarkListChanged()
        {
            // 如果有引用则标记为变更 否则直接触发OnChanged事件
            if (m_BatchOperator.IsBatching)
                m_IsChanged = true;
            else
                OnChanged?.Invoke();
        }

        /// <summary>
        /// 批量操作器
        /// </summary>
        public sealed class BatchOperator : IDisposable
        {
            private readonly ObservableList<T> m_Owner;
            private int m_RefCount;

            /// <summary>
            /// 是否正在批量操作
            /// </summary>
            public bool IsBatching => m_RefCount > 0;

            public BatchOperator(ObservableList<T> owner)
            {
                m_Owner = owner;
            }

            /// <summary>
            /// 开始批量操作 批量操作完成需要调用EndBatch或者Dispose
            /// </summary>
            public BatchOperator BeginBatch()
            {
                ++m_RefCount;
                return this;
            }

            /// <summary>
            /// 结束批量操作
            /// </summary>
            public void EndBatch()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (m_RefCount == 0)
                    return;

                --m_RefCount;

                if (m_RefCount == 0 && m_Owner.m_IsChanged)
                    m_Owner.MarkListChanged();
            }

            public BatchOperator Add(T item)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Add(item);
                return this;
            }

            public BatchOperator Remove(T item)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Remove(item);
                return this;
            }

            public BatchOperator Clear()
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Clear();
                return this;
            }

            public BatchOperator Insert(int index, T item)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Insert(index, item);
                return this;
            }

            public BatchOperator RemoveAt(int index)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.RemoveAt(index);
                return this;
            }

            public BatchOperator Sort()
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Sort();
                return this;
            }

            public BatchOperator Sort(Comparison<T> comparison)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Sort(comparison);
                return this;
            }

            public BatchOperator Sort(IComparer<T> comparer)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.Sort(comparer);
                return this;
            }

            public BatchOperator AddRange(IEnumerable<T> enumerable)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.AddRange(enumerable);
                return this;
            }

            public BatchOperator AddRange(ICollection<T> collection)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.AddRange(collection);
                return this;
            }
            
            public BatchOperator AddRange(IEnumerable<T> enumerable, Func<T, bool> filter)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.AddRange(enumerable, filter);
                return this;
            }
            
            public BatchOperator AddRange(ICollection<T> collection, Func<T, bool> filter)
            {
                if (m_RefCount == 0)
                    throw new InvalidOperationException("BatchOperator is not in batch mode.");

                m_Owner.AddRange(collection, filter);
                return this;
            }
        }
    }
}