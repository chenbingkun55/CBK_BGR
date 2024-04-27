using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.CoroutineLock
{
    internal sealed class CoroutineLockQueue : AReference, ICoroutineLockQueue
    {
        public uint Type { get; set; }
        public long Key { get; set; }
        public Action<CoroutineLock> OnDispose { get; set; }

        public bool isEmpty => _queueWait.Count == 0;

        public async UniTask<ICoroutineLock> Allocate(CancellationToken token)
        {
            if (_currentLock == null)
            {
                _currentLock = Allocate();
                return _currentLock;
            }

            using var @ref = ReferenceService.GetReference<WaitCoroutineLock>().AsRef();
            _queueWait.Enqueue(@ref);

            var coroutineLock = await @ref.Reference.Allocate(token);
            if (coroutineLock == null)
            {
                if (!token.IsCancellationRequested)
                {
                    Log.Warning($"coroutineLock == null && !token.IsCancellationRequested {Type} {Key}");
                }

                // 创建一个锁实例返回 这个锁不占用队列 仅用于保证有返回值
                return ReferenceService.GetReference<CoroutineLock>();
            }

            _currentLock = coroutineLock;
            return _currentLock;
        }

        public void Notify()
        {
            while (_queueWait.Count > 0)
            {
                var @ref = _queueWait.Dequeue();

                if (!@ref.IsReferenceValid || @ref.Reference.IsCancelled)
                    continue;

                // 创建一个锁实例作为返回值
                @ref.Reference.SetResult(Allocate());
                break;
            }
        }

        public override void OnRecycle()
        {
            Type = 0;
            Key = 0;

            _currentLock = null;
            _queueWait.Clear();
        }

        private ICoroutineLock Allocate()
        {
            var coroutineLock = ReferenceService.GetReference<CoroutineLock>();
            coroutineLock.Type = Type;
            coroutineLock.Key = Key;
            coroutineLock.OnDispose = OnDispose;
            return coroutineLock;
        }

        private ICoroutineLock _currentLock;
        private readonly Queue<ReferenceRef<WaitCoroutineLock>> _queueWait = new();
    }
}