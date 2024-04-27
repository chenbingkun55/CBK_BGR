using System;
using CBK.Framework.Core;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机接口
    /// </summary>
    public interface IFsm
    {
        /// <summary>
        /// 状态机运行状态
        /// </summary>
        FsmStatus Status { get; }

        /// <summary>
        /// 当前状态 仅在Status为Running时有值
        /// </summary>
        IFsmState RunningState { get; }
        
        /// <summary>
        /// 当前状态类型 仅在Status为Running时有值
        /// </summary>
        Type RunningStateType { get; }

        /// <summary>
        /// 状态机黑板
        /// </summary>
        Blackboard Blackboard { get; }
    }
}