using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using UnityEngine;

namespace CBK.Framework.Res.YooAsset
{
    internal partial class YooAssetResService
    {
        public IResLoader Allocate()
        {
            var resLoader = referenceService.GetReference<ResLoader>();
            resLoader.ResCore = this;
            return resLoader;
        }

        private sealed class ResLoader : AReference, IResLoader
        {
            public IResCore ResCore { get; set; }

            private readonly Dictionary<string, Object> m_KeyToAssets = new();
            private readonly Dictionary<string, int> m_KeyToRefCounts = new();
            private readonly Dictionary<int, string> m_InstanceIDToKeys = new();

            public void Dispose()
            {
                this.Recycle();
            }

            public override void OnRecycle()
            {
                UnloadResources();
                ResCore = null;
            }

            public T LoadSync<T>(string assetKey) where T : Object
            {
                if (!m_KeyToAssets.TryGetValue(assetKey, out var asset))
                {
                    asset = ResCore.LoadSync<T>(assetKey);
                    if (asset == null)
                        return null;

                    m_KeyToAssets[assetKey] = asset;
                    m_InstanceIDToKeys[asset.GetInstanceID()] = assetKey;
                }

                AddReference(assetKey);
                return (T)asset;
            }

            public byte[] LoadRawSync(string assetKey)
            {
                return ResCore.LoadRawSync(assetKey);
            }

            public string LoadRawTextSync(string assetKey)
            {
                return ResCore.LoadRawTextSync(assetKey);
            }

            public async UniTask<T> LoadAsync<T>(string assetKey) where T : Object
            {
                if (!m_KeyToAssets.TryGetValue(assetKey, out var asset))
                {
                    asset = await ResCore.LoadAsync<T>(assetKey);
                    if (asset == null)
                        return null;

                    m_KeyToAssets[assetKey] = asset;
                    m_InstanceIDToKeys[asset.GetInstanceID()] = assetKey;
                }

                AddReference(assetKey);
                return (T)asset;
            }

            public UniTask<byte[]> LoadRawAsync(string assetKey)
            {
                return ResCore.LoadRawAsync(assetKey);
            }

            public UniTask<string> LoadRawTextAsync(string assetKey)
            {
                return ResCore.LoadRawTextAsync(assetKey);
            }

            public void Release(Object asset)
            {
                if (asset == null)
                    return;

                var instanceId = asset.GetInstanceID();
                if (!m_InstanceIDToKeys.TryGetValue(instanceId, out var assetKey))
                    return;

                RemoveReference(assetKey);
            }

            public void UnloadUnusedResources()
            {
                foreach (var (assetKey, value) in m_KeyToRefCounts)
                {
                    if (value > 0)
                        continue;

                    if (!m_KeyToAssets.TryGetValue(assetKey, out var asset))
                        continue;

                    if (asset == null)
                        continue;

                    var instanceID = asset.GetInstanceID();

                    ResCore.Release(asset);
                    m_KeyToAssets.Remove(assetKey);
                    m_InstanceIDToKeys.Remove(instanceID);
                }
            }

            public void UnloadResources()
            {
                foreach (var asset in m_KeyToAssets.Values)
                    ResCore.Release(asset);

                m_KeyToAssets.Clear();
                m_KeyToRefCounts.Clear();
                m_InstanceIDToKeys.Clear();
            }

            private void AddReference(string assetKey)
            {
                if (!m_KeyToRefCounts.TryGetValue(assetKey, out var refCount))
                    refCount = 0;

                m_KeyToRefCounts[assetKey] = refCount + 1;
            }

            private void RemoveReference(string assetKey)
            {
                if (!m_KeyToRefCounts.TryGetValue(assetKey, out var refCount) || refCount <= 0)
                    return;

                m_KeyToRefCounts[assetKey] = refCount - 1;
            }
        }
    }
}