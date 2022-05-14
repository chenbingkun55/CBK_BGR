using System.Collections;
using CBK.Core;

namespace CBK.Product
{
    class App
    {
        private static App m_instantiate = default;
        public static App Instantiate
        {
            get
            {
                if(m_instantiate == null)
                {
                    m_instantiate = new App();
                }

                return m_instantiate;
            }
        }
        
        public bool Initialize()
        {
            config.Initialize();
            data.Initialize();
            save.Initialize();

            return true;
        }

        public IEnumerable InitializeAsync()
        {
            yield return config.InitializeAsync();
        }

        public void Destroy()
        {
            config.Destroy();
            data.Destroy();
            save.Destroy();
        }


        public Config.ConfigManager config = new Config.ConfigManager();
        public Data.DataManager data = new Data.DataManager();
        public Save.SaveManager save = new Save.SaveManager();
    }
}