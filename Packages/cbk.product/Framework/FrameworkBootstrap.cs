using CatLib;
using CBK.Framework.App;
using CBK.Framework.Camera.Standard;
using CBK.Framework.Configuration.Luban;
using CBK.Framework.CoroutineLock;
using CBK.Framework.Database.SQLite;
using CBK.Framework.DebugCommand;
using CBK.Framework.DynamicText;
using CBK.Framework.Event;
using CBK.Framework.File.BetterStreaming;
using CBK.Framework.Fsm;
// using CBK.Framework.GameServer;
using CBK.Framework.Http;
using CBK.Framework.Json;
using CBK.Framework.Localization;
using CBK.Framework.Network;
using CBK.Framework.ObjectPool;
using CBK.Framework.Path.Unity;
using CBK.Framework.Persistence.Database;
using CBK.Framework.Procedure.Fsm;
using CBK.Framework.Reference;
using CBK.Framework.Reflect;
using CBK.Framework.Res.YooAsset;
using CBK.Framework.Route;
using CBK.Framework.Rpc;
using CBK.Framework.Scene.YooAsset;
using CBK.Framework.ScreenAdaptor;
using CBK.Framework.Timer;
using CBK.Framework.UI;
using CBK.Framework.UnityConsole;
using CBK.Framework.Update;
using CBK.Framework.WebServer;

namespace GameFramework
{
    [AppBootstrap(Priority = int.MaxValue)]
    public sealed class GameFrameworkBootstrap : IBootstrap
    {
        public void Bootstrap()
        {
            CatLib.App.That.Register(new UnityConsoleLogServiceProvider());
            CatLib.App.That.Register(new CatLibEventServiceProvider());
            CatLib.App.That.Register(new UnityPathServiceProvider());
            CatLib.App.That.Register(new BetterStreamingFileServiceProvider());
            CatLib.App.That.Register(new ReflectServiceProvider());
            CatLib.App.That.Register(new ReferenceServiceProvider());
            CatLib.App.That.Register(new UnityHttpServiceProvider());
            CatLib.App.That.Register(new WebServerServiceProvider());
            CatLib.App.That.Register(new RpcServiceProvider());
            CatLib.App.That.Register(new RouteServiceProvider());
            CatLib.App.That.Register(new CoroutineLockServiceProvider());
            CatLib.App.That.Register(new YooAssetResServiceProvider());
            CatLib.App.That.Register(new YooAssetSceneServiceProvider());
            CatLib.App.That.Register(new ObjectPoolServiceProvider());
            CatLib.App.That.Register(new UpdateServiceProvider());
            CatLib.App.That.Register(new FsmServiceProvider());
            CatLib.App.That.Register(new FsmProcedureServiceProvider());
            CatLib.App.That.Register(new LubanConfigurationServiceProvider());
            CatLib.App.That.Register(new LocalizationServiceProvider());
            CatLib.App.That.Register(new SQLiteDatabaseServiceProvider());
            CatLib.App.That.Register(new DatabasePersistenceServiceServiceProvider());
            CatLib.App.That.Register(new TimerServiceProvider());
            CatLib.App.That.Register(new UIServiceProvider());
            CatLib.App.That.Register(new CatJsonServiceProvider());
            CatLib.App.That.Register(new FairyGUIDynamicTextServiceProvider());
            CatLib.App.That.Register(new NetworkServiceProvider());
            // CatLib.App.That.Register(new GameServerServiceProvider());
            CatLib.App.That.Register(new StandardCameraServiceProvider());
            CatLib.App.That.Register(new ScreenServiceProvider());
            CatLib.App.That.Register(new DebugCommandServiceProvider());
        }
    }
}