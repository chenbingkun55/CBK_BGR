using CatLib;
using CatLib.Container;

namespace CBK.Framework.File.BetterStreaming
{
    /// <summary>
    /// 基于BetterStreaming的文件服务提供者
    /// </summary>
    public sealed class BetterStreamingFileServiceProvider : ServiceProvider
    {
        public override void Register()
        {
            App.Singleton<IFileService, BetterStreamingFileServiceImpl>();
        }
    }
}