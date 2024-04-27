using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Attribute;
using CBK.Framework.UI;
using CBK.Framework.UI.FairyGUI;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 打开启动界面流程
    /// </summary>
    [Procedure]
    public sealed class OpenLaunchFormProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.OpenLaunchForm;
        protected override UniTask<int> ExecuteStep()
        {
            // TODO Open Launch Form
            return UniTask.FromResult(0);
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            Change<CheckVersionProcedure>(fsm);
        }
    }
}