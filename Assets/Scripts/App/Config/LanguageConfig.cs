using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using CBK.Product;

namespace CBK.Product.Config
{
    public class LanguageConfig : IConfig
    {
        private readonly string kStringTableCollectionName = "StringTable";
        private StringTable m_StringTable = default;


        public bool Initialize()
        {
            return LoadStrings();
        }

        public IEnumerable InitializeAsync()
        {
            yield return null;
        }

        public void Destroy()
        {
            m_StringTable = default;
        }

        // 加载多语言 StringTable
        public bool LoadStrings()
        {
            m_StringTable = LocalizationSettings.StringDatabase.GetTable(kStringTableCollectionName);

            if (m_StringTable == null)
            {
                Debug.LogError("Could not load String Table\n" + kStringTableCollectionName);
            }

            return m_StringTable != null;
        }

        // 获取多语内容
        public string GetLocalizedString(string strKey, params object[] args)
        {
            if(m_StringTable == default)
            {
                Debug.LogError("GetLocalizedString error, m_StringTable not Initialize.");
                return strKey;
            }

            var entry = m_StringTable.GetEntry(strKey);
            
            return (entry == null) ? strKey : entry.GetLocalizedString(args);
        }
    }
}