using CatLib;
using CatLib.Container;

namespace CBK.Framework.Scene.YooAsset
{
    /// <summary>
    /// YooAsset场景服务提供者
    /// </summary>
    internal sealed class YooAssetSceneServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<ISceneService, YooAssetSceneService>();
        }
    }
}