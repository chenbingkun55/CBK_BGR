using System;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 事件拦截器
    /// </summary>
    public delegate bool EventInterceptor(string eventName, object sender, EventArgs e);

    /// <summary>
    /// 事件拦截器接口
    /// </summary>
    public interface IEventInterceptor
    {
        /// <summary>
        /// 拦截事件
        /// </summary>
        bool Intercept(string eventName, object sender, EventArgs e);
    }
    
    public static class EventInterceptorExtensions
    {
        /// <summary>
        /// 将事件拦截器转换为事件拦截器委托
        /// </summary>
        public static EventInterceptor ToEventInterceptor(this IEventInterceptor interceptor)
        {
            return interceptor.Intercept;
        }

        /// <summary>
        /// 注册事件拦截器。
        /// </summary>
        public static void RegisterInterceptor(this IEventPool eventPool, IEventInterceptor interceptor)
        {
            eventPool.RegisterInterceptor(interceptor.ToEventInterceptor());
        }

        /// <summary>
        /// 取消注册事件拦截器。
        /// </summary>
        public static void UnregisterInterceptor(this IEventPool eventPool, IEventInterceptor interceptor)
        {
            eventPool.UnregisterInterceptor(interceptor.ToEventInterceptor());
        }
    }
}