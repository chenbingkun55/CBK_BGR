using CatLib;
using CatLib.Container;

namespace CBK.Framework.ScreenAdaptor
{
    /// <summary>
    /// 屏幕适配服务提供者。
    /// </summary>
    internal sealed class ScreenServiceProvider : ServiceProvider
    {
        public override void Register()
        {
#if UNITY_EDITOR
            App.Singleton<IScreenAdaptorService, EditorScreenAdaptorService>();
#else
            App.Singleton<IScreenAdaptorService, UnityScreenAdaptorService>();
#endif
        }
    }
}