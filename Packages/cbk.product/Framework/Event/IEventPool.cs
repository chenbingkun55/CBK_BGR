using System;
using CBK.Framework.Update;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 事件池接口。 用于保障线程安全地分发事件
    /// </summary>
    public interface IEventPool : IDisposable, IUpdate
    {
        /// <summary>
        /// 获取事件数量。
        /// </summary>
        int EventCount { get; }
        
        /// <summary>
        /// 每帧事件分发时间限制(毫秒)。
        /// </summary>
        long EventDispatchTimeLimit { get; set; }
        
        /// <summary>
        /// 注册事件拦截器。
        /// </summary>
        void RegisterInterceptor(EventInterceptor interceptor);
        
        /// <summary>
        /// 取消注册事件拦截器。
        /// </summary>
        void UnregisterInterceptor(EventInterceptor interceptor);
        
        /// <summary>
        /// 派发事件。
        /// </summary>
        void Raise(string eventName, object sender, EventArgs e);
        
        /// <summary>
        /// 清理事件。
        /// </summary>
        void Clear();
    }
}