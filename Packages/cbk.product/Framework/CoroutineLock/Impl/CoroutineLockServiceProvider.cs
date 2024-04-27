using CatLib;
using CatLib.Container;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 协程锁服务提供者
    /// </summary>
    internal sealed class CoroutineLockServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();

            App.Singleton<ICoroutineLockService, CoroutineLockServiceImpl>();
        }
    }
}