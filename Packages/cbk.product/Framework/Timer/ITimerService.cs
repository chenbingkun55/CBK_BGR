namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器服务
    /// </summary>
    public interface ITimerService
    {
        /// <summary>
        /// 创建定时器
        /// </summary>
        ITimer Allocate();
        
        /// <summary>
        /// 更新指定定时器
        /// </summary>
        void Update(ITimer timer, float elapseSeconds, float realElapseSeconds);
    }
}