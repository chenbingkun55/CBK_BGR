using System;
using System.Collections.Generic;
using CBK.Framework.Utility;

namespace CBK.Framework.Core
{
    /// <summary>
    /// 可监听值变动的变量
    /// </summary>
    public sealed class ObservableVariable<T>
    {
        public delegate void ValueChangedHandler(T oldValue, T newValue);

        private T m_Value;
        private readonly List<ValueChangedHandler> m_Handlers = new List<ValueChangedHandler>();

        public T Value
        {
            get => m_Value;
            set
            {
                if (value == null && m_Value == null)
                    return;

                if (value != null && value.Equals(m_Value))
                    return;

                var oldValue = m_Value;
                m_Value = value;
                NotifyValueChanged(oldValue, value);
            }
        }

        public ObservableVariable(T mValue = default)
        {
            m_Value = mValue;
        }

        /// <summary>
        /// 不触发事件的设置属性值方法
        /// </summary>
        public void SetValueSilently(T value)
        {
            m_Value = value;
        }

        /// <summary>
        /// 订阅值变动事件
        /// </summary>
        public void Subscribe(ValueChangedHandler handler, bool callImmediately = false)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            m_Handlers.Add(handler);
            if (!callImmediately)
                return;

            try
            {
                handler(m_Value, m_Value);
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        /// <summary>
        /// 取消订阅值变动事件
        /// </summary>
        public void Unsubscribe(ValueChangedHandler handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            m_Handlers.Remove(handler);
        }

        /// <summary>
        /// 通知值变动
        /// </summary>
        private void NotifyValueChanged(T oldValue, T newValue)
        {
            foreach (var handler in m_Handlers)
            {
                try
                {
                    handler(oldValue, newValue);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
        }
    }

    public static class ObservableVariableExtensions
    {
        /// <summary>
        /// 订阅值变动事件 并返回一个可释放的对象 该对象Dispose时会自动取消订阅
        /// </summary>
        public static IDisposable SubscribeAsDisposable<T>(this ObservableVariable<T> variable, ObservableVariable<T>.ValueChangedHandler handler, bool callImmediately = false)
        {
            void Dispose()
            {
                variable.Unsubscribe(handler);
            }
            
            variable.Subscribe(handler, callImmediately);
            return ((Action)Dispose).AsDisposable();
        }
    }
}