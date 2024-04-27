using System;
using System.Collections.Generic;
using System.Diagnostics;
using CBK.Framework.Reference;

namespace CBK.Framework.Event
{
    internal sealed partial class CatLibEventServiceImpl
    {
        public IEventPool Allocate()
        {
            var pool = ReferenceService.GetReference<EventPool>();
            pool.EventService = this;
            return pool;
        }

        private sealed class EventInfo : AReference
        {
            public string EventName { get; set; }
            public object Sender { get; set; }
            public EventArgs EventArgs { get; set; }

            public override void OnRecycle()
            {
                EventName = null;
                Sender = null;

                if (EventArgs is IReference reference)
                    reference.Recycle();

                EventArgs = null;
            }
        }

        private sealed class EventPool : AReference, IEventPool
        {
            public IEventService EventService { get; set; }

            // ReSharper disable once InconsistentlySynchronizedField
            public int EventCount => m_EventQueue.Count;
            public long EventDispatchTimeLimit { get; set; }

            private readonly Queue<EventInfo> m_EventQueue = new Queue<EventInfo>();
            private readonly Stopwatch m_Stopwatch = new Stopwatch();
            private EventInterceptor m_EventInterceptor;

            public override void OnRecycle()
            {
                Clear();
                EventDispatchTimeLimit = 0;
                m_EventInterceptor = null;
            }

            public void Dispose()
            {
                this.Recycle();
            }

            public void OnUpdate(float elapseSeconds, float realElapseSeconds)
            {
                // ReSharper disable once InconsistentlySynchronizedField
                if (m_EventQueue.Count <= 0)
                    return;

                m_Stopwatch.Reset();
                m_Stopwatch.Start();

                // ReSharper disable once InconsistentlySynchronizedField
                while (m_EventQueue.Count > 0)
                {
                    EventInfo eventInfo;

                    lock (m_EventQueue)
                    {
                        eventInfo = m_EventQueue.Dequeue();
                    }
                    
                    if (m_EventInterceptor == null || !m_EventInterceptor(eventInfo.EventName, eventInfo.Sender, eventInfo.EventArgs))
                    {
                        EventService.Raise(eventInfo.EventName, eventInfo.Sender, eventInfo.EventArgs);
                    }

                    eventInfo.Recycle();

                    if (EventDispatchTimeLimit > 0 && m_Stopwatch.ElapsedMilliseconds > EventDispatchTimeLimit)
                        break;
                }

                m_Stopwatch.Stop();
            }

            public void RegisterInterceptor(EventInterceptor interceptor)
            {
                m_EventInterceptor = interceptor;
            }

            public void UnregisterInterceptor(EventInterceptor interceptor)
            {
                if (m_EventInterceptor == interceptor)
                    m_EventInterceptor = null;
            }

            public void Raise(string eventName, object sender, EventArgs e)
            {
                lock (m_EventQueue)
                {
                    var eventInfo = ReferenceService.GetReference<EventInfo>();
                    eventInfo.EventName = eventName;
                    eventInfo.Sender = sender;
                    eventInfo.EventArgs = e;

                    m_EventQueue.Enqueue(eventInfo);
                }
            }

            public void Clear()
            {
                lock (m_EventQueue)
                {
                    while (m_EventQueue.Count > 0)
                        m_EventQueue.Dequeue().Recycle();
                }
            }
        }
    }
}