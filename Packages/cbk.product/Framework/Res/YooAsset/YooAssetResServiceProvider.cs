using CatLib;
using CatLib.Container;

namespace CBK.Framework.Res.YooAsset
{
    /// <summary>
    /// YooAsset资源服务提供者
    /// </summary>
    internal sealed class YooAssetResServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IResService, YooAssetResService>();
            App.Bind<IResLoader>(() => ResService.That.Allocate());
        }
    }
}