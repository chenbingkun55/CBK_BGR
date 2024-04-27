using System;

namespace CBK.Framework.Event
{
    public static class EventUtility
    {
        /// <summary>
        /// 获取事件名称
        /// </summary>
        public static string GetEventName(this EventArgs eventArgs)
        {
            return eventArgs != null ? GetEventName(eventArgs.GetType()) : string.Empty;
        }

        /// <summary>
        /// 获取事件名称
        /// </summary>
        public static string GetEventName<T>() where T : EventArgs
        {
            return GetEventName(typeof(T));
        }
        
        /// <summary>
        /// 获取事件名称
        /// </summary>
        public static string GetEventName(Type type)
        {
            return type.FullName;
        }
    }
}