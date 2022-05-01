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
            data.Initialize();
            save.Initialize();

            return true;
        }

        public void Destroy()
        {
            data.Destroy();
            save.Destroy();
        }

        public Data.DataManager data = new Data.DataManager();
        public Save.SaveManager save = new Save.SaveManager();
    }
}