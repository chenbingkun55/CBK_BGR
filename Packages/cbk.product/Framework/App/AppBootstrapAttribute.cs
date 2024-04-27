using System;

namespace CBK.Framework.App
{
    /// <summary>
    /// 标记该特性的IBootstrap引导器将在GameApp初始化环节被触发引导
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AppBootstrapAttribute : Attribute
    {
        /// <summary>
        /// 优先级 将按照从大到小的顺序进行引导
        /// </summary>
        public int Priority { get; set; }

        public AppBootstrapAttribute(int priority = 0)
        {
            Priority = priority;
        }
    }
}