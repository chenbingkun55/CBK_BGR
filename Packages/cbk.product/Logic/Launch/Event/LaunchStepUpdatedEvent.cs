using CBK.Framework.Event;
using CBK.Framework.Reference;

namespace CBK.Logic.Launch
{
    /// <summary>
    /// 启动环节更新事件
    /// </summary>
    public sealed class LaunchStepUpdatedEvent : AEventArgs
    {
        /// <summary>
        /// 当前阶段
        /// </summary>
        public LaunchStep Step { get; set; }
        
        /// <summary>
        /// 当前阶段进度
        /// </summary>
        public float Progress { get; set; }
        
        public override void OnRecycle()
        {
            Step = LaunchStep.LaunchStart;
            Progress = 0f;
        }

        public static LaunchStepUpdatedEvent Acquire(LaunchStep step, float progress = 0f)
        {
            var eventArgs = CBK.Framework.Reference.ReferenceService.That.GetReference<LaunchStepUpdatedEvent>();
            eventArgs.Step = step;
            eventArgs.Progress = progress;
            return eventArgs;
        }
    }
}