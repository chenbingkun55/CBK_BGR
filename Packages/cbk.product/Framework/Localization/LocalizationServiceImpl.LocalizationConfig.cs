using System;
using Bright.Serialization;
using UnityEngine;

namespace CBK.Framework.Localization
{
    internal sealed partial class LocalizationServiceImpl
    {
        private void LoadConfig()
        {
            m_Configs.Clear();
            m_DictConfigs.Clear();
            m_CurrentConfig = null;
            Clean();

            var bytes = ResService.LoadRawSync(LocalizationUtility.ResolveAssetKey("config"));
            var buffer = new ByteBuf(bytes);
            for (var n = buffer.ReadSize(); n > 0; --n)
            {
                var config = LocalizationConfig.DeserializeConfig(buffer);
                m_Configs.Add(config);
                m_DictConfigs.Add(config.Id, config);
            }

            // 优先使用持久化数据读取之前选择的语言
            var id = PersistenceData.GetInt(PersistenceIdentifier, -1);
            if (!m_DictConfigs.TryGetValue(id, out m_CurrentConfig) || m_CurrentConfig == null)
            {
                // 其次根据系统语言选择
                var language = Application.systemLanguage;
                foreach (var cfg in m_Configs)
                {
                    if (Array.IndexOf(cfg.Languages, language) < 0)
                        continue;

                    m_CurrentConfig = cfg;
                    break;
                }

                m_CurrentConfig ??= m_Configs[0];
            }
        }

        private sealed class LocalizationConfig : ILocalizationConfig
        {
            public int Id { get; }
            public string Name { get; }
            public string LangCode { get; }
            public SystemLanguage[] Languages { get; }

            public LocalizationConfig(ByteBuf _buf)
            {
                Id = _buf.ReadInt();
                Name = _buf.ReadString();
                LangCode = _buf.ReadString();
                {
                    int __n0 = System.Math.Min(_buf.ReadSize(), _buf.Size);
                    Languages = new SystemLanguage[__n0];
                    for (var __index0 = 0; __index0 < __n0; __index0++)
                    {
                        int __e0;
                        __e0 = _buf.ReadInt();
                        Languages[__index0] = (SystemLanguage)__e0;
                    }
                }
            }

            public static LocalizationConfig DeserializeConfig(ByteBuf _buf)
            {
                return new LocalizationConfig(_buf);
            }
        }
    }
}