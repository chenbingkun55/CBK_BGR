using Cysharp.Threading.Tasks;

namespace CBK.Framework.Configuration
{
    /// <summary>
    /// 配置表服务
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// 配置状态
        /// </summary>
        ConfigurationStatus Status { get; }
        
        /// <summary>
        /// 异步加载配置表 若已加载完成则直接返回，若当前正处于异步加载状态，则会等待加载完成 不会发起新的加载
        /// </summary>
        /// <returns>错误码 0代表成功</returns>
        UniTask<int> LoadAsync();

        /// <summary>
        /// 获取指定配置表实例
        /// </summary>
        T Get<T>() where T : class, IConfiguration;

        /// <summary>
        /// 清理配置表数据
        /// </summary>
        void Clean();
    }
}