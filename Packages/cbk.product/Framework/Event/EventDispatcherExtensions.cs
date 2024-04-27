using System;
using CatLib.EventDispatcher;
using CBK.Framework.Reference;
using CBK.Framework.Utility;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 事件派发器拓展方法
    /// </summary>
    public static class EventDispatcherExtensions
    {
        /// <summary>
        /// 通过事件参数类型添加事件监听
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="handler">The event listener.</param>
        /// <returns>True if the handler added. otherwise false if handler already exists.</returns>
        public static bool AddListener<T>(this IEventDispatcher dispatcher, EventHandler handler) where T : EventArgs
        {
            return dispatcher.AddListener(EventUtility.GetEventName<T>(), handler);
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 并返回一个可释放的对象 释放时自动移除事件监听
        /// </summary>
        public static IDisposable AddListenerAsDisposable(this IEventDispatcher dispatcher, string eventName, EventHandler handler)
        {
            dispatcher.AddListener(eventName, handler);
            
            void Dispose()
            {
                dispatcher.RemoveListener(eventName, handler);
            }

            return ((Action)Dispose).AsDisposable();
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 并返回一个可释放的对象 释放时自动移除事件监听
        /// </summary>
        public static IDisposable AddListenerAsDisposable<T>(this IEventDispatcher dispatcher, EventHandler handler) where T : EventArgs
        {
            return dispatcher.AddListenerAsDisposable(EventUtility.GetEventName<T>(), handler);
        }

        /// <summary>
        /// 通过事件参数类型移除事件监听
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="handler">Remove the specified event listener, otherwise remove all listeners under the event.</param>
        /// <returns>True if removed the listener.</returns>
        public static bool RemoveListener<T>(this IEventDispatcher dispatcher, EventHandler handler = null) where T : EventArgs
        {
            return dispatcher.RemoveListener(typeof(T).FullName, handler);
        }
        
        /// <summary>
        /// 派发事件 通过事件参数实例获取其绑定的事件名
        /// </summary>
        public static void Raise(this IEventDispatcher dispatcher, object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)
                throw new Exception($"事件参数为空");
            
            dispatcher.Raise(eventArgs.GetEventName(), sender, eventArgs);
        }
        
        /// <summary>
        /// 通过事件名添加事件监听
        /// </summary>
        public static bool Subscribe(this IEventDispatcher dispatcher, string eventName, EventHandler handler)
        {
            return dispatcher.AddListener(eventName, handler);
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="handler">The event listener.</param>
        /// <returns>True if the handler added. otherwise false if handler already exists.</returns>
        public static bool Subscribe<T>(this IEventDispatcher dispatcher, EventHandler handler) where T : EventArgs
        {
            return dispatcher.AddListener<T>(handler);
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 并返回一个可释放的对象 释放时自动移除事件监听
        /// </summary>
        public static IDisposable SubscribeAsDisposable(this IEventDispatcher dispatcher, string eventName, EventHandler handler)
        {
            return dispatcher.AddListenerAsDisposable(eventName, handler);
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 并返回一个可释放的对象 释放时自动移除事件监听
        /// </summary>
        public static IDisposable SubscribeAsDisposable<T>(this IEventDispatcher dispatcher, EventHandler handler) where T : EventArgs
        {
            return dispatcher.AddListenerAsDisposable<T>(handler);
        }
        
        /// <summary>
        /// 通过事件名移除事件监听
        /// </summary>
        public static bool Unsubscribe(this IEventDispatcher dispatcher, string eventName, EventHandler handler)
        {
            return dispatcher.RemoveListener(eventName, handler);
        }

        /// <summary>
        /// 通过事件参数类型移除事件监听
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="handler">Remove the specified event listener, otherwise remove all listeners under the event.</param>
        /// <returns>True if removed the listener.</returns>
        public static bool Unsubscribe<T>(this IEventDispatcher dispatcher, EventHandler handler = null) where T : EventArgs
        {
            return dispatcher.RemoveListener<T>(handler);
        }
        
        /// <summary>
        /// 派发事件 通过事件参数实例获取其绑定的事件名
        /// </summary>
        public static void Raise<T>(this IEventDispatcher dispatcher, object sender, T eventArgs) where T : EventArgs, IReference
        {
            if (eventArgs == null)
                throw new Exception("事件参数为空");
            
            if (eventArgs.SerialId == 0)
                throw new Exception($"事件参数{eventArgs.GetType().FullName}未初始化");

            using var _ = eventArgs.AutoRecycle();
            dispatcher.Raise(sender, (EventArgs)eventArgs);
        }
        
        /// <summary>
        /// 派发事件 构造一个指定类型的事件参数实例进行派发
        /// </summary>
        public static void Raise<T>(this IEventDispatcher dispatcher, object sender) where T : EventArgs, IReference, new()
        {
            var eventArgs = ReferenceService.That.GetReference<T>();
            dispatcher.Raise(sender, eventArgs);
        }
        
        /// <summary>
        /// 派发事件 构造一个指定类型的事件参数实例进行派发
        /// </summary>
        public static void Raise<T>(this IEventDispatcher dispatcher, object sender, Action<T> decor) where T : EventArgs, IReference, new()
        {
            var eventArgs = ReferenceService.That.GetReference<T>();
            decor.Invoke(eventArgs);
            dispatcher.Raise(sender, eventArgs);
        }

        /// <summary>
        /// 派发事件 通过事件参数实例获取其绑定的事件名
        /// </summary>
        public static void Raise(this IEventPool eventPool, object sender, EventArgs eventArgs)
        {
            if (eventArgs == null)
                throw new Exception($"事件参数为空");
            
            eventPool.Raise(eventArgs.GetEventName(), sender, eventArgs);
        }
    }
}