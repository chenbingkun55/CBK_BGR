using CatLib;
using CatLib.Container;

namespace CBK.Framework.WebServer
{
    /// <summary>
    /// 中心服服务提供者
    /// </summary>
    internal sealed class WebServerServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IWebServerService, WebServerServiceImpl>();
        }
    }
}