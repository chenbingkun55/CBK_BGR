using System;

namespace CBK.Framework.DynamicText
{
    /// <summary>
    /// 动态文本核心接口
    /// </summary>
    public interface IDynamicTextCore : IDisposable
    {
        /// <summary>
        /// 设置文本变量
        /// </summary>
        IDynamicTextCore SetVar(string name, string value);
        
        /// <summary>
        /// 根据当前设置的变量刷新文本 并指定是否释放当前实例
        /// </summary>
        string FlushVars(bool dispose = true);
    }
}