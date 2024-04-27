using CatLib;
using CatLib.Container;

namespace CBK.Framework.Update
{
    /// <summary>
    /// 帧更新服务提供者
    /// </summary>
    internal sealed class UpdateServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IUpdateService, UpdateServiceImpl>();
        }
    }
}