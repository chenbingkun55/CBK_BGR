using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.Http
{
    /// <summary>
    /// Htto服务
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// 发送Get请求
        /// </summary>
        UniTask<HttpResponse> Get(string api, CancellationToken token);

        /// <summary>
        /// 发送Post请求
        /// </summary>
        UniTask<HttpResponse> Post(string api, string param, CancellationToken token);
    }
}