namespace CBK.Framework.Fsm
{
    public static class FsmExtensions
    {
        /// <summary>
        /// 判断状态机当前运行的是否是指定状态
        /// </summary>
        public static bool IsStateRunning<T>(this IFsm fsm) where T : class, IFsmState
        {
            return fsm.RunningStateType == typeof(T);
        }
    }
}