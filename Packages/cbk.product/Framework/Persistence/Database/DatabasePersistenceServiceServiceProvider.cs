using CatLib;
using CatLib.Container;

namespace CBK.Framework.Persistence.Database
{
    /// <summary>
    /// 数据库持久化服务提供者
    /// </summary>
    internal sealed class DatabasePersistenceServiceServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IPersistenceService, DatabasePersistenceServiceImpl>();
            App.Bind<IPersistenceData>(() => PersistenceService.That.Allocate());
        }
    }
}