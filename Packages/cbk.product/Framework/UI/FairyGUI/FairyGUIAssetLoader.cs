using System;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using FairyGUI.Dynamic;
using CBK.Framework.Res;
using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI UI资源加载器
    /// </summary>
    internal sealed class FairyGUIAssetLoader : IUIAssetLoader, IDisposable
    {
        [Inject]
        public IResLoader ResLoader { get; set; }

        private readonly string m_AssetKeyPrefix;

        public FairyGUIAssetLoader(string mAssetKeyPrefix)
        {
            m_AssetKeyPrefix = mAssetKeyPrefix;
        }

        public void Dispose()
        {
            ResLoader.Dispose();
        }

        private string GetPackageAssetKey(string packageName)
        {
            return $"{m_AssetKeyPrefix}{packageName}_fui";
        }

        private string GetResAssetKey(string packageName, string assetName)
        {
            return $"{m_AssetKeyPrefix}{packageName}_{assetName}";
        }

        public void LoadUIPackageBytesAsync(string packageName, LoadUIPackageBytesCallback callback)
        {
            ResLoader.LoadRawAsync(GetPackageAssetKey(packageName)).ContinueWith(bytes => callback?.Invoke(bytes, string.Empty)).Forget();
        }

        public void LoadUIPackageBytes(string packageName, out byte[] bytes, out string assetNamePrefix)
        {
            bytes = ResLoader.LoadRawSync(GetPackageAssetKey(packageName));
            assetNamePrefix = string.Empty;
        }

        public void LoadTextureAsync(string packageName, string assetName, string extension, LoadTextureCallback callback)
        {
            ResLoader.LoadAsync<Texture>(GetResAssetKey(packageName, assetName)).ContinueWith(texture => callback?.Invoke(texture)).Forget();
        }

        public void UnloadTexture(Texture texture)
        {
            ResLoader.Release(texture);
        }

        public void LoadAudioClipAsync(string packageName, string assetName, string extension, LoadAudioClipCallback callback)
        {
            ResLoader.LoadAsync<AudioClip>(GetResAssetKey(packageName, assetName)).ContinueWith(audioClip => callback?.Invoke(audioClip)).Forget();
        }

        public void UnloadAudioClip(AudioClip audioClip)
        {
            ResLoader.Release(audioClip);
        }
    }
}