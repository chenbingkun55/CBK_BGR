using System;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由标记 通过该标记注册路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RouteAttribute : Attribute
    {
        /// <summary>
        /// 路由值
        /// </summary>
        public string Route { get; }

        public RouteAttribute(string route)
        {
            Route = route;
        }
    }
}