using System;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由查找器接口 用于按规则查找项目中有哪些请求要注册路由
    /// </summary>
    public interface IRouteFinder
    {
        /// <summary>
        /// 查找路由请求
        /// </summary>
        void FindRouteRequests(Action<string, Type> forEach);
    }
}