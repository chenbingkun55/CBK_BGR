using Cysharp.Threading.Tasks;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化实例接口
    /// </summary>
    public interface ILocalization
    {
        /// <summary>
        /// 异步加载对应语言的本地化数据
        /// </summary>
        UniTask<int> LoadAsync(ILocalizationConfig config);

        /// <summary>
        /// 清理本地化数据
        /// </summary>
        void Clean();
    }
}