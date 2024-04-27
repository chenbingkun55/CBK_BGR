using CatLib;
using CatLib.Container;

namespace CBK.Framework.DebugCommand
{
    /// <summary>
    /// 调试命令服务提供者。
    /// </summary>
    internal sealed class DebugCommandServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            App.Singleton<IDebugCommandService, DebugLogConsoleDebugCommandService>();
#else
            App.Singleton<IDebugCommandService, EmptyDebugCommandService>();
#endif
        }

        public override void Init()
        {
            base.Init();
            App.Make<IDebugCommandService>();
        }
    }
}