using System;
using CatLib.Container;
using FairyGUI.Dynamic;
using CBK.Framework.Core;
using CBK.Framework.Res;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI UI资源配置器
    /// </summary>
    internal sealed class FairyGUIAssetManagerConfiguration : IInitialize, IDisposable, IUIAssetManagerConfiguration
    {
        [Inject]
        public IUIAssetLoader AssetLoader { get; set; }

        public bool UnloadUnusedUIPackageImmediately { get; }

        public IUIPackageHelper PackageHelper { get; private set; }

        [Inject]
        public IResLoader ResLoader { get; set; }

        public FairyGUIAssetManagerConfiguration(string mMappingAssetKey, bool unloadUnusedUIPackageImmediately)
        {
            m_MappingAssetKey = mMappingAssetKey;
            UnloadUnusedUIPackageImmediately = unloadUnusedUIPackageImmediately;
        }

        public void Initialize()
        {
            PackageHelper = ResLoader.LoadSync<UIPackageMapping>(m_MappingAssetKey);
        }

        public void Dispose()
        {
            ResLoader.Dispose();
        }

        private readonly string m_MappingAssetKey;
    }
}