using System;
using System.Collections.Generic;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using YooAsset;
using Object = UnityEngine.Object;

namespace CBK.Framework.Res.YooAsset
{
    /// <summary>
    /// 基于YooAsset的资源服务实现
    /// </summary>
    internal sealed partial class YooAssetResService : IResService, IDisposable
    {
        [Inject]
        public IReferenceService referenceService { get; set; }

        private readonly Dictionary<string, AssetOperationHandle> m_KeyToHandles = new();
        private readonly Dictionary<string, int> m_KeyToRefCounts = new();
        private readonly Dictionary<int, string> m_InstanceIDToKeys = new();

        public void Dispose()
        {
            UnloadResources();
        }

        public T LoadSync<T>(string assetKey) where T : Object
        {
            if (!m_KeyToHandles.TryGetValue(assetKey, out var handle))
            {
                handle = YooAssets.LoadAssetSync<T>(assetKey);
                m_KeyToHandles.Add(assetKey, handle);
            }
            else if (!handle.IsDone)
            {
                handle.WaitForAsyncComplete();
            }

            return GetAssetAndAddReference<T>(assetKey, handle);
        }

        public byte[] LoadRawSync(string assetKey)
        {
            var handle = YooAssets.LoadRawFileSync(assetKey);
            var bytes = handle.GetRawFileData();
            handle.Release();
            return bytes;
        }

        public string LoadRawTextSync(string assetKey)
        {
            var handle = YooAssets.LoadRawFileSync(assetKey);
            var text = handle.GetRawFileText();
            handle.Release();
            return text;
        }

        public async UniTask<T> LoadAsync<T>(string assetKey) where T : Object
        {
            if (!m_KeyToHandles.TryGetValue(assetKey, out var handle))
            {
                handle = YooAssets.LoadAssetAsync<T>(assetKey);
                m_KeyToHandles.Add(assetKey, handle);
            }

            if (!handle.IsDone)
                await handle.ToUniTask();

            return GetAssetAndAddReference<T>(assetKey, handle);
        }

        public async UniTask<byte[]> LoadRawAsync(string assetKey)
        {
            var handle = YooAssets.LoadRawFileAsync(assetKey);
            await handle.ToUniTask();

            var bytes = handle.GetRawFileData();
            handle.Release();
            return bytes;
        }

        public async UniTask<string> LoadRawTextAsync(string assetKey)
        {
            var handle = YooAssets.LoadRawFileAsync(assetKey);
            await handle.ToUniTask();

            var text = handle.GetRawFileText();
            handle.Release();
            return text;
        }

        public void Release(Object asset)
        {
            if (asset == null)
                return;

            var id = asset.GetInstanceID();
            if (!m_InstanceIDToKeys.TryGetValue(id, out var assetKey))
                return;

            RemoveReference(assetKey);
        }

        public void UnloadUnusedResources()
        {
            foreach (var (assetKey, value) in m_KeyToRefCounts)
            {
                if (value > 0)
                    continue;

                if (!m_KeyToHandles.TryGetValue(assetKey, out var handle))
                    continue;

                if (handle == null)
                    continue;

                var instanceID = handle.AssetObject.GetInstanceID();

                handle.Release();
                m_KeyToHandles.Remove(assetKey);
                m_InstanceIDToKeys.Remove(instanceID);
            }
        }

        public void UnloadResources()
        {
            foreach (var handle in m_KeyToHandles.Values)
                handle.Release();

            m_KeyToHandles.Clear();
            m_KeyToRefCounts.Clear();
            m_InstanceIDToKeys.Clear();
        }

        private T GetAssetAndAddReference<T>(string assetKey, AssetOperationHandle handle) where T : Object
        {
            var asset = handle.GetAssetObject<T>();
            if (asset == null)
                return null;

            var id = asset.GetInstanceID();
            m_InstanceIDToKeys[id] = assetKey;

            if (!m_KeyToRefCounts.TryGetValue(assetKey, out var refCount))
                refCount = 0;

            m_KeyToRefCounts[assetKey] = refCount + 1;

            return asset;
        }

        private void RemoveReference(string assetKey)
        {
            if (!m_KeyToRefCounts.TryGetValue(assetKey, out var refCount))
                return;

            if (refCount <= 0)
                return;

            m_KeyToRefCounts[assetKey] = refCount - 1;
        }
    }
}