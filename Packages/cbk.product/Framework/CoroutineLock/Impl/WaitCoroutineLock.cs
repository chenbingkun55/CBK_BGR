using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using System.Threading;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 等待协程锁
    /// </summary>
    internal sealed class WaitCoroutineLock : AReference
    {
        public uint Priority { get; set; }

        public bool IsCancelled { get; private set; }

        public override void OnRecycle()
        {
            _coroutineLock = null;
            IsCancelled = false;
            Priority = 0;
        }

        public async UniTask<ICoroutineLock> Allocate(CancellationToken token)
        {
            await UniTask.WaitUntil(ExistsLock, cancellationToken: token).SuppressCancellationThrow();

            IsCancelled = token.IsCancellationRequested && _coroutineLock == null;

            return _coroutineLock;
        }

        private bool ExistsLock()
        {
            return _coroutineLock != null;
        }

        public void SetResult(ICoroutineLock coroutineLock)
        {
            _coroutineLock = coroutineLock;
        }

        private ICoroutineLock _coroutineLock;
    }
}