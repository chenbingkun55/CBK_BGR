namespace CBK.Framework.Localization
{
    internal static class LocalizationUtility
    {
        /// <summary>
        /// 拼接资源键值
        /// </summary>
        public static string ResolveAssetKey(string asset)
        {
            return $"Localization_{asset}";
        }
    }
}