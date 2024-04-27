using CatLib;
using CatLib.Container;

namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器服务提供者
    /// </summary>
    internal sealed class TimerServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<ITimerService, TimerServiceImpl>();
            App.Bind<ITimer>(() => TimerService.That.Allocate());
        }
    }
}