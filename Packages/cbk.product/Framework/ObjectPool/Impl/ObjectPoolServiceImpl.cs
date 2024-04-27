using System;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.Res;
using UnityEngine;

namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池服务实现
    /// </summary>
    internal sealed class ObjectPoolServiceImpl : IInitialize, IDisposable, IObjectPoolService
    {
        [Inject]
        public IResService ResService { get; set; }
        
        /// <summary>
        /// 对象池服务内部使用一个对象池来管理全局对象
        /// </summary>
        private IObjectPool m_ObjectPool;
        
        public void Initialize()
        {
            m_ObjectPool = Allocate();
        }

        public void Dispose()
        {
            m_ObjectPool.Dispose();
        }

        public void SetLimit(string assetKey, int limit)
        {
            m_ObjectPool.SetLimit(assetKey, limit);
        }

        public GameObject Instantiate(string assetKey)
        {
            return m_ObjectPool.Instantiate(assetKey);
        }

        public UniTask<GameObject> InstantiateAsync(string assetKey)
        {
            return m_ObjectPool.InstantiateAsync(assetKey);
        }

        public void Release(GameObject instance)
        {
            m_ObjectPool.Release(instance);
        }

        public void DestroyUnusedObjects()
        {
            m_ObjectPool.DestroyUnusedObjects();
        }

        public IObjectPool Allocate()
        {
            return new ObjectPoolImpl(ResService.Allocate());
        }
    }
}