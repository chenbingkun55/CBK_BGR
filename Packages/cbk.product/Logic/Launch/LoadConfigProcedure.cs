using Cysharp.Threading.Tasks;
using CBK.Framework.Configuration;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Attribute;
// using GameLogic.Login;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 加载配置表流程
    /// </summary>
    [Procedure]
    public sealed class LoadConfigProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.LoadConfig;
        protected override UniTask<int> ExecuteStep()
        {
            return ConfigurationService.That.LoadAsync();
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            Change<CreateGameProcedure>(fsm);
        }
    }
}