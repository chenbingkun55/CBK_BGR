using System;
using CBK.Framework.Reference;

namespace CBK.Framework.CoroutineLock
{
    internal sealed class CoroutineLock : AReference, ICoroutineLock
    {
        public uint Type { get; set; }
        public long Key { get; set; }
        public Action<CoroutineLock> OnDispose { get; set; }

        public override void OnRecycle()
        {
            Type = 0;
            Key = 0;
            OnDispose = null;
        }

        public void Dispose()
        {
            OnDispose?.Invoke(this);
            this.Recycle();
        }
    }
}