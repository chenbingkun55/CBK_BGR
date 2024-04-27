namespace CBK.Framework.Localization
{
    /// <summary>
    /// 文本本地化实例接口
    /// </summary>
    public interface ITextLocalization : ILocalization
    {
        /// <summary>
        /// 获取当前语言指定文本
        /// </summary>
        string GetText(string key);
    }
}