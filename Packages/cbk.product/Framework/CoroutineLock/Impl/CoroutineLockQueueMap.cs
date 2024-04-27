using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.CoroutineLock
{
    internal sealed class CoroutineLockQueueMap
    {
        public CoroutineLockQueueMap(IReferenceService referenceService, uint type, Action<CoroutineLock> onLockRelease)
        {
            _referenceService = referenceService;
            _type = type;
            _onLockRelease = onLockRelease;
        }

        public UniTask<ICoroutineLock> Allocate(long key, CancellationToken token)
        {
            if (_dictQueues.TryGetValue(key, out var queue))
                return queue.Allocate(token);

            var newQueue = _referenceService.GetReference<CoroutineLockQueue>();
            newQueue.Type = _type;
            newQueue.Key = key;
            newQueue.OnDispose = _onLockRelease;
            _dictQueues.Add(key, newQueue);

            return newQueue.Allocate(token);
        }

        public UniTask<ICoroutineLock> Allocate(long key, uint priority, CancellationToken token)
        {
            if (!_dictQueues.TryGetValue(key, out var queue))
            {
                var newQueue = _referenceService.GetReference<PriorityCoroutineLockQueue>();
                newQueue.Type = _type;
                newQueue.Key = key;
                newQueue.OnDispose = _onLockRelease;
                _dictQueues.Add(key, newQueue);

                return newQueue.Allocate(priority, token);
            }
            else if (queue is IPriorityCoroutineLockQueue priorityQueue)
            {
                return priorityQueue.Allocate(priority, token);
            }
            else
            {
                Log.Error($"queue is not IPriorityCoroutineLockQueue {_type} {key}");
                return queue.Allocate(token);
            }
        }

        public void Notify(long key)
        {
            if (!_dictQueues.TryGetValue(key, out var queue))
                return;

            if (queue.isEmpty)
            {
                _dictQueues.Remove(key);
                queue.Recycle();
                return;
            }

            queue.Notify();
        }

        private readonly IReferenceService _referenceService;
        private readonly uint _type;
        private readonly Action<CoroutineLock> _onLockRelease;
        private readonly Dictionary<long, ICoroutineLockQueue> _dictQueues = new();
    }
}