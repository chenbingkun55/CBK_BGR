using UnityEngine;

namespace CBK.Framework.Localization
{
    /// <summary>
    /// 本地化配置
    /// </summary>
    public interface ILocalizationConfig
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 语言名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 语言代码
        /// </summary>
        string LangCode { get; }

        /// <summary>
        /// 对应的语言枚举
        /// </summary>
        SystemLanguage[] Languages { get; }
    }
}