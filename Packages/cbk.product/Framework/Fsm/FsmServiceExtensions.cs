namespace CBK.Framework.Fsm
{
    public static class FsmServiceExtensions
    {
        /// <summary>
        /// 创建有限状态机
        /// </summary>
        public static IFsm Allocate(this IFsmService service, params IFsmState[] states)
        {
            return service.Allocate(states);
        }

        /// <summary>
        /// 使用指定的状态运行状态机
        /// </summary>
        public static void Start<T>(this IFsmService service, IFsm fsm, bool isManualUpdate) where T : class, IFsmState
        {
            service.Start(fsm, typeof(T), isManualUpdate);
        }
    }
}