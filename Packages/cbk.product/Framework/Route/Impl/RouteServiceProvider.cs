using CatLib;
using CatLib.Container;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由服务提供者
    /// </summary>
    internal sealed class RouteServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IRouteService, RouteServiceImpl>();
        }
    }
}