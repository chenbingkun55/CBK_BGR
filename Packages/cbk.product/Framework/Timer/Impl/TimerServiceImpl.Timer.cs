using System;
using CBK.Framework.Reference;
using CBK.Framework.Timer.Exception;

namespace CBK.Framework.Timer
{
    internal sealed partial class TimerServiceImpl
    {
        private void OnTimerStart(Timer timer)
        {
            switch (timer.DriveMode)
            {
                case TimerDriveMode.Update:
                    _updatesToAdd.Add(timer.AsRef());
                    break;
                case TimerDriveMode.LateUpdate:
                    _lateUpdatesToAdd.Add(timer.AsRef());
                    break;
                case TimerDriveMode.FixedUpdate:
                    _fixedUpdatesToAdd.Add(timer.AsRef());
                    break;
                case TimerDriveMode.Manual:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnTimerDispose(Timer timer)
        {
            ReferenceService.Recycle(timer);
        }

        private sealed class Timer : AReference, ITimer
        {
            public TimerStatus Status { get; private set; }
            public float ElapseSeconds { get; private set; }
            public float ElapseSecondsSinceLastInvoke { get; private set; }
            public int InvokeCount { get; private set; }
            public TimerDriveMode DriveMode { get; set; }
            public bool UseUnscaledTime { get; set; }
            public float Delay { get; set; }
            public float Interval { get; set; }
            public uint LoopLimit { get; set; }
            public event TimerAction OnTimerStart;
            public event TimerAction OnTimerFirstInvoke;
            public event TimerAction OnTimerInvoke;
            public event TimerAction OnTimerStop;
            public object UserData { get; set; }

            public Action<Timer> OnStart { get; set; }
            public Action<Timer> OnDispose { get; set; }

            public override void OnRecycle()
            {
                if (Status == TimerStatus.Paused || Status == TimerStatus.Running)
                    Stop();

                Status = TimerStatus.None;
                ElapseSeconds = 0f;
                ElapseSecondsSinceLastInvoke = 0f;
                InvokeCount = 0;
                DriveMode = TimerDriveMode.Update;
                UseUnscaledTime = false;
                Delay = 0f;
                Interval = 0f;
                LoopLimit = 0;
                OnTimerStart = null;
                OnTimerFirstInvoke = null;
                OnTimerInvoke = null;
                OnTimerStop = null;
                OnStart = null;
                OnDispose = null;
                UserData = null;
            }

            public void Dispose()
            {
                OnDispose?.Invoke(this);
            }

            public void Start()
            {
                if (Status != TimerStatus.None && Status != TimerStatus.Stopped)
                    throw new TimerOperateStatusException(new[] { TimerStatus.None, TimerStatus.Stopped }, Status);

                Status = TimerStatus.Running;
                OnStart?.Invoke(this);

                try
                {
                    OnTimerStart?.Invoke(this);
                }
                catch (System.Exception e)
                {
                    Log.Exception(e);
                }
            }

            public void Pause()
            {
                if (Status != TimerStatus.Running)
                    throw new TimerOperateStatusException(TimerStatus.Running, Status);

                Status = TimerStatus.Paused;
            }

            public void Resume()
            {
                if (Status != TimerStatus.Paused)
                    throw new TimerOperateStatusException(TimerStatus.Paused, Status);

                Status = TimerStatus.Running;
            }

            public void Stop()
            {
                if (Status != TimerStatus.Running && Status != TimerStatus.Paused)
                    throw new TimerOperateStatusException(new[] { TimerStatus.Running, TimerStatus.Paused }, Status);

                Status = TimerStatus.Stopped;

                try
                {
                    OnTimerStop?.Invoke(this);
                }
                catch (System.Exception e)
                {
                    Log.Exception(e);
                }

                // 防止在OnTimerStop中又调用Start 导致数据被清理
                if (Status != TimerStatus.Stopped)
                    return;

                // 如果是手动模式 只清理数据 不销毁定时器
                if (DriveMode == TimerDriveMode.Manual)
                    ResetRunningStates();
                else
                    Dispose();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (Status != TimerStatus.Running)
                    throw new TimerOperateStatusException(TimerStatus.Running, Status);

                elapseSeconds = UseUnscaledTime ? realElapseSeconds : elapseSeconds;

                ElapseSeconds += elapseSeconds;
                ElapseSecondsSinceLastInvoke += elapseSeconds;

                var interval = InvokeCount == 0 ? Delay : Interval;
                if (ElapseSecondsSinceLastInvoke < interval)
                    return;

                ++InvokeCount;
                ElapseSecondsSinceLastInvoke -= interval;

                if (InvokeCount == 1)
                {
                    try
                    {
                        OnTimerFirstInvoke?.Invoke(this);
                    }
                    catch (System.Exception e)
                    {
                        Log.Exception(e);
                    }
                }

                try
                {
                    OnTimerInvoke?.Invoke(this);
                }
                catch (System.Exception e)
                {
                    Log.Exception(e);
                }

                if (LoopLimit == 0 || InvokeCount < LoopLimit)
                    return;

                Stop();
            }

            private void ResetRunningStates()
            {
                ElapseSeconds = 0f;
                ElapseSecondsSinceLastInvoke = 0f;
                InvokeCount = 0;
            }
        }
    }
}