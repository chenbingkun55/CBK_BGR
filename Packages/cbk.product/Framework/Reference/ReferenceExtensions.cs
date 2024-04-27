using System;
using System.Collections.Generic;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用实例接口拓展方法
    /// </summary>
    public static class ReferenceExtensions
    {
        /// <summary>
        /// 将引用实例对象转换为引用实例容器
        /// </summary>
        public static ReferenceRef<T> AsRef<T>(this T reference) where T : IReference
        {
            return new ReferenceRef<T>(reference);
        }

        /// <summary>
        /// 通过指定的实例id判断对应对象是否已经被回收
        /// </summary>
        public static bool IsRecycled(this IReference reference, int serialId = 0)
        {
            if (reference.SerialId == 0)
                return true;

            if (serialId == 0)
                return false;
            
            return reference.SerialId != serialId;
        }

        /// <summary>
        /// 回收实例
        /// </summary>
        public static void Recycle(this IReference reference)
        {
            if (reference.ReferenceService == null)
            {
                reference.OnRecycle();
                return;
            }

            reference.ReferenceService.Recycle(reference);
        }

        /// <summary>
        /// 回收容器中的实例
        /// </summary>
        public static void RecycleItems<T>(this ICollection<T> collection) where T : IReference
        {
            foreach (var item in collection)
            {
                item.Recycle();
            }
            
            collection.Clear();
        }

        /// <summary>
        /// 回收容器中的实例
        /// </summary>
        public static void RecycleItems<TK, T>(this IDictionary<TK, T> collection) where T : IReference
        {
            foreach (var item in collection)
            {
                item.Value.Recycle();
            }
            
            collection.Clear();
        }

        /// <summary>
        /// 调用Dispose时令实例回收
        /// </summary>
        public static IDisposable AutoRecycle(this IReference reference)
        {
            var wrapper = ReferenceService.That.GetReference<AutoRecycleWrapper>();
            wrapper.reference = reference;
            return wrapper;
        }

        private sealed class AutoRecycleWrapper : IReference, IDisposable
        {
            public int SerialId { get; set; }
            public IReferenceService ReferenceService { get; set; }

            public IReference reference { get; set; }

            public void OnRecycle()
            {
                if (reference == null)
                    return;

                reference?.Recycle();
                reference = null;
            }

            public void Dispose()
            {
                this.Recycle();
            }
        }
    }
}