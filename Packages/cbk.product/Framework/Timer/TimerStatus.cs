namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器状态
    /// </summary>
    public enum TimerStatus
    {
        /// <summary>
        /// 未运行
        /// </summary>
        None,
        
        /// <summary>
        /// 运行中
        /// </summary>
        Running,
        
        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
        
        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,
        
        /// <summary>
        /// 已销毁
        /// </summary>
        Destroyed,
    }
}