using System;

namespace CBK.Framework.Fsm
{
    [Flags]
    public enum FsmStatus
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        None,

        /// <summary>
        /// 已初始化
        /// </summary>
        Initialized,

        /// <summary>
        /// 运行中
        /// </summary>
        Started,

        /// <summary>
        /// 停止
        /// </summary>
        Stopped,

        /// <summary>
        /// 已销毁
        /// </summary>
        Destroyed,
    }
}