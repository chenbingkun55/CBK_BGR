using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.WebServer
{
    /// <summary>
    /// 中心服请求基类 T为返回的响应类型
    /// </summary>
    public abstract class WebServerReq<T> : AReference, IRequest where T : WebServerResp, new()
    {
        /// <summary>
        /// 路由 用于拼接api地址 完整api地址=ip:port/game?api=Route
        /// </summary>
        public abstract string Route { get; }
        
        public async UniTask<IResponse> Execute(CancellationToken cancellationToken = default)
        {
            return await WebServerService.That.SendAsync<T>(this, cancellationToken);
        }
    }
}