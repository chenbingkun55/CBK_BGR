using System;

namespace CBK.Framework.Procedure.Attribute
{
    /// <summary>
    /// 流程特性 标记了该特性的类将会被注册为流程
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ProcedureAttribute : System.Attribute
    {
        /// <summary>
        /// 是否为入口流程 默认为false 如果为true 则该流程将会被注册为入口流程 只能有一个入口流程 否则会抛出异常
        /// </summary>
        public bool IsEntry { get; private set; }
        
        public ProcedureAttribute(bool isEntry = false)
        {
            IsEntry = isEntry;
        }
    }
}