using System;
using System.Threading;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Event;
using CBK.Framework.Utility;

namespace CBK.Framework.Business
{
    /// <summary>
    /// 视图控制器抽象类 提供视图控制器的一些通用能力
    /// </summary>
    public abstract class AViewController : IViewController
    {
        [Inject]
        public IEventService EventService { get; set; }
        
        public void Initialize()
        {
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();

            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource.Dispose();
                m_CancellationTokenSource = null;
            }

            m_DisposableGroup?.Dispose();
        }

        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        #region [Event Extensions]

        /// <summary>
        /// 订阅值变更委托 并且会在视图控制器Dispose时自动取消订阅
        /// </summary>
        protected void Subscribe<T>(ObservableVariable<T> variable, ObservableVariable<T>.ValueChangedHandler handler, bool callImmediately = false)
        {
            DisposableGroup.Add(variable.SubscribeAsDisposable(handler, callImmediately));
        }

        /// <summary>
        /// 订阅事件 并且会在视图控制器Dispose时自动取消订阅
        /// </summary>
        protected void Subscribe(string eventName, EventHandler handler)
        {
            DisposableGroup.Add(EventService.AddListenerAsDisposable(eventName, handler));
        }

        /// <summary>
        /// 订阅事件 并且会在视图控制器Dispose时自动取消订阅
        /// </summary>
        protected void Subscribe<T>(EventHandler handler) where T : EventArgs
        {
            DisposableGroup.Add(EventService.AddListenerAsDisposable<T>(handler));
        }

        #endregion
        
        protected DisposableGroup DisposableGroup => (m_DisposableGroup ??= new DisposableGroup());
        private DisposableGroup m_DisposableGroup;

        protected CancellationToken CancellationToken => (m_CancellationTokenSource ??= new CancellationTokenSource()).Token;
        private CancellationTokenSource m_CancellationTokenSource;
    }
}