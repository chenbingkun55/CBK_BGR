using System;
using CBK.Framework.Event;

namespace CBK.Framework.UI.FairyGUI
{
    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnClose阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnClose(string eventName, EventHandler handler)
        {
            DisposableGroupOnClose.Add(EventService.That.AddListenerAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnClose阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnClose<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnClose.Add(EventService.That.AddListenerAsDisposable<T>(handler));
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnPause阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnPause(string eventName, EventHandler handler)
        {
            DisposableGroupOnPause.Add(EventService.That.AddListenerAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnPause阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnPause<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnPause.Add(EventService.That.AddListenerAsDisposable<T>(handler));
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnRecycle阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnRecycle(string eventName, EventHandler handler)
        {
            DisposableGroupOnRecycle.Add(EventService.That.AddListenerAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnRecycle阶段自动解除监听
        /// </summary>
        protected void AddListenerDisposeOnRecycle<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnRecycle.Add(EventService.That.AddListenerAsDisposable<T>(handler));
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnClose阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnClose(string eventName, EventHandler handler)
        {
            DisposableGroupOnClose.Add(EventService.That.SubscribeAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnClose阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnClose<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnClose.Add(EventService.That.SubscribeAsDisposable<T>(handler));
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnPause阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnPause(string eventName, EventHandler handler)
        {
            DisposableGroupOnPause.Add(EventService.That.SubscribeAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnPause阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnPause<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnPause.Add(EventService.That.SubscribeAsDisposable<T>(handler));
        }
        
        /// <summary>
        /// 通过事件名添加事件监听 它将在OnRecycle阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnRecycle(string eventName, EventHandler handler)
        {
            DisposableGroupOnRecycle.Add(EventService.That.SubscribeAsDisposable(eventName, handler));
        }
        
        /// <summary>
        /// 通过事件参数类型添加事件监听 它将在OnRecycle阶段自动解除监听
        /// </summary>
        protected void SubscribeDisposeOnRecycle<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroupOnRecycle.Add(EventService.That.SubscribeAsDisposable<T>(handler));
        }
    }
}