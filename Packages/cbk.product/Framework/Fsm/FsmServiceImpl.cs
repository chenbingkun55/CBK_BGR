using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Fsm.Exception;
using CBK.Framework.Reference;
using CBK.Framework.Update;

namespace CBK.Framework.Fsm
{
    /// <summary>
    /// 有限状态机服务实现
    /// </summary>
    internal sealed class FsmServiceImpl : IInitialize, IDisposable, IFsmService, IUpdate
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }

        [Inject]
        public IUpdateService UpdateService { get; set; }

        public void Initialize()
        {
            UpdateService.RegisterUpdate(this);
        }

        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);

            _updatingFsms.Clear();
            _updateFsms.Clear();
            _startedFsms.Clear();

            foreach (var fsm in _fsms)
            {
                fsm.Destroy();
                ReferenceService.Recycle(fsm);
            }
            
            _fsms.Clear();
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            _updatingIndex = 0;
            _updatingFsms.Clear();

            if (_updateFsms.Count == 0)
                return;

            _updatingFsms.AddRange(_updateFsms);

            while (_updatingIndex < _updatingFsms.Count)
            {
                try
                {
                    _updatingFsms[_updatingIndex].Update(elapseSeconds, realElapseSeconds);
                }
                catch (System.Exception e)
                {
                    Log.Exception(e);
                }

                ++_updatingIndex;
            }
        }

        public IFsm Allocate(IEnumerable<IFsmState> states)
        {
            var fsm = ReferenceService.GetReference<Fsm>();
            fsm.Initialize(states);

            _fsms.Add(fsm);
            return fsm;
        }

        public void Start(IFsm fsm, Type stateType, bool isManualUpdate)
        {
            var inst = fsm as Fsm;
            if (inst == null)
                throw new FsmNotFoundException();

            inst.Start(stateType);

            _startedFsms.Add(inst);

            if (isManualUpdate)
                return;

            _updateFsms.Add(inst);
        }

        public void Update(IFsm fsm, float elapseSeconds, float realElapseSeconds)
        {
            var inst = fsm as Fsm;
            if (inst == null)
                throw new FsmNotFoundException();

            inst.Update(elapseSeconds, realElapseSeconds);
        }

        public void Stop(IFsm fsm)
        {
            var inst = fsm as Fsm;
            if (inst == null)
                throw new FsmNotFoundException();

            inst.Stop();

            if (!_startedFsms.Remove(inst))
                return;

            if (!_updateFsms.Remove(inst))
                return;

            var index = _updatingFsms.IndexOf(inst);
            if (index <= _updatingIndex)
                return;

            _updatingFsms.RemoveAt(index);
        }

        public void Destroy(IFsm fsm)
        {
            if (fsm.Status == FsmStatus.Started)
                Stop(fsm);

            var inst = fsm as Fsm;
            if (inst == null || !_fsms.Remove(inst))
                throw new FsmNotFoundException();

            inst.Destroy();
            ReferenceService.Recycle(inst);
        }

        private readonly List<Fsm> _fsms = new();
        private readonly List<Fsm> _startedFsms = new();
        private readonly List<Fsm> _updateFsms = new();

        private readonly List<Fsm> _updatingFsms = new();
        private int _updatingIndex;
    }
}