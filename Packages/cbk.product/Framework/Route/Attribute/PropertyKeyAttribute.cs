using System;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由属性键值标记 用于指定路由参数的键值 未标记该特性的属性将使用属性名作为键值
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyKeyAttribute : Attribute
    {
        /// <summary>
        /// 键值
        /// </summary>
        public string Key { get; }

        public PropertyKeyAttribute(string key)
        {
            Key = key;
        }
    }
}