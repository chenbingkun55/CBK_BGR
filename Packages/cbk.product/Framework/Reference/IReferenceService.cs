using System;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用服务
    /// </summary>
    public interface IReferenceService
    {
        /// <summary>
        /// 对全局设置实例设置回收数量上限（小于0代表无上限, 默认无上限）
        /// </summary>
        int RecycledCountLimit { get; set; }

        /// <summary>
        /// 获取指定类型的实例
        /// </summary>
        IReference GetReference(Type type);

        /// <summary>
        /// 回收指定实例
        /// </summary>
        void Recycle(IReference reference);

        /// <summary>
        /// 清理指定类型实例的缓存到指定剩余数量 这些被清理的实例将被垃圾收集器回收
        /// </summary>
        void Clean(Type type, int remainCount = 0);

        /// <summary>
        /// 清理引用服务中所有实例缓存 这些被清理的实例将被垃圾收集器回收
        /// </summary>
        void CleanAll();

        /// <summary>
        /// 对指定类型设置工厂方法 当引用服务内无可复用实例时 将通过工厂方法构造新的实例 默认通过反射无参数的构造函数构造实例
        /// </summary>
        void SetFactory(Type type, Func<IReference> factory);

        /// <summary>
        /// 获取处于回收状态的指定类型实例个数
        /// </summary>
        int CountOfRecycledReferences(Type type);

        /// <summary>
        /// 对指定类型的实例设置回收数量上限（小于0代表无上限, 默认无上限），当超过上限后，多余的实例将被垃圾收集器收集
        /// </summary>
        void SetRecycledCountLimit(Type type, int limit);

        /// <summary>
        /// 获取指定类型的实例设置回收数量上限（小于0代表无上限）
        /// </summary>
        int GetRecycledCountLimit(Type type);
    }
}