using System;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 状态机状态实例抽象类
    /// </summary>
    public abstract class AFsmState : IFsmState
    {
        public virtual void OnInitialize(IFsm fsm)
        {
        }

        public virtual void OnEnter(IFsm fsm)
        {
        }

        public virtual void OnUpdate(IFsm fsm, float elapseSeconds, float realElapseSeconds)
        {
        }

        public virtual void OnLeave(IFsm fsm)
        {
        }

        public virtual void OnDestroy(IFsm fsm)
        {
        }

        /// <summary>
        /// 令状态机切换到指定状态
        /// </summary>
        protected void Change(IFsm fsm, Type stateType)
        {
            ((IFsmOperator)fsm).ChangeState(stateType);
        }

        /// <summary>
        /// 令状态机切换到指定状态
        /// </summary>
        protected void Change<T>(IFsm fsm) where T : class, IFsmState
        {
            Change(fsm, typeof(T));
        }
    }
}