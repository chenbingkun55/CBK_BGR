using CBK.Framework.Event;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化数据加载完成事件
    /// </summary>
    public sealed class LocalizationLoadDoneEvent : AEventArgs
    {
        public override void OnRecycle()
        {
        }
    }
}