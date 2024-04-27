namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化状态
    /// </summary>
    public enum LocalizationStatus
    {
        /// <summary>
        /// 未加载
        /// </summary>
        Unload,
        
        /// <summary>
        /// 加载中
        /// </summary>
        Loading,
        
        /// <summary>
        /// 已加载
        /// </summary>
        Loaded,
        
        /// <summary>
        /// 加载失败
        /// </summary>
        Failed,
    }
}