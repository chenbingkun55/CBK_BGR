
namespace CBK
{
    class App
    {
        public static App Instantiate
        {
            get{
                if(Instantiate == null)
                {
                    Instantiate = new App();
                }

                return Instantiate;
            }
        }
        

        public FileManager file = new FileManager();
        public SaveSystem save = new SaveSystem();

    }
}