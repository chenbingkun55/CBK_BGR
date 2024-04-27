using CatLib;
using CatLib.Container;

namespace CBK.Framework.DynamicText
{
    /// <summary>
    /// 与FairyGUI相同实现的动态文本服务提供者
    /// </summary>
    public sealed class FairyGUIDynamicTextServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IDynamicTextService, FairyGUIDynamicTextService>();
        }
    }
}