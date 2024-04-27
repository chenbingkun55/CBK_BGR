using System;
using System.Collections.Generic;
using CBK.Framework.Core;
using CBK.Framework.Fsm.Exception;
using CBK.Framework.Reference;

namespace CBK.Framework.Fsm
{
    internal class Fsm : AReference, IFsm, IFsmOperator
    {
        public FsmStatus Status { get; private set; }
        public IFsmState RunningState { get; private set; }
        public Type RunningStateType { get; private set; }
        public Blackboard Blackboard { get; private set; }

        public void Initialize(IEnumerable<IFsmState> states)
        {
            if (Status != FsmStatus.None)
                throw new FsmOperateStatusException(FsmStatus.None, Status);

            Blackboard = ReferenceService.GetReference<Blackboard>();
            Status = FsmStatus.Initialized;
            foreach (var state in states)
            {
                _fsmStates.Add(state.GetType(), state);
                state.OnInitialize(this);
            }
        }

        public void Start(Type stateType)
        {
            if (Status != FsmStatus.Initialized && Status != FsmStatus.Stopped)
                throw new FsmOperateStatusException(new[] { FsmStatus.Initialized, FsmStatus.Stopped }, Status);

            Status = FsmStatus.Started;
            EnterState(stateType);
        }

        public void ChangeState(Type stateType)
        {
            if (Status != FsmStatus.Started)
                throw new FsmOperateStatusException(FsmStatus.Started, Status);

            if (RunningStateType == stateType)
                return;

            LeaveState();
            EnterState(stateType);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Status != FsmStatus.Started)
                throw new FsmOperateStatusException(FsmStatus.Started, Status);

            RunningState.OnUpdate(this, elapseSeconds, realElapseSeconds);
        }

        public void Stop()
        {
            if (Status != FsmStatus.Started)
                throw new FsmOperateStatusException(FsmStatus.Started, Status);

            LeaveState();
            Status = FsmStatus.Stopped;
        }

        public void Destroy()
        {
            if (Status == FsmStatus.None || Status == FsmStatus.Destroyed)
                throw new FsmOperateStatusException(new[] { FsmStatus.Initialized, FsmStatus.Started, FsmStatus.Stopped }, Status);

            LeaveState();

            foreach (var state in _fsmStates.Values)
                state.OnDestroy(this);

            Status = FsmStatus.Destroyed;
        }

        private void EnterState(Type stateType)
        {
            if (!_fsmStates.TryGetValue(stateType, out var state))
                throw new FsmStateNotFoundException(stateType);

            RunningStateType = stateType;
            RunningState = state;
            RunningState.OnEnter(this);
        }

        private void LeaveState()
        {
            if (RunningStateType == null)
                return;

            var state = RunningState;
            RunningStateType = null;
            RunningState = null;
            state.OnLeave(this);
        }

        public override void OnRecycle()
        {
            Status = FsmStatus.None;
            RunningState = null;
            _fsmStates.Clear();
            Blackboard?.Recycle();
            Blackboard = null;
        }

        private readonly Dictionary<Type, IFsmState> _fsmStates = new Dictionary<Type, IFsmState>();
    }
}