using System;
using System.Collections.Generic;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Reference;
using CBK.Framework.Timer.Exception;
using CBK.Framework.Update;

namespace CBK.Framework.Timer
{
    /// <summary>
    /// 定时器服务实现类
    /// </summary>
    internal sealed partial class TimerServiceImpl : IInitialize, IDisposable, ITimerService, IUpdate, ILateUpdate, IFixedUpdate
    {
        [Inject]
        public IUpdateService UpdateService { get; set; }

        [Inject]
        public IReferenceService ReferenceService { get; set; }

        public void Initialize()
        {
            UpdateService.RegisterUpdate(this);
            UpdateService.RegisterLateUpdate(this);
            UpdateService.RegisterFixedUpdate(this);
        }

        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);
            UpdateService.UnRegisterLateUpdate(this);
            UpdateService.UnRegisterFixedUpdate(this);

            _updates.Clear();
            _lateUpdates.Clear();
            _fixedUpdates.Clear();

            _updatesToAdd.Clear();
            _lateUpdatesToAdd.Clear();
            _fixedUpdatesToAdd.Clear();
        }

        public ITimer Allocate()
        {
            var timer = ReferenceService.GetReference<Timer>();
            timer.OnDispose = OnTimerDispose;
            timer.OnStart = OnTimerStart;
            return timer;
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            UpdateTimers(_updates, _updatesToAdd, elapseSeconds, realElapseSeconds);
        }

        public void OnLateUpdate(float elapseSeconds, float realElapseSeconds)
        {
            UpdateTimers(_lateUpdates, _lateUpdatesToAdd, elapseSeconds, realElapseSeconds);
        }

        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            UpdateTimers(_fixedUpdates, _fixedUpdatesToAdd, elapseSeconds, realElapseSeconds);
        }

        public void Update(ITimer timer, float elapseSeconds, float realElapseSeconds)
        {
            if (timer.DriveMode != TimerDriveMode.Manual)
                throw new TimerNotAllowManualUpdateException();

            ((Timer)timer).Update(elapseSeconds, realElapseSeconds);
        }

        private void UpdateTimers(List<ReferenceRef<Timer>> timers, List<ReferenceRef<Timer>> timersToAdd, float elapseSeconds, float realElapseSeconds)
        {
            if (timersToAdd.Count > 0)
            {
                timers.AddRange(timersToAdd);
                timersToAdd.Clear();
            }

            for (var index = 0; index < timers.Count; index++)
            {
                var timerRef = timers[index];

                if (timerRef.IsReferenceValid)
                {
                    if (timerRef.Reference.Status == TimerStatus.Running)
                        timerRef.Reference.Update(elapseSeconds, realElapseSeconds);
                    else
                        continue;
                }

                if (timerRef.IsReferenceValid)
                    continue;

                timers.RemoveAt(index--);
            }
        }

        private readonly List<ReferenceRef<Timer>> _updates = new();
        private readonly List<ReferenceRef<Timer>> _lateUpdates = new();
        private readonly List<ReferenceRef<Timer>> _fixedUpdates = new();

        private readonly List<ReferenceRef<Timer>> _updatesToAdd = new();
        private readonly List<ReferenceRef<Timer>> _lateUpdatesToAdd = new();
        private readonly List<ReferenceRef<Timer>> _fixedUpdatesToAdd = new();
    }
}