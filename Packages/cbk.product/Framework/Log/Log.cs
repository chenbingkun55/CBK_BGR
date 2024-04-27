using System.Diagnostics;

namespace CBK.Framework
{
    /// <summary>
    /// 日志入口
    /// </summary>
    public static class Log
    {
        [Conditional("ENABLE_LOG_DEBUG")]
        public static void Debug(object message)
        {
            LogService.That.Debug(message);
        }

        [Conditional("ENABLE_LOG_WARNING")]
        public static void Warning(object message)
        {
            LogService.That.Warning(message);
        }

        [Conditional("ENABLE_LOG_INFO")]
        public static void Info(object message)
        {
            LogService.That.Info(message);
        }

        [Conditional("ENABLE_LOG_ERROR")]
        public static void Error(object message)
        {
            LogService.That.Error(message);
        }

        [Conditional("ENABLE_LOG_FATAL")]
        public static void Fatal(object message)
        {
            LogService.That.Fatal(message);
        }

        [Conditional("ENABLE_LOG_EXCEPTION")]
        public static void Exception(System.Exception exception)
        {
            LogService.That.Exception(exception);
        }
    }
}