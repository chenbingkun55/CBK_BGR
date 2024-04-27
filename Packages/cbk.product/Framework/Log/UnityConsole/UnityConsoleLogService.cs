using System;

namespace CBK.Framework.UnityConsole
{
    /// <summary>
    /// Unity控制台日志服务
    /// </summary>
    internal sealed class UnityConsoleLogService : ILogService
    {
        public void Debug(object message)
        {
            UnityEngine.Debug.Log($"[D] {message}");
        }

        public void Warning(object message)
        {
            UnityEngine.Debug.LogWarning($"[W] {message}");
        }

        public void Info(object message)
        {
            UnityEngine.Debug.Log($"[I] {message}");
        }

        public void Error(object message)
        {
            UnityEngine.Debug.LogError($"[E] {message}");
        }

        public void Fatal(object message)
        {
            UnityEngine.Debug.LogError($"[F] {message}");
        }

        public void Exception(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
    }
}