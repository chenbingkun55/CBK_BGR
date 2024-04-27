using Cysharp.Threading.Tasks;
using CBK.Framework.Request;
using CancellationToken = System.Threading.CancellationToken;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由服务拓展方法
    /// </summary>
    public static class RouteServiceExtensions
    {
        /// <summary>
        /// 执行指定的路由请求
        /// </summary>
        public static UniTask<IResponse> Execute(this IRouteService service, string route, CancellationToken token = default)
        {
            var response = service.MakeRequest(route);
            if (!response.IsSuccess())
                return UniTask.FromResult<IResponse>(response);

            var request = response.Request;
            response.Request = null;
            return request.Execute(token);
        }

        /// <summary>
        /// 执行指定的路由请求 并获取指定类型的应答包
        /// </summary>
        public static async UniTask<T> Execute<T>(this IRouteService service, string route, CancellationToken token = default) where T : AResponse, new()
        {
            var response = await service.Execute(route, token);
            return response.ParseResponse<T>();
        }

        /// <summary>
        /// 通过字符串创建路由请求
        /// </summary>
        public static RequestMakeResponse MakeRequest(this string route)
        {
            return RouteService.That.MakeRequest(route);
        }

        /// <summary>
        /// 通过字符串执行指定的路由请求
        /// </summary>
        public static UniTask<IResponse> ExecuteRequest(this string route)
        {
            return RouteService.That.Execute(route);
        }

        /// <summary>
        /// 通过字符串执行指定的路由请求
        /// </summary>
        public static UniTask<T> ExecuteRequest<T>(this string route) where T : AResponse, new()
        {
            return RouteService.That.Execute<T>(route);
        }
    }
}