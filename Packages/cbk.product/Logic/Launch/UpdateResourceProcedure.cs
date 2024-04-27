using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Attribute;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 更新资源流程
    /// </summary>
    [Procedure]
    public sealed class UpdateResourceProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.UpdateResource;
        protected override UniTask<int> ExecuteStep()
        {
            // TODO 直接当作更新成功
            return UniTask.FromResult(0);
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            // 更新完成需要热重载游戏
            // AApp.That.Reboot();
        }
    }
}