namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机状态接口
    /// </summary>
    public interface IFsmState
    {
        /// <summary>
        /// 状态机初始化时回调
        /// </summary>
        void OnInitialize(IFsm fsm);

        /// <summary>
        /// 进入该状态时回调
        /// </summary>
        void OnEnter(IFsm fsm);

        /// <summary>
        /// 当该状态处于激活状态时 每帧回调
        /// </summary>
        void OnUpdate(IFsm fsm, float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 离开该状态时回调
        /// </summary>
        void OnLeave(IFsm fsm);

        /// <summary>
        /// 状态机被销毁时回调
        /// </summary>
        void OnDestroy(IFsm fsm);
    }
}