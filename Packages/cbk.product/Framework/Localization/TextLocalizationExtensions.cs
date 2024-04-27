namespace CBK.Framework.Localization
{
    /// <summary>
    /// 文本本地化扩展方法
    /// </summary>
    public static class TextLocalizationExtensions
    {
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        public static string GetLocalizedText(this string key)
        {
            return TextLocalization.That.GetText(key);
        }
    }
}