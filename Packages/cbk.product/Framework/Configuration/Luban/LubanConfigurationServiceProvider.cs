using CatLib;
using CatLib.Container;

namespace CBK.Framework.Configuration.Luban
{
    /// <summary>
    /// Luban配置服务提供者
    /// </summary>
    internal sealed class LubanConfigurationServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IConfigurationService, LubanConfigurationService>();
        }
    }
}