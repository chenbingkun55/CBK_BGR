using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.WebServer
{
    /// <summary>
    /// 中心服服务
    /// </summary>
    public interface IWebServerService
    {
        /// <summary>
        /// 发送WebServer请求
        /// </summary>
        UniTask<T> SendAsync<T>(WebServerReq<T> req, CancellationToken token) where T : WebServerResp, new();
    }
}