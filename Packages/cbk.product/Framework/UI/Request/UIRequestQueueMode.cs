namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面请求队列模式
    /// </summary>
    public enum UIRequestQueueMode
    {
        /// <summary>
        /// 顺序模式，请求将按调用顺序执行
        /// </summary>
        Sequence,
        
        /// <summary>
        /// 倒序模式，请求将按调用倒序执行
        /// </summary>
        Reverse,
        
        /// <summary>
        /// 覆盖模式，新的请求会覆盖旧的请求
        /// </summary>
        Override,
        
        /// <summary>
        /// 并行模式，新的请求将立即执行
        /// </summary>
        Parallel,
    }
}