using System;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由属性非空标记 标记了该特性的属性将在路由参数解析时进行非空检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyNotNullAttribute : Attribute
    {
    }
}