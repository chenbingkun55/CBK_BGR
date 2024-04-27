using System;
using CBK.Framework.Reflect;
using CBK.Framework.Request;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 基于特性规则的路由查找器实现
    /// </summary>
    public sealed class AttributeRouteFinder : IRouteFinder
    {
        public void FindRouteRequests(Action<string, Type> forEach)
        {
            ReflectService.That.ForEachTypeWithAttribute<RouteAttribute>(typeof(IRequest), (type, attribute) =>
            {
                forEach(attribute.Route, type);
            });
        }
    }
}