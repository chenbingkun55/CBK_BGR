using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;

namespace CBK.Entry
{
    /// <summary>
    /// 加载器接口
    /// </summary>
    public interface IEntryLoader
    {
        /// <summary>
        /// 当前程序集版本号
        /// </summary>
        string CurrentAssemblyVersion { get; }

        /// <summary>
        /// 当前程序集列表
        /// </summary>
        IReadOnlyList<Assembly> CurrentAssembles { get; }

        /// <summary>
        /// 加载游戏逻辑程序集
        /// </summary>
        UniTask LoadGameAssemblies();
    }
}