using CatLib;
using CatLib.Container;

namespace CBK.Framework.Reflect
{
    /// <summary>
    /// 反射服务提供者
    /// </summary>
    internal sealed class ReflectServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IReflectService, ReflectServiceImpl>();
        }
    }
}