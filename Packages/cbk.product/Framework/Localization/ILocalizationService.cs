using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化服务
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// 支持的本地化配置列表
        /// </summary>
        IReadOnlyList<ILocalizationConfig> Configs { get; }
        
        /// <summary>
        /// 当前生效中的本地化配置
        /// </summary>
        ILocalizationConfig CurrentConfig { get; }

        /// <summary>
        /// 本地化数据状态
        /// </summary>
        LocalizationStatus Status { get; }

        /// <summary>
        /// 切换本地化配置
        /// </summary>
        UniTask<int> SwitchConfig(int id);

        /// <summary>
        /// 加载当前的本地化数据 若已加载完成则直接返回，若当前正处于异步加载状态，则会等待加载完成 不会发起新的加载
        /// </summary>
        /// <returns>错误码 0代表成功</returns>
        UniTask<int> LoadAsync();

        /// <summary>
        /// 清理本地化数据
        /// </summary>
        void Clean();
    }
}