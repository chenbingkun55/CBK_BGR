using CatLib;
using CatLib.Container;

namespace CBK.Framework.Rpc
{
    /// <summary>
    /// RPC服务提供者
    /// </summary>
    public sealed class RpcServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IRpcService, RpcServiceImpl>();
        }
    }
}