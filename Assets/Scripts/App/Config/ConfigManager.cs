using System;
using System.Collections;
using System.Collections.Generic;

namespace CBK.Product.Config
{
    public class ConfigManager
    {
        private Dictionary<ConfigType, IConfig> m_configs = new Dictionary<ConfigType, IConfig>();

        public ConfigManager()
        {
            // Config 数据
            m_configs.Add(ConfigType.Language, new LanguageConfig());
        }

        public bool Initialize()
        {
            // 初始化数据
            foreach(var config in m_configs)
                config.Value.Initialize();

            return true;
        }

        public IEnumerable InitializeAsync()
        {
            // 初始化数据异步
            foreach(var config in m_configs)
                yield return config.Value.InitializeAsync();
        }

        public void Destroy()
        {
            // 销毁数据
            foreach(var config in m_configs)
                config.Value.Destroy();

            m_configs.Clear();
        }

        /// <summary>
        /// 数据访问索引器
        /// </summary>
        /// <value></value>
        public IConfig this[ConfigType eType]
        {
            get => m_configs[eType];
        }
    }
}