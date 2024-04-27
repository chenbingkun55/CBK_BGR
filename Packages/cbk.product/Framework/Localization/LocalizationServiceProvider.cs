using CatLib;
using CatLib.Container;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化服务提供者
    /// </summary>
    internal sealed class LocalizationServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<ILocalizationService, LocalizationServiceImpl>();
        }
    }
}