using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.Business
{
    /// <summary>
    /// 需要进行加载的接口
    /// </summary>
    public interface ILoadable
    {
        /// <summary>
        /// 异步加载
        /// </summary>
        UniTask LoadAsync(CancellationToken token);
    }

    /// <summary>
    /// 要求使用顺序加载的接口（即指定不允许并行加载） 业务流程会先按顺序执行完顺序加载后，才会开始剩下的并行加载
    /// </summary>
    public interface ISequenceLoadable : ILoadable
    {
    }
}