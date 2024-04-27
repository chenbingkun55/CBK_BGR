namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由服务接口
    /// </summary>
    public interface IRouteService
    {
        /// <summary>
        /// 通过路由构造请求实例
        /// </summary>
        RequestMakeResponse MakeRequest(string route);
    }
}