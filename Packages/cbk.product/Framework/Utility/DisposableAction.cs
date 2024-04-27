using System;
using CBK.Framework.Reference;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// Dispose触发指定操作的包装器
    /// </summary>
    public sealed class DisposableAction : AReference, IDisposable
    {
        /// <summary>
        /// 在Dispose时触发的操作
        /// </summary>
        public Action Action { get; set; }
        
        public void Dispose()
        {
            Action?.Invoke();
            this.Recycle();
        }

        public override void OnRecycle()
        {
            Action = null;
        }
    }

    public static class DisposableActionExtensions
    {
        /// <summary>
        /// 令指定操作在Dispose时触发
        /// </summary>
        public static IDisposable AsDisposable(this Action action)
        {
            var wrapper = ReferenceService.That.GetReference<DisposableAction>();
            wrapper.Action = action;
            return wrapper;
        }
    }
}