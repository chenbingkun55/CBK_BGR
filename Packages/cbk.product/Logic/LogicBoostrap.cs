using CatLib;
using CBK.Framework.App;
using CBK.Framework.Game;
using CBK.Framework.Network.MessagePack;
using CBK.Framework.UI.FairyGUI;
using CBK.Framework.WebServer;
// using CBK.Logic.Game;
// using CBK.Logic.UI;
// using CBK.Logic.UI.Waiting;
// using Server.Proto;

namespace CBK.Logic
{
    [AppBootstrap]
    public sealed class GameLogicBoostrap : IBootstrap
    {
        public void Bootstrap()
        {
            // App.That.UseFairyGUI();
            // App.That.Register(new GameUIConfigurationProvider());
            // App.Singleton<IMessageFactory, MsgFactory>();
            // App.Singleton<IWebServerConfiguration, WebServerConfiguration>();
            // App.Singleton<WaitingService>();
            // App.Singleton<IGame, IdleRpgGame>();
        }
    }
}