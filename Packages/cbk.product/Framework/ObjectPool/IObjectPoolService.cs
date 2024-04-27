namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池服务接口 继承自对象池核心接口 并提供获取对象池实例的方法供局部使用
    /// </summary>
    public interface IObjectPoolService : IObjectPoolCore
    {
        /// <summary>
        /// 创建对象池实例
        /// </summary>
        IObjectPool Allocate();
    }
}