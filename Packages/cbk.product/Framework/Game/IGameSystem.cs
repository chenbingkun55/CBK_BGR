using System;
using CatLib;
using CBK.Framework.Core;
using CBK.Framework.Event;
using CBK.Framework.Utility;

namespace CBK.Framework.Game
{
    /// <summary>
    /// 玩法系统接口
    /// </summary>
    public interface IGameSystem
    {
    }

    /// <summary>
    /// 玩法系统接口
    /// </summary>
    public abstract class AGameSystem<T> : Facade<T>, IGameSystem, IInitialize, IDisposable where T : AGameSystem<T>
    {
        protected DisposableGroup DisposableGroup => m_DisposableGroup ??= new DisposableGroup();
        private DisposableGroup m_DisposableGroup;

        public void Initialize()
        {
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
            m_DisposableGroup?.Dispose();
            m_DisposableGroup = null;
        }

        protected abstract void OnInitialize();
        protected abstract void OnDispose();
        
        protected void Subscribe<TEvent>(EventHandler handler) where TEvent : EventArgs
        {
            DisposableGroup.Add(EventService.That.SubscribeAsDisposable<TEvent>(handler));
        }
    }
}