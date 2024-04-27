using CatLib;
using CatLib.Container;

namespace CBK.Framework.UnityConsole
{
    /// <summary>
    /// Unity控制台日志服务提供者
    /// </summary>
    internal sealed class UnityConsoleLogServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<ILogService, UnityConsoleLogService>();
        }

        public override void Init()
        {
            base.Init();
            var _ = LogService.That; // 初始化日志服务门面
        }
    }
}