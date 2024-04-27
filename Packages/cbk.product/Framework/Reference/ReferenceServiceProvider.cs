using CatLib;
using CatLib.Container;

namespace CBK.Framework.Reference
{
    /// <summary>
    /// 引用服务提供者
    /// </summary>
    internal sealed class ReferenceServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            base.Register();
            App.Singleton<IReferenceService, ReferenceServiceImpl>();
        }
    }
}