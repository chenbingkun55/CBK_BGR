using CatLib;
using CatLib.Container;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 基于CatLib事件系统的事件服务提供者
    /// </summary>
    internal sealed class CatLibEventServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IEventService, CatLibEventServiceImpl>();
            App.Bind<IEventPool>(() => EventService.That.Allocate());
        }
    }
}