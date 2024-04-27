using CatLib.Container;
using CBK.Framework.Business;
using CBK.Framework.Extensions;
using CBK.Framework.Fsm;
using CBK.Framework.Update;

namespace CBK.Framework.Game
{
    /// <summary>
    /// 玩法流程抽象类
    /// </summary>
    public abstract class AGameProcedure : ABusinessProcedure
    {
        protected IGame Game { get; private set; }

        private IBindData m_GameBindData;
        
        public override void OnEnter(IFsm fsm)
        {
            var container = CatLib.App.That;
            
            Game = CreateGame(fsm);
            m_GameBindData = container.BindSingletonInstance(Game);
            
            UpdateService.That.RegisterFixedUpdate(Game);
            
            base.OnEnter(fsm);
        }

        public override void OnLeave(IFsm fsm)
        {
            base.OnLeave(fsm);
            
            UpdateService.That.UnRegisterFixedUpdate(Game);
            
            m_GameBindData?.Unbind();
            m_GameBindData = null;
            Game = null;
        }

        /// <summary>
        /// 创建玩法实例
        /// </summary>
        protected abstract IGame CreateGame(IFsm fsm);
    }
}