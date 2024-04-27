using System;
using CBK.Framework.Reflect;
using CBK.Framework.UI;

namespace CBK.Framework.Route
{
    /// <summary>
    /// UI路由查找器
    /// </summary>
    public sealed class UIRouteFinder : IRouteFinder
    {
        public void FindRouteRequests(Action<string, Type> forEach)
        {
            ReflectService.That.ForEachType<IUIRequest>(type =>
            {
                if (type.IsAbstract)
                    return;
                
                forEach(GetRoute(type), type);
            });
        }
        
        private string GetRoute(Type type)
        {
            var typeName = type.Name;
            
            if (typeName.EndsWith("UIRequest"))
                typeName = typeName.Substring(0, typeName.Length - "UIRequest".Length);
            else if (typeName.EndsWith("Request"))
                typeName = typeName.Substring(0, typeName.Length - "Request".Length);
            
            var route = $"uiForm://{typeName}";
            return route;
        }
    }
}