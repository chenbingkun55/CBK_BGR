using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Fsm;

namespace CBK.Framework.Procedure.Fsm
{
    /// <summary>
    /// 基于有限状态机实现的流程服务实现
    /// </summary>
    internal sealed class FsmProcedureServiceImpl : IInitialize, IDisposable, IProcedureService
    {
        public IProcedure CurrentProcedure => _fsm?.RunningState as IProcedure;

        public Type CurrentProcedureType => _fsm?.RunningStateType;

        [Inject]
        public IFsmService FsmService { get; set; }

        public void Initialize()
        {
        }

        public void Dispose()
        {
            if (_fsm == null)
                return;

            FsmService.Destroy(_fsm);
            _fsm = null;
        }

        public void Run(IEnumerable<IProcedure> procedures, Type entryProcedureType)
        {
            if (_fsm != null)
                throw new ProcedureAlreadyRunException();

            var states = new List<IFsmState>();
            foreach (var procedure in procedures)
            {
                if (!(procedure is AFsmProcedure fsmProcedure))
                    throw new ProcedureNotFsmProcedureException(procedure.GetType());

                states.Add(fsmProcedure);
            }

            _fsm = FsmService.Allocate(states);
            FsmService.Start(_fsm, entryProcedureType, false);
        }

        private IFsm _fsm;
    }
}