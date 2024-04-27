namespace CBK.Framework.WebServer
{
    /// <summary>
    /// Web服务器配置
    /// </summary>
    public interface IWebServerConfiguration
    {
        /// <summary>
        /// 中心服地址
        /// </summary>
        string Domain { get; }
    }
}