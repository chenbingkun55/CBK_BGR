using System.Collections.Generic;
using CatLib;
using CBK.Framework.Reference;

namespace CBK.Framework.Game
{
    /// <summary>
    /// 玩法实体仓库接口
    /// </summary>
    public interface IGameEntityRepository<TKey, TEntity> : IGameRepository where TEntity : IReference
    {
        /// <summary>
        /// 获取实体列表
        /// </summary>
        IReadOnlyList<TEntity> Entities { get; }

        /// <summary>
        /// 获取当前实体数量
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 查找指定实体
        /// </summary>
        TEntity Find(TKey key);

        /// <summary>
        /// 查找或创建指定实体
        /// </summary>
        TEntity FindOrCreate(TKey key);

        /// <summary>
        /// 创建指定实体
        /// </summary>
        TEntity Create(TKey key);

        /// <summary>
        /// 插入指定实体
        /// </summary>
        void Insert(TKey key, TEntity entity);

        /// <summary>
        /// 移除指定实体
        /// </summary>
        bool Remove(TKey key);
    }

    public abstract class AGameEntityRepository<T, TKey, TEntity> : Facade<T>, IGameEntityRepository<TKey, TEntity>
        where T : AGameEntityRepository<T, TKey, TEntity>
        where TEntity : IReference
    {
        public IReadOnlyList<TEntity> Entities => m_Entities;
        public int Count => m_Entities.Count;

        private readonly List<TEntity> m_Entities = new List<TEntity>();
        private readonly Dictionary<TKey, TEntity> m_Dict = new Dictionary<TKey, TEntity>();

        public TEntity Find(TKey key)
        {
            return m_Dict.TryGetValue(key, out var entity) ? entity : default;
        }

        public TEntity FindOrCreate(TKey key)
        {
            if (m_Dict.TryGetValue(key, out var entity))
                return entity;

            entity = CreateEntity(key);
            m_Entities.Add(entity);
            m_Dict.Add(key, entity);
            return entity;
        }

        public TEntity Create(TKey key)
        {
            if (m_Dict.ContainsKey(key))
                throw new System.Exception($"Entity({typeof(TEntity).Name}) with key '{key}' already exists.");

            var entity = CreateEntity(key);
            m_Entities.Add(entity);
            m_Dict.Add(key, entity);
            return entity;
        }

        public void Insert(TKey key, TEntity entity)
        {
            if (m_Dict.ContainsKey(key))
                throw new System.Exception($"Entity({typeof(TEntity).Name}) with key '{key}' already exists.");

            m_Entities.Add(entity);
            m_Dict.Add(key, entity);
        }

        public bool Remove(TKey key)
        {
            if (!m_Dict.TryGetValue(key, out var entity))
                return false;

            m_Entities.Remove(entity);
            m_Dict.Remove(key);
            entity.Recycle();
            return true;
        }

        public virtual void Clear()
        {
            m_Entities.RecycleItems();
            m_Dict.Clear();
        }

        protected abstract TEntity CreateEntity(TKey key);
    }
}