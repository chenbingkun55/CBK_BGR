using System;

namespace CBK.Framework.Utility
{
    public sealed class DisposableLock : IDisposable
    {
        public bool IsLocked { get; private set; }

        public IDisposable Lock()
        {
            if (IsLocked)
                throw new InvalidOperationException("DisposableLock is locked");
            
            IsLocked = true;
            return this;
        }
        
        public void Dispose()
        {
            IsLocked = false;
        }
    }
}