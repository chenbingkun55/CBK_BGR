using CatLib;
using CatLib.Container;

namespace CBK.Framework.Database.SQLite
{
    /// <summary>
    /// 基于SQLite的数据库服务提供者
    /// </summary>
    internal sealed class SQLiteDatabaseServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IDatabaseService, SQLiteDatabaseServiceImpl>();
        }
    }
}