using System;
using CatLib.Container;
using CatLib.EventDispatcher;
using CBK.Framework.Reference;

namespace CBK.Framework.Event
{
    /// <summary>
    /// 基于CatLib事件系统的事件服务实现
    /// </summary>
    internal sealed partial class CatLibEventServiceImpl : IEventService
    {
        [Inject]
        public IEventDispatcher EventDispatcher { get; set; }
        
        [Inject]
        public IReferenceService ReferenceService { get; set; }
        
        public bool AddListener(string eventName, EventHandler handler)
        {
            return EventDispatcher.AddListener(eventName, handler);
        }

        public bool RemoveListener(string eventName, EventHandler handler = null)
        {
            return EventDispatcher.RemoveListener(eventName, handler);
        }

        public EventHandler[] GetListeners(string eventName)
        {
            return EventDispatcher.GetListeners(eventName);
        }

        public bool HasListener(string eventName)
        {
            return EventDispatcher.HasListener(eventName);
        }

        public void Raise(string eventName, object sender, EventArgs e = null)
        {
            EventDispatcher.Raise(eventName, sender, e);
        }
    }
}