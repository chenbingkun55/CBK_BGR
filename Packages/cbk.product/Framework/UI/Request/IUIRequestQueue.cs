using System;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面请求队列接口
    /// </summary>
    public interface IUIRequestQueue
    {
        /// <summary>
        /// 队列为空的事件
        /// </summary>
        event Action OnQueueEmpty;
        
        /// <summary>
        /// 队列是否为空
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// 当前执行中的请求
        /// </summary>
        IUIRequest CurrentRequest { get; }
        
        /// <summary>
        /// 令请求加入队列
        /// </summary>
        void Enqueue(IUIRequest request);

        /// <summary>
        /// 界面打开时调用
        /// </summary>
        void OnUIFormOpened();

        /// <summary>
        /// 界面关闭时调用
        /// </summary>
        void OnUIFormClosed();
    }
}