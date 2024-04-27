namespace CBK.Framework.Network
{
    public static class NetworkUtility
    {
        /// <summary>
        /// 是否启用消息日志
        /// </summary>
        public static bool EnableMessageLog
        {
            get
            {
                
#if UNITY_EDITOR || DEVELOPMENT_BUILD || LOG_NETWORK_MESSAGE
                return true;
#else
                return false;
#endif
            }
        }
    }
}