using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Attribute;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 检查版本流程
    /// </summary>
    [Procedure]
    public sealed class CheckVersionProcedure : ALaunchStepProcedure
    {
        private bool m_NeedToUpgradeApp;
        private bool m_NeedToUpgradeResources;

        protected override LaunchStep Step => LaunchStep.CheckVersion;
        
        protected override UniTask<int> ExecuteStep()
        {
            // TODO 直接当作检查成功
            m_NeedToUpgradeApp = false;
            m_NeedToUpgradeResources = false;
            
            return UniTask.FromResult(0);
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            if (m_NeedToUpgradeApp)
            {
                // 需要更新App
                // TODO 跳转到应用商店
                return;
            }

            if (m_NeedToUpgradeResources)
            {
                // 需要更新资源
                Change<UpdateResourceProcedure>(fsm);
                return;
            }
            
            // 进入下个流程
            Change<LoadConfigProcedure>(fsm);
        }
    }
}