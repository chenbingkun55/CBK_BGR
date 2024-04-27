using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using CBK.Framework.Extensions;
using CBK.Framework.Reference;
using System.Threading;

namespace CBK.Framework.CoroutineLock
{
    internal sealed class PriorityCoroutineLockQueue : AReference, IPriorityCoroutineLockQueue
    {
        public uint Type { get; set; }
        public long Key { get; set; }
        public Action<CoroutineLock> OnDispose { get; set; }
        public bool isEmpty => _queue.Count == 0;

        public override void OnRecycle()
        {
            Type = 0;
            Key = 0;

            _currentLock = null;
            _queue.Clear();
        }

        public UniTask<ICoroutineLock> Allocate(CancellationToken token)
        {
            return Allocate(0, token);
        }

        public async UniTask<ICoroutineLock> Allocate(uint priority, CancellationToken token)
        {
            using var @ref = ReferenceService.GetReference<WaitCoroutineLock>().AsRef();
            @ref.Reference.Priority = priority;

            _queue.AddSorted(@ref, Comparison);

            if (_currentLock != null)
                return await PriorityWaitAsyncInner(@ref.Reference, token);

            // 延迟一帧执行
            await UniTask.Yield();

            if (_currentLock != null || _queue.IndexOf(@ref) != _queue.Count - 1)
                return await PriorityWaitAsyncInner(@ref.Reference, token);

            // 当前没有锁实例 并且当前等待锁实例是队列中最后一个
            _queue.RemoveAt(_queue.Count - 1);
            _currentLock = Allocate();
            return _currentLock;
        }

        public void Notify()
        {
            while (_queue.Count > 0)
            {
                var @ref = _queue.Last();
                _queue.RemoveAt(_queue.Count - 1);

                if (!@ref.IsReferenceValid || @ref.Reference.IsCancelled)
                    continue;

                // 创建一个锁实例作为返回值
                @ref.Reference.SetResult(Allocate());
                break;
            }
        }

        private int Comparison(ReferenceRef<WaitCoroutineLock> x, ReferenceRef<WaitCoroutineLock> y)
        {
            var compare = x.IsReferenceValid.CompareTo(y.IsReferenceValid);
            if (compare != 0)
                return compare;
            
            // 优先级大的排在后面
            return x.Reference.Priority.CompareTo(y.Reference.Priority);
        }

        private async UniTask<ICoroutineLock> PriorityWaitAsyncInner(WaitCoroutineLock waitCoroutineLock, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return ReferenceService.GetReference<CoroutineLock>();

            var coroutineLock = await waitCoroutineLock.Allocate(token);
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

        private ICoroutineLock Allocate()
        {
            var coroutineLock = ReferenceService.GetReference<CoroutineLock>();
            coroutineLock.Type = Type;
            coroutineLock.Key = Key;
            coroutineLock.OnDispose = OnDispose;
            return coroutineLock;
        }

        private ICoroutineLock _currentLock;
        private readonly List<ReferenceRef<WaitCoroutineLock>> _queue = new();
    }
}