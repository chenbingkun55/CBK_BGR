using System.Runtime.Serialization;
using CatJson;
using CBK.Framework.Route;
using SimpleSQL;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用实例接口 实现该接口的类将允许通过引用服务进行创建与回收
    /// </summary>
    public interface IReference
    {
        /// <summary>
        /// 实例的唯一ID 由引用服务进行维护 禁止手动对其赋值
        /// </summary>
        int SerialId { get; set; }

        /// <summary>
        /// 引用服务 由引用服务进行维护 禁止手动对其赋值
        /// </summary>
        IReferenceService ReferenceService { get; set; }

        /// <summary>
        /// 当实例被引用服务回收时 将调用该接口
        /// </summary>
        void OnRecycle();
    }

    /// <summary>
    /// 引用实例抽象类 派生自该类的子类将允许通过引用服务进行创建与回收
    /// </summary>
    public abstract class AReference : IReference
    {
        [PropertyIgnore]
        [Ignore]
        [JsonIgnore]
        [IgnoreDataMember]
        public int SerialId { get; set; }

        [PropertyIgnore]
        [Ignore]
        [JsonIgnore]
        [IgnoreDataMember]
        public IReferenceService ReferenceService { get; set; }

        public abstract void OnRecycle();
    }
}