using Cysharp.Threading.Tasks;
using GameFramework;
using CBK.Framework.Event;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Fsm;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 启动流程步骤
    /// </summary>
    public abstract class ALaunchStepProcedure : AFsmProcedure
    {
        protected abstract LaunchStep Step { get; }
        
        private bool m_Complete;

        public override void OnEnter(IFsm fsm)
        {
            base.OnEnter(fsm);
            m_Complete = false;
            EventService.That.Raise(this, LaunchStepUpdatedEvent.Acquire(Step));
            
            Execute().Forget();
        }

        public override void OnLeave(IFsm fsm)
        {
            base.OnLeave(fsm);
            EventService.That.Raise(this, LaunchStepUpdatedEvent.Acquire(Step, 1f));
        }

        public override void OnUpdate(IFsm fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);
            
            if (!m_Complete)
                return;
            
            OnCompleteUpdate(fsm);
        }
        
        private async UniTask Execute()
        {
            var code = await ExecuteStep();

            if (code != 0)
            {
                // TODO 错误处理
                // Log.Error($"ExecuteStep: {Step} {code}");
                return;
            }
            
            m_Complete = true;
        }

        protected abstract UniTask<int> ExecuteStep();
        protected abstract void OnCompleteUpdate(IFsm fsm);
    }
}