namespace CBK.Framework.DynamicText
{
    /// <summary>
    /// 动态文本服务接口
    /// </summary>
    public interface IDynamicTextService
    {
        /// <summary>
        /// 分配动态文本核心实例
        /// </summary>
        IDynamicTextCore Allocate(string source);
    }
}