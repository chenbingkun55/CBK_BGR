namespace CBK.Framework.Persistence
{
    /// <summary>
    /// 持久化服务
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// 获取指定的持久化数据实例 当不需要使用时需要调用Dispose接口释放
        /// </summary>
        IPersistenceData Allocate(string identifier);
    }
}