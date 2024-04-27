using System;
using System.Collections.Generic;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机服务
    /// </summary>
    public interface IFsmService
    {
        /// <summary>
        /// 创建有限状态机
        /// </summary>
        IFsm Allocate(IEnumerable<IFsmState> states);

        /// <summary>
        /// 使用指定的状态运行状态机
        /// </summary>
        void Start(IFsm fsm, Type stateType, bool isManualUpdate);

        /// <summary>
        /// 手动触发状态机的帧更新逻辑
        /// </summary>
        void Update(IFsm fsm, float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 停止状态机
        /// </summary>
        void Stop(IFsm fsm);

        /// <summary>
        /// 销毁状态机
        /// </summary>
        void Destroy(IFsm fsm);
    }
}