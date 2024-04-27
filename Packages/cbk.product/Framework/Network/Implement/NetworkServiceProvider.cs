using CatLib;
using CatLib.Container;

namespace CBK.Framework.Network
{
    internal sealed class NetworkServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<INetworkService, NetworkServiceImpl>();
        }
    }
}