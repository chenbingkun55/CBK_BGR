using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 请求实例拓展
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// 异步执行请求 并指定应答包类型 若应答包类型不匹配则会返回对应的错误码
        /// </summary>
        public static UniTask<T> Execute<T>(this IRequest request, System.Threading.CancellationToken cancellationToken = default) where T : AResponse, new()
        {
            return request.Execute(cancellationToken).ParseResponse<T>();
        }
    }
}