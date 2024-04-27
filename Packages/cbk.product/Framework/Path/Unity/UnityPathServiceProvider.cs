using CatLib;
using CatLib.Container;

namespace CBK.Framework.Path.Unity
{
    /// <summary>
    /// 基于Unity的路径服务提供者
    /// </summary>
    internal sealed class UnityPathServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IPathService, UnityPathServiceImpl>();
        }
    }
}