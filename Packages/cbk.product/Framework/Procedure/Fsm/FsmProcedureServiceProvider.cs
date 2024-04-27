using CatLib;
using CatLib.Container;

namespace CBK.Framework.Procedure.Fsm
{
    /// <summary>
    /// 基于有限状态机实现的流程服务提供者
    /// </summary>
    internal sealed class FsmProcedureServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IProcedureService, FsmProcedureServiceImpl>();
        }
    }
}