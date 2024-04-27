using CatLib;
using CatLib.Container;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机服务提供者
    /// </summary>
    internal sealed class FsmServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IFsmService, FsmServiceImpl>();
        }
    }
}