namespace CBK.Framework
{
    /// <summary>
    /// 日志服务
    /// </summary>
    public interface ILogService
    {
        void Debug(object message);

        void Warning(object message);

        void Info(object message);
        
        void Error(object message);
        
        void Fatal(object message);
        
        void Exception(System.Exception exception);
    }
}