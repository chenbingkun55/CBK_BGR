using CatLib;
using CatLib.Container;

namespace CBK.Framework.Camera.Standard
{
    public sealed class StandardCameraServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<ICameraService, StandardCameraService>();
        }
    }
}