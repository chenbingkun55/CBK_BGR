using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Localization;
using CBK.Framework.Procedure.Attribute;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 初始化本地化服务流程
    /// </summary>
    [Procedure]
    public sealed class InitLocalizationProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.InitLocalization;

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            Change<OpenLaunchFormProcedure>(fsm);
        }

        protected override async UniTask<int> ExecuteStep()
        {
            var code = await LocalizationService.That.LoadAsync();
            if (code != 0)
                return code;

            return 0;
        }
    }
}