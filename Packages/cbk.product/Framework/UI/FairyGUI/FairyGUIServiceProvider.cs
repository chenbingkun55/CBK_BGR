using CatLib;
using CatLib.Container;
using FairyGUI.Dynamic;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 服务提供者。
    /// </summary>
    internal sealed class FairyGUIServiceProvider : ServiceProvider
    {
        private readonly string m_MappingAssetKey;
        private readonly string m_AssetKeyPrefix;
        private readonly bool m_UnloadUnusedUIPackageImmediately;

        public FairyGUIServiceProvider(string mMappingAssetKey, string mAssetKeyPrefix, bool mUnloadUnusedUIPackageImmediately)
        {
            m_MappingAssetKey = mMappingAssetKey;
            m_AssetKeyPrefix = mAssetKeyPrefix;
            m_UnloadUnusedUIPackageImmediately = mUnloadUnusedUIPackageImmediately;
        }

        public override void Register()
        {
            base.Register();
            App.Bind<IUIGroupHelper, FairyGUIGroupHelper>();
            App.Singleton<IUIFormHelper, FairyGUIFormHelper>();
            App.Singleton<IUIAssetLoader>(() => new FairyGUIAssetLoader(m_AssetKeyPrefix));
            App.Singleton<IUIAssetManagerConfiguration>(() => new FairyGUIAssetManagerConfiguration(m_MappingAssetKey, m_UnloadUnusedUIPackageImmediately));
            App.Singleton<IUIScreenAdaptorService, FairyGUIScreenAdaptorService>();
        }
    }

    public static class FairyGUIExtension
    {
        /// <summary>
        /// 使用FairyGUI界面服务
        /// </summary>
        public static void UseFairyGUI(this IApplication application,
            string mappingAssetKey = nameof(UIPackageMapping),
            string assetKeyPrefix = "UI_",
            bool unloadUnusedUIPackageImmediately = false)
        {
            application.Register(new FairyGUIServiceProvider(mappingAssetKey, assetKeyPrefix, unloadUnusedUIPackageImmediately));
        }
    }
}