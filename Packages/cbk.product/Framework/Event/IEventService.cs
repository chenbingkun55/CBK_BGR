using CatLib.EventDispatcher;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 事件服务
    /// </summary>
    public interface IEventService : IEventDispatcher
    {
        /// <summary>
        /// 分配事件池实例, 用于保障线程安全地分发事件 当事件池不再使用时, 需要调用Dispose方法进行释放
        /// </summary>
        IEventPool Allocate();
    }
}