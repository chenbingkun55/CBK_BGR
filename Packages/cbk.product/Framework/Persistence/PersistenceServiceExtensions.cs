namespace CBK.Framework.Persistence
{
    public static class PersistenceServiceExtensions
    {
        private const string DefaultPersistenceIdentifier = "DevicePersistence";

        /// <summary>
        /// 获取默认的持久化数据实例 该实例无需手动Dispose
        /// </summary>
        public static IPersistenceData Allocate(this IPersistenceService persistenceService)
        {
            return persistenceService.Allocate(DefaultPersistenceIdentifier);
        }
    }
}