using CatLib;
using CatLib.Container;

namespace CBK.Framework.Json
{
    /// <summary>
    /// CatJson Json服务提供者
    /// </summary>
    internal sealed class CatJsonServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IJsonService, CatJsonService>();
            App.Bind<IJsonCore>(() => JsonService.That.Allocate());
        }
    }
}