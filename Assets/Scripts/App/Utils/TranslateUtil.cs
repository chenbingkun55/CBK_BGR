using System;
using CBK.Product.Config;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement;

namespace CBK.Product.Utils
{
    public class TranslateUtil
    {
        /// <summary>
        /// 获取翻译内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetText(string strKey, params object[] args)
        {
            var languageCfg = App.Instantiate.config[ConfigType.Language] as LanguageConfig;
            
            return languageCfg.GetLocalizedString(strKey, args);
        }
    }
}