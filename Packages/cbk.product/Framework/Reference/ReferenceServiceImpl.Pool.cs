using System;
using System.Collections.Generic;

namespace CBK.Framework.Reference
{
    internal partial class ReferenceServiceImpl
    {
        private sealed class Pool : IDisposable
        {
            public int RecycledCountLimit { get; set; }

            public Func<IReference> Factory { get; set; }

            public int CountOfReferences => _references.Count;

            public IReferenceService ReferenceService { get; set; }

            public Pool(Type type)
            {
                _type = type;
                Factory = DefaultFactory;
                RecycledCountLimit = -1;
            }

            public void Dispose()
            {
                lock (_references)
                {
                    _references.Clear();
                }
            }

            public IReference GetReference()
            {
                lock (_references)
                {
                    var reference = _references.Count > 0 ? _references.Dequeue() : Factory();
                    reference.SerialId = ++_serialId;
                    reference.ReferenceService = ReferenceService;
                    return reference;
                }
            }

            public void Recycle(IReference reference)
            {
                reference.OnRecycle();
                reference.SerialId = 0;

                lock (_references)
                {
                    if (RecycledCountLimit >= 0 && _references.Count >= RecycledCountLimit)
                        return;

                    _references.Enqueue(reference);
                }
            }

            public void Clean(int remainCount = 0)
            {
                remainCount = Math.Max(0, remainCount);

                lock (_references)
                {
                    while (_references.Count > remainCount)
                        _references.Dequeue();
                }
            }

            private IReference DefaultFactory()
            {
                return (IReference)Activator.CreateInstance(_type);
            }

            private readonly Type _type;
            private int _serialId;
            private readonly Queue<IReference> _references = new Queue<IReference>();
        }

        private Pool GetPool(Type type)
        {
            lock (_pools)
            {
                if (_pools.TryGetValue(type, out var pool))
                    return pool;

                _pools[type] = pool = new Pool(type)
                {
                    RecycledCountLimit = RecycledCountLimit,
                    ReferenceService = this,
                };

                return pool;
            }
        }

        private readonly Dictionary<Type, Pool> _pools = new Dictionary<Type, Pool>();
    }
}