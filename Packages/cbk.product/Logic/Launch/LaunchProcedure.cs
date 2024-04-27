using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Attribute;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 启动流程
    /// </summary>
    [Procedure(true)]
    public sealed class LaunchProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.LaunchStart;

        protected override UniTask<int> ExecuteStep()
        {
            return UniTask.FromResult(0);
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            Change<InitLocalizationProcedure>(fsm);
        }
    }
}