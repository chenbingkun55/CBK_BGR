using CatLib;
using CatLib.Container;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面服务提供者
    /// </summary>
    public sealed class UIServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IUIService, UIServiceImpl>();
        }
    }
}