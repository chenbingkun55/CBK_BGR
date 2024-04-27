using System;
using CBK.Framework.Reference;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// Disposable包装器
    /// </summary>
    public sealed class DisposableWrapper : AReference, IDisposable
    {
        public IDisposable Disposable { get; set; }
        
        public override void OnRecycle()
        {
            Disposable = null;
        }

        public void Dispose()
        {
            Disposable?.Dispose();
            this.Recycle();
        }
    }
    
    public static class DisposableWrapperExtension
    {
        public static DisposableWrapper AsWrap(this IDisposable disposable)
        {
            var wrapper = ReferenceService.That.GetReference<DisposableWrapper>();
            wrapper.Disposable = disposable;
            return wrapper;
        }
    }
}