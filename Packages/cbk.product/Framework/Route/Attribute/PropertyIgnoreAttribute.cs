using System;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由属性忽略标记 标记了该特性的属性将不参与路由参数解析
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyIgnoreAttribute : Attribute
    {
    }
}