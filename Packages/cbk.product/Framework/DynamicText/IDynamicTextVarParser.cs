namespace CBK.Framework.DynamicText
{
    /// <summary>
    /// 动态文本变量解析器接口
    /// </summary>
    public interface IDynamicTextVarParser
    {
        /// <summary>
        /// 绑定的变量名
        /// </summary>
        string VarName { get; }
        
        /// <summary>
        /// 获取解析结果
        /// </summary>
        string Parse();
    }
}