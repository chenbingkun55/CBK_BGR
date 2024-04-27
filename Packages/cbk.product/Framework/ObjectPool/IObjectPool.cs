using System;

namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池接口 继承自对象池核心接口 并实现 IDisposable
    /// 在Dispose时将销毁所有未使用的实例
    /// </summary>
    public interface IObjectPool : IObjectPoolCore, IDisposable
    {
    }
}