using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 协程锁管理队列
    /// </summary>
    internal interface ICoroutineLockQueue : IReference
    {
        /// <summary>
        /// 分配协程锁
        /// </summary>
        UniTask<ICoroutineLock> Allocate(CancellationToken token);

        /// <summary>
        /// 是否为空
        /// </summary>
        bool isEmpty { get; }

        /// <summary>
        /// 通知释放一个锁
        /// </summary>
        void Notify();
    }
}