namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器服务扩展
    /// </summary>
    public static class TimerServiceExtensions
    {
        /// <summary>
        /// 创建并开启一个无限循环的定时器
        /// </summary>
        public static ITimer StartTimer(this ITimerService service, float interval, TimerAction action)
        {
            var timer = service.Allocate();
            timer.Delay = interval;
            timer.Interval = interval;
            timer.OnTimerInvoke += action;
            timer.Start();
            return timer;
        }
        
        /// <summary>
        /// 创建并开启一个循环指定次数的定时器
        /// </summary>
        public static ITimer StartTimer(this ITimerService service, float interval, uint loopLimit, TimerAction action)
        {
            var timer = service.Allocate();
            timer.Delay = interval;
            timer.Interval = interval;
            timer.LoopLimit = loopLimit;
            timer.OnTimerInvoke += action;
            timer.Start();
            return timer;
        }
        
        /// <summary>
        /// 创建并开启一个指定延迟后执行一次的定时器
        /// </summary>
        public static ITimer StartOnceTimer(this ITimerService service, float delay, TimerAction action)
        {
            var timer = service.Allocate();
            timer.Delay = delay;
            timer.LoopLimit = 1;
            timer.OnTimerInvoke += action;
            timer.Start();
            return timer;
        }
        
        /// <summary>
        /// 创建并开启一个指定延迟后按照指定间隔时间触发若干次数的定时器
        /// </summary>
        public static ITimer StartTimer(this ITimerService service, float delay, float interval, uint loopLimit, TimerAction action)
        {
            var timer = service.Allocate();
            timer.Delay = delay;
            timer.Interval = interval;
            timer.LoopLimit = loopLimit;
            timer.OnTimerInvoke += action;
            timer.Start();
            return timer;
        }
    }
}