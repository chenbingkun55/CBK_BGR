using System;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用实例容器 用于存储引用实例对象与判断对象是否被回收
    /// </summary>
    public readonly struct ReferenceRef<T> : IDisposable where T : IReference
    {
        private readonly int _serialId;

        /// <summary>
        /// 是否为空引用
        /// </summary>
        public bool IsEmpty => Reference == null;

        /// <summary>
        /// 是否已经被回收
        /// </summary>
        public bool IsRecycled => Reference != null && Reference.SerialId != _serialId;

        /// <summary>
        /// 是否为有效引用
        /// </summary>
        public bool IsReferenceValid => Reference != null && Reference.SerialId == _serialId;

        /// <summary>
        /// 获取引用实例对象
        /// </summary>
        public T Reference { get; }

        public ReferenceRef(T reference) : this()
        {
            Reference = reference;
            _serialId = reference?.SerialId ?? 0;
        }

        /// <summary>
        /// 回收引用实例对象
        /// </summary>
        public void Recycle()
        {
            if (!IsReferenceValid)
                return;
            
            Reference.Recycle();
        }

        public void Dispose()
        {
            Recycle();
        }
    }
}