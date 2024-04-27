using CatLib;
using CatLib.Container;

namespace CBK.Framework.Http
{
    /// <summary>
    /// 基于UnityWebRequest的Http服务提供者
    /// </summary>
    public sealed class UnityHttpServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IHttpService, UnityHttpService>();
        }
    }
}