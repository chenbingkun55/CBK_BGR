using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CBK.Framework.Res;
using CBK.Framework.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池实现
    /// </summary>
    internal sealed class ObjectPoolImpl : IObjectPool
    {
        private static uint s_Serial = 0;
        
        private bool m_Disposed;

        private readonly IResLoader m_ResLoader;
        private readonly Transform m_PoolRoot;
        private readonly Dictionary<string, PackedInfo> m_DictPackedInfo = new Dictionary<string, PackedInfo>();
        private readonly Dictionary<int, string> m_DictInstanceIdToAssetKey = new Dictionary<int, string>();
        private readonly DisposableGroup m_DisposableGroup = new DisposableGroup();

        public ObjectPoolImpl(IResLoader resLoader)
        {
            var gameObject = new GameObject($"ObjectPool_{++s_Serial}");
            Object.DontDestroyOnLoad(gameObject);
            m_PoolRoot = gameObject.transform;
            m_ResLoader = resLoader;
        }

        public void Dispose()
        {
            if (m_Disposed)
                return;
            
            m_Disposed = true;
            m_DisposableGroup.Dispose();
            m_DictPackedInfo.Clear();
            m_DictInstanceIdToAssetKey.Clear();
            Object.Destroy(m_PoolRoot.gameObject);
            m_ResLoader.Dispose();
        }
        
        public void SetLimit(string assetKey, int limit)
        {
            if (m_Disposed)
                return;
            
            GetPackedInfo(assetKey).SetLimit(limit);
        }

        public GameObject Instantiate(string assetKey)
        {
            if (m_Disposed)
                return null;
            
            var packedInfo = GetPackedInfo(assetKey);
            packedInfo.LoadSync();
            
            var instance = packedInfo.Instantiate();
            if (instance == null)
                return null;
            
            m_DictInstanceIdToAssetKey.Add(instance.GetInstanceID(), assetKey);
            return instance;
        }

        public async UniTask<GameObject> InstantiateAsync(string assetKey)
        {
            if (m_Disposed)
                return null;
            
            var packedInfo = GetPackedInfo(assetKey);
            await packedInfo.LoadAsync();
            
            if (m_Disposed)
                return null;
            
            var instance = packedInfo.Instantiate();
            if (instance == null)
                return null;
            
            m_DictInstanceIdToAssetKey.Add(instance.GetInstanceID(), assetKey);
            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null)
                return;

            if (m_Disposed)
            {
                Object.Destroy(instance);
                return;
            }
            
            var instanceID = instance.GetInstanceID();
            if (!m_DictInstanceIdToAssetKey.TryGetValue(instanceID, out var assetKey))
            {
                // 未知实例，直接销毁
                Object.Destroy(instance);
                return;
            }

            m_DictInstanceIdToAssetKey.Remove(instanceID);
            GetPackedInfo(assetKey).Release(instance);
        }

        public void DestroyUnusedObjects()
        {
            if (m_Disposed)
                return;
            
            foreach (var packedInfo in m_DictPackedInfo.Values)
            {
                packedInfo.DestroyUnusedObjects();
            }
        }
        
        private PackedInfo GetPackedInfo(string assetKey)
        {
            if (m_DictPackedInfo.TryGetValue(assetKey, out var packedInfo))
                return packedInfo;

            packedInfo = new PackedInfo(m_PoolRoot, assetKey, m_ResLoader);
            m_DictPackedInfo.Add(assetKey, packedInfo);
            m_DisposableGroup.Add(packedInfo);
            return packedInfo;
        }

        private sealed class PackedInfo : IDisposable
        {
            /// <summary>
            /// 对象池根节点
            /// </summary>
            private readonly Transform m_PoolRoot;
            
            /// <summary>
            /// 资源Key
            /// </summary>
            private readonly string m_AssetKey;
            
            /// <summary>
            /// 资源加载器
            /// </summary>
            private readonly IResLoader m_ResLoader;
            
            /// <summary>
            /// 预制体实例
            /// </summary>
            private GameObject m_Prefab;
            
            /// <summary>
            /// 对象实例
            /// </summary>
            private readonly Queue<GameObject> m_Instances = new Queue<GameObject>();
            
            /// <summary>
            /// 对象池中最大实例数量
            /// </summary>
            private int m_Limit;
            
            /// <summary>
            /// 是否已加载
            /// </summary>
            private bool m_Loaded;

            /// <summary>
            /// 是否正在加载
            /// </summary>
            private bool m_Loading;
            
            /// <summary>
            /// 是否已销毁
            /// </summary>
            private bool m_Disposed;

            public PackedInfo(Transform poolRoot, string assetKey, IResLoader resLoader)
            {
                var root = new GameObject(assetKey).transform;
                root.SetParent(poolRoot);
                
                m_PoolRoot = root;
                m_AssetKey = assetKey;
                m_ResLoader = resLoader;
            }

            public void LoadSync()
            {
                if (m_Loaded)
                    return;
                
                if (m_Disposed)
                    return;

                m_Prefab = m_ResLoader.LoadSync<GameObject>(m_AssetKey);
                m_Loaded = true;
                m_Loading = false;
            }

            public UniTask LoadAsync()
            {
                if (m_Loaded)
                    return UniTask.CompletedTask;
                
                if (m_Disposed)
                    return UniTask.CompletedTask;
                
                if (!m_Loading)
                {
                    m_Loading = true;
                    LoadAsyncInner().Forget();
                }

                return UniTask.WaitWhile(() => m_Loading);

                async UniTask LoadAsyncInner()
                {
                    var prefab = await m_ResLoader.LoadAsync<GameObject>(m_AssetKey);

                    if (!m_Loading)
                    {
                        m_ResLoader.Release(prefab);
                        return;
                    }
                    
                    m_Prefab = prefab;
                    m_Loaded = true;
                    m_Loading = false;
                }
            }
            
            public GameObject Instantiate()
            {
                if (m_Disposed)
                    return null;
                
                if (m_Instances.Count > 0)
                {
                    var instance = m_Instances.Dequeue();
                    instance.transform.SetParent(null);
                    return instance;
                }

                if (m_Prefab == null)
                    return null;

                return UnityEngine.Object.Instantiate(m_Prefab);
            }

            public void Release(GameObject gameObject)
            {
                if (gameObject == null)
                    return;

                if (m_Disposed)
                {
                    UnityEngine.Object.Destroy(gameObject);
                    return;
                }
                
                if (m_Limit >= 0 && m_Instances.Count >= m_Limit)
                {
                    // 超过最大实例数量，直接销毁
                    UnityEngine.Object.Destroy(gameObject);
                    return;
                }
                
                gameObject.transform.SetParent(m_PoolRoot);
                m_Instances.Enqueue(gameObject);
            }

            public void SetLimit(int limit)
            {
                if (m_Disposed)
                    return;
                
                m_Limit = limit;
                
                while (m_Instances.Count > m_Limit)
                {
                    var instance = m_Instances.Dequeue();
                    UnityEngine.Object.Destroy(instance);
                }
            }
            
            
            public void DestroyUnusedObjects()
            {
                if (m_Disposed)
                    return;
                
                while (m_Instances.Count > 0)
                {
                    var instance = m_Instances.Dequeue();
                    UnityEngine.Object.Destroy(instance);
                }
            }

            public void Dispose()
            {
                if (m_Disposed)
                    return;
                
                m_Disposed = true;
                m_Instances.Clear();
                UnityEngine.Object.Destroy(m_PoolRoot.gameObject);
            }
        }
    }
}