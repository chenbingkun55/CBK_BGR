using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 带优先级排序功能的协程锁管理队列
    /// </summary>
    internal interface IPriorityCoroutineLockQueue : ICoroutineLockQueue
    {
        /// <summary>
        /// 分配协程锁
        /// </summary>
        UniTask<ICoroutineLock> Allocate(uint priority, CancellationToken token);
    }
}