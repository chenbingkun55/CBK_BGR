using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CatLib;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Entry;
using CBK.Framework.Business;
using CBK.Framework.Core;
using CBK.Framework.Extensions;
using CBK.Framework.Procedure;
using CBK.Framework.Update;
using UnityEngine;
using Application = CatLib.Application;

namespace CBK.Framework.App
{
    public abstract class AApp : MonoBehaviour, IApp, IBootstrap, IUpdate
    {
        private Application m_Application;
        private IEntryLoader m_entryLoader;

        private readonly List<IViewController> m_ViewControllers = new();
        private readonly List<IUpdate> m_Updates = new();
        
        public void Init(IEntryLoader loader)
        {
            m_entryLoader = loader;
            var bootstraps = GetAppBootstraps(m_entryLoader.CurrentAssembles);
            bootstraps.Insert(0, this);

            CatLib.App.That = m_Application = new Application();
            CatLib.App.OnNewApplication += OnNewApplication;
            
            m_Application.GetDispatcher().AddListener(ApplicationEvents.OnStartCompleted, OnStartCompleted);
            m_Application.OnAfterResolving<IInitialize>(inst => inst.Initialize());
            m_Application.Bootstrap(bootstraps.ToArray());
            m_Application.Init();
        }

        private void OnNewApplication(IApplication application)
        {
            if (application == m_Application)
                return;
            
            DestroyImmediate(gameObject);
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            for (var index = m_ViewControllers.Count - 1; index >= 0; --index)
            {
                var controller = m_ViewControllers[index];
                
                try
                {
                    controller.Dispose();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
            
            CatLib.App.OnNewApplication -= OnNewApplication;
            m_Application.Terminate();
            m_Application = null;
        }

        public void Reboot()
        {
            var loader = m_entryLoader;

            DestroyImmediate(gameObject);

            loader.LoadGameAssemblies().Forget();
        }

        public void Quit(int errorCode = 0)
        {
            DestroyImmediate(gameObject);
            UnityEngine.Application.Quit(errorCode);
        }

        public void Bootstrap()
        {
            CatLib.App.Instance<IApp>(this);
            CatLib.App.Instance<IEntryLoader>(m_entryLoader);
        }

        private static List<IBootstrap> GetAppBootstraps(IEnumerable<Assembly> assemblies)
        {
            var bootstrapType = typeof(IBootstrap);
            var bootstraps = (from assembly in assemblies from type in assembly.GetTypes() where type.IsClass && !type.IsAbstract where bootstrapType.IsAssignableFrom(type) let attribute = type.GetCustomAttribute<AppBootstrapAttribute>() where attribute != null let priority = attribute.Priority let bootstrap = (IBootstrap)Activator.CreateInstance(type) select (bootstrap, priority)).ToList();

            int Comparison((IBootstrap bootstrap, int priority) x, (IBootstrap bootstrap, int priority) y)
            {
                return y.priority.CompareTo(x.priority);
            }

            bootstraps.Sort(Comparison);
            return bootstraps.Select(item => item.bootstrap).ToList();
        }

        private void OnStartCompleted(object sender, EventArgs _)
        {
            OnAddControllers(m_ViewControllers);
                
            foreach (var controller in m_ViewControllers)
            {
                try
                {
                    m_Application.Inject(controller);
                    controller.Initialize();
                    
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (controller is IUpdate update)
                        m_Updates.Add(update);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
            
            if (m_Updates.Count > 0)
                UpdateService.That.RegisterUpdate(this);
            
            ProcedureService.That.Run(m_entryLoader.CurrentAssembles);
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Updates.Count == 0)
                return;

            foreach (var update in m_Updates)
            {
                try
                {
                    update.OnUpdate(elapseSeconds, realElapseSeconds);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
        }

        /// <summary>
        /// 派生类在该方法中添加所需的视图控制器
        /// </summary>
        protected virtual void OnAddControllers(List<IViewController> controllers)
        {
        }
    }
}