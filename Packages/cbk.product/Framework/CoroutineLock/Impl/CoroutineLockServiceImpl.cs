using System;
using System.Collections.Generic;
using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.CoroutineLock.Exception;
using CBK.Framework.Reference;
using CBK.Framework.Update;

namespace CBK.Framework.CoroutineLock
{
    /// <summary>
    /// 协程锁服务实现
    /// </summary>
    internal sealed class CoroutineLockServiceImpl : IInitialize, IDisposable, ICoroutineLockService, IUpdate
    {
        private const int MaxLockCount = 100;

        [Inject]
        public IUpdateService UpdateService { get; set; }

        [Inject]
        public IReferenceService ReferenceService { get; set; }

        public void Initialize()
        {
            for (var type = 0u; type < _queues.Length; type++)
                _queues[type] = new CoroutineLockQueueMap(ReferenceService, type + 1, OnLockRelease);

            UpdateService.RegisterUpdate(this);
        }

        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);

            _nextFrameRun.Clear();
        }

        public UniTask<ICoroutineLock> Allocate(uint type, long key, CancellationToken token = default)
        {
            if (type == 0 || type > MaxLockCount)
                throw new CoroutineLockTypeOutOfRangeException(1, MaxLockCount, type);

            return _queues[type - 1].Allocate(key, token);
        }

        public UniTask<ICoroutineLock> Allocate(uint type, long key, uint priority, CancellationToken token = default)
        {
            if (type == 0 || type > MaxLockCount)
                throw new CoroutineLockTypeOutOfRangeException(1, MaxLockCount, type);

            return _queues[type - 1].Allocate(key, priority, token);
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            while (_nextFrameRun.Count > 0)
            {
                var (type, key) = _nextFrameRun.Dequeue();
                _queues[type - 1].Notify(key);
            }
        }

        private void OnLockRelease(CoroutineLock coroutineLock)
        {
            if (coroutineLock.Type == 0)
                return;

            _nextFrameRun.Enqueue((coroutineLock.Type, coroutineLock.Key));
        }

        private readonly Queue<(uint, long)> _nextFrameRun = new();
        private readonly CoroutineLockQueueMap[] _queues = new CoroutineLockQueueMap[MaxLockCount];
    }
}