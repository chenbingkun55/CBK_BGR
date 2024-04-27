namespace CBK.Framework.Database
{
    /// <summary>
    /// 数据库服务 对外提供获取数据库连接的接口
    /// </summary>
    public interface IDatabaseService
    {
        /// <summary>
        /// 获取数据库连接 当不需要使用时需要调用Dispose接口释放
        /// </summary>
        IDatabaseConnection Allocate(string identifier);

        /// <summary>
        /// 清理未使用的连接
        /// </summary>
        void CleanUnusedConnections();
    }
}