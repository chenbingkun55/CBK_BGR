using System;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// 借助IDisposable实现的引用计数器 调用AddRef令计数+1 在Dispose时计数-1
    /// </summary>
    public sealed class DisposableRefCounter : IDisposable
    {
        public event Action OnRefZero;
        public event Action OnRefNotZero;
        
        public int RefCount { get; private set; }
        
        public bool AnyRef => RefCount > 0;

        public IDisposable AddRef()
        {
            ++RefCount;
            
            if (RefCount == 1)
                OnRefNotZero?.Invoke();
            
            return this;
        }

        public void Dispose()
        {
            if (RefCount == 0)
                return;
                
            --RefCount;
            
            if (RefCount == 0)
                OnRefZero?.Invoke();
        }
        
        public void Reset()
        {
            if (RefCount == 0)
                return;
            
            RefCount = 0;
            OnRefZero?.Invoke();
        }
    }
}