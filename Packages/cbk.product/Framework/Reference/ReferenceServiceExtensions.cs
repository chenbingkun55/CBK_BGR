using System;
using CBK.Framework.Collections;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用服务拓展方法
    /// </summary>
    public static class ReferenceServiceExtensions
    {
        /// <summary>
        /// 获取指定类型的实例
        /// </summary>
        public static T GetReference<T>(this IReferenceService service) where T : IReference, new()
        {
            return (T)service.GetReference(typeof(T));
        }
        
        /// <summary>
        /// 获取指定类型的实例
        /// </summary>
        public static T GetReference<T>(this IReferenceService service, Action<T> decor) where T : IReference, new()
        {
            var instance = service.GetReference<T>();
            decor?.Invoke(instance);
            return instance;
        }

        /// <summary>
        /// 清理指定类型实例的缓存到指定剩余数量 这些被清理的实例将被垃圾收集器回收
        /// </summary>
        public static void Clean<T>(this IReferenceService service, int remainCount = 0) where T : IReference, new()
        {
            service.Clean(typeof(T), remainCount);
        }

        /// <summary>
        /// 对指定类型设置工厂方法 当引用服务内无可复用实例时 将通过工厂方法构造新的实例 默认通过反射无参数的构造函数构造实例
        /// </summary>
        public static void SetFactory<T>(this IReferenceService service, Func<IReference> factory) where T : IReference, new()
        {
            service.SetFactory(typeof(T), factory);
        }

        /// <summary>
        /// 获取处于回收状态的指定类型实例个数
        /// </summary>
        public static int CountOfRecycledReferences<T>(this IReferenceService service) where T : IReference, new()
        {
            return service.CountOfRecycledReferences(typeof(T));
        }

        /// <summary>
        /// 对指定类型的实例设置回收数量上限（小于0代表无上限, 默认无上限），当超过上限后，多余的实例将被垃圾收集器收集
        /// </summary>
        public static void SetRecycledCountLimit<T>(this IReferenceService service, int limit) where T : IReference, new()
        {
            service.SetRecycledCountLimit(typeof(T), limit);
        }

        /// <summary>
        /// 获取指定类型的实例设置回收数量上限（小于0代表无上限）
        /// </summary>
        public static int GetRecycledCountLimit<T>(this IReferenceService service) where T : IReference, new()
        {
            return service.GetRecycledCountLimit(typeof(T));
        }
        
        /// <summary>
        /// 获取指定类型的引用列表实例
        /// </summary>
        public static ListReference<T> GetListReference<T>(this IReferenceService service)
        {
            return service.GetReference<ListReference<T>>();
        }
    }
}