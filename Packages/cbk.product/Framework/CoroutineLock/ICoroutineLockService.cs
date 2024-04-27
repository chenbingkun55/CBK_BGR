using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 协程锁服务接口
    /// </summary>
    public interface ICoroutineLockService
    {
        /// <summary>
        /// 分配协程锁 若锁处于占用状态 则会等待
        /// </summary>
        UniTask<ICoroutineLock> Allocate(uint type, long key, CancellationToken token = default);

        /// <summary>
        /// 分配带优先级的协程锁 若锁处于占用状态 则会等待 这种协程锁至少会延迟一帧执行
        /// </summary>
        UniTask<ICoroutineLock> Allocate(uint type, long key, uint priority, CancellationToken token = default);
    }
}