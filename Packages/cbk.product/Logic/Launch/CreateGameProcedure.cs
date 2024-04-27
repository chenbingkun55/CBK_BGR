using CatLib;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Fsm;
using CBK.Framework.Game;
using CBK.Framework.Procedure.Attribute;
using CBK.Framework.Update;
// using CBK.Logic.Login;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 创建玩法实例流程
    /// </summary>
    [Procedure]
    public class CreateGameProcedure : ALaunchStepProcedure
    {
        protected override LaunchStep Step => LaunchStep.CreateGame;
        protected override UniTask<int> ExecuteStep()
        {
            // 放置类玩法实例是一个全局的单例
            // var game = App.That.Make<IGame>();
            // UpdateService.That.RegisterFixedUpdate(game);
            return UniTask.FromResult(0);
        }

        protected override void OnCompleteUpdate(IFsm fsm)
        {
            // Change<LoginProcedure>(fsm);
        }
    }
}