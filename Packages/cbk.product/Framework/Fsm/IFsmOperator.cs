using System;
using System.Collections.Generic;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机操作接口 该接口为状态机服务内部使用 不对外暴露
    /// </summary>
    internal interface IFsmOperator : IFsm
    {
        /// <summary>
        /// 使用指定的若干状态初始化状态机
        /// </summary>
        void Initialize(IEnumerable<IFsmState> states);

        /// <summary>
        /// 使用指定的状态运行状态机
        /// </summary>
        void Start(Type stateType);

        /// <summary>
        /// 轮询状态机
        /// </summary>
        void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 停止状态机
        /// </summary>
        void Stop();

        /// <summary>
        /// 销毁状态机
        /// </summary>
        void Destroy();

        /// <summary>
        /// 令状态机切换到指定状态
        /// </summary>
        void ChangeState(Type stateType);
    }
}