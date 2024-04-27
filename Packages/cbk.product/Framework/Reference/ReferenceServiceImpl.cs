using System;
using CBK.Framework.Core;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用服务实现类
    /// </summary>
    internal sealed partial class ReferenceServiceImpl : IInitialize, IDisposable, IReferenceService
    {
        public int RecycledCountLimit
        {
            get => _recycledCountLimit;
            set
            {
                if (_recycledCountLimit == value)
                    return;

                lock (_pools)
                {
                    foreach (var pool in _pools.Values)
                    {
                        if (pool.RecycledCountLimit != _recycledCountLimit)
                            continue;

                        pool.RecycledCountLimit = value;
                    }
                }

                _recycledCountLimit = value;
            }
        }

        public void Initialize()
        {
            RecycledCountLimit = -1;
        }

        public void Dispose()
        {
            lock (_pools)
            {
                foreach (var pool in _pools.Values)
                    pool.Dispose();

                _pools.Clear();
            }
        }

        public IReference GetReference(Type type)
        {
            return GetPool(type).GetReference();
        }

        public void Recycle(IReference reference)
        {
            if (reference == null)
                return;

            if (reference.SerialId == 0)
            {
                // 没有序列化id 可能已经在对象池里 未防止出现异常 这里不让其回收 直接调用Recycle
                reference.OnRecycle();
                return;
            }

            GetPool(reference.GetType()).Recycle(reference);
        }

        public void Clean(Type type, int remainCount = 0)
        {
            GetPool(type).Clean(remainCount);
        }

        public void CleanAll()
        {
            lock (_pools)
            {
                foreach (var pool in _pools.Values)
                    pool.Clean();
            }
        }

        public void SetFactory(Type type, Func<IReference> factory)
        {
            GetPool(type).Factory = factory;
        }

        public int CountOfRecycledReferences(Type type)
        {
            return GetPool(type).CountOfReferences;
        }

        public void SetRecycledCountLimit(Type type, int limit)
        {
            GetPool(type).RecycledCountLimit = limit;
        }

        public int GetRecycledCountLimit(Type type)
        {
            return GetPool(type).RecycledCountLimit;
        }

        private int _recycledCountLimit;
    }
}