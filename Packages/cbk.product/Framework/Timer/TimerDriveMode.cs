namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器驱动模式
    /// </summary>
    public enum TimerDriveMode
    {
        /// <summary>
        /// 由定时器服务在Update中驱动
        /// </summary>
        Update,

        /// <summary>
        /// 由定时器服务在LateUpdate中驱动
        /// </summary>
        LateUpdate,

        /// <summary>
        /// 由定时器服务在FixedUpdate中驱动
        /// </summary>
        FixedUpdate,

        /// <summary>
        /// 由用户手动驱动
        /// </summary>
        Manual,
    }
}