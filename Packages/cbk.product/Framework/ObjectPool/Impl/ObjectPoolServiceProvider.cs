using CatLib;
using CatLib.Container;

namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池服务提供者
    /// </summary>
    internal sealed class ObjectPoolServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IObjectPoolService, ObjectPoolServiceImpl>();
            App.Bind<IObjectPool>(() => ObjectPoolService.That.Allocate());
        }
    }
}