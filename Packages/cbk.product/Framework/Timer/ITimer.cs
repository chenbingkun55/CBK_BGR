using System;
using CBK.Framework.Reference;

namespace CBK.Framework.Timer
{
    public delegate void TimerAction(ITimer timer);

    /// <summary>
    /// 定时器接口
    /// </summary>
    public interface ITimer : IReference, IDisposable
    {
        #region [运行时属性]

        /// <summary>
        /// 定时器状态
        /// </summary>
        TimerStatus Status { get; }

        /// <summary>
        /// 定时器已经运行的时间
        /// </summary>
        float ElapseSeconds { get; }

        /// <summary>
        /// 定时器距离上次调用的时间
        /// </summary>
        float ElapseSecondsSinceLastInvoke { get; }

        /// <summary>
        /// 定时器已经调用的次数
        /// </summary>
        int InvokeCount { get; }

        #endregion

        #region [设置属性]

        /// <summary>
        /// 定时器驱动模式
        /// </summary>
        TimerDriveMode DriveMode { get; set; }
        
        /// <summary>
        /// 是否使用非缩放时间
        /// </summary>
        bool UseUnscaledTime { get; set; }

        /// <summary>
        /// 定时器第一次触发的延迟时间
        /// </summary>
        float Delay { get; set; }

        /// <summary>
        /// 定时器调用间隔时间
        /// </summary>
        float Interval { get; set; }

        /// <summary>
        /// 定时器循环次数限制 0表示无限循环
        /// </summary>
        uint LoopLimit { get; set; }
        
        /// <summary>
        /// 定时器启动时的回调
        /// </summary>
        event TimerAction OnTimerStart;

        /// <summary>
        /// 定时器第一次触发的事件
        /// </summary>
        event TimerAction OnTimerFirstInvoke;

        /// <summary>
        /// 定时器每次触发的事件
        /// </summary>
        event TimerAction OnTimerInvoke;

        /// <summary>
        /// 定时器停止时的回调
        /// </summary>
        event TimerAction OnTimerStop;

        /// <summary>
        /// 用户透传数据
        /// </summary>
        object UserData { get; set; }

        #endregion

        #region [API]

        /// <summary>
        /// 启动定时器 在启动之前请先设置好定时器的各项参数
        /// </summary>
        void Start();

        /// <summary>
        /// 暂停定时器
        /// </summary>
        void Pause();

        /// <summary>
        /// 恢复定时器
        /// </summary>
        void Resume();

        /// <summary>
        /// 停止定时器 (将清理运行时数据 对于托管的定时器在停止后将自动销毁 对于手动的定时器需要手动调用Dispose）
        /// 当定时器运行次数达到限制时也会自动停止
        /// </summary>
        void Stop();

        #endregion
    }
}