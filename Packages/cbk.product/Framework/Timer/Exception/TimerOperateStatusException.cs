namespace CBK.Framework.Timer.Exception
{
    /// <summary>
    /// 定时器操作时运行状态异常
    /// </summary>
    public sealed class TimerOperateStatusException : System.Exception
    {
        public TimerStatus[] Excepted { get; set; }
        public TimerStatus Result { get; set; }

        public TimerOperateStatusException(TimerStatus excepted, TimerStatus result)
        {
            Excepted = new TimerStatus[] { excepted };
            Result = result;
        }

        public TimerOperateStatusException(TimerStatus[] excepted, TimerStatus result)
        {
            Excepted = excepted;
            Result = result;
        }
    }
}