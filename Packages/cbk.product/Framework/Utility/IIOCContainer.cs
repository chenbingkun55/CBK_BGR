using System;
using System.Collections.Generic;
using CatLib.Container;
using CatLib.Exception;
using CBK.Framework.Core;
using CBK.Framework.Extensions;

namespace CBK.Framework.Utility
{
    /// <summary>
    /// IOC容器接口
    /// </summary>
    public interface IIOCContainer<TInterface> : IInitialize, IDisposable
    {
        /// <summary>
        /// 注册实例
        /// </summary>
        void Register<T>(T instance) where T : TInterface;

        /// <summary>
        /// 注册实例
        /// </summary>
        void Register<T>() where T : class, TInterface;

        /// <summary>
        /// 注册实例
        /// </summary>
        void Register<T, TImpl>() where T : TInterface where TImpl : class, T;

        /// <summary>
        /// 注册实例
        /// </summary>
        void Register(Type type, TInterface instance);
    }
    
    /// <summary>
    /// IOC容器抽象类
    /// </summary>
    public abstract class AIOCContainer<TInterface> : IIOCContainer<TInterface>
    {
        private bool m_IsInitialized;
        private bool m_IsDisposed;
        private readonly List<Type> m_RegisterTypes = new List<Type>();
        private readonly List<TInterface> m_RegisterInstances = new List<TInterface>();
        private readonly List<IBindData> m_BindDataList = new List<IBindData>();
        
        protected IReadOnlyList<TInterface> RegisterInstances => m_RegisterInstances;

        public void Initialize()
        {
            if (m_IsInitialized)
                throw new Exception("IOCContainer: Can't initialize twice.");

            m_IsInitialized = true;
            var container = CatLib.App.That;

            for (var i = 0; i < m_RegisterTypes.Count; i++)
            {
                var registerType = m_RegisterTypes[i];
                var registerInstance = m_RegisterInstances[i];
                
                try
                {
                    container.Inject(registerInstance);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }

                try
                {
                    var bindData = container.BindSingletonInstance(registerInstance, registerType);
                    m_BindDataList.Add(bindData);
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            try
            {
                AfterInitialize();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        public void Dispose()
        {
            if (m_IsDisposed)
                throw new Exception("IOCContainer: Can't dispose twice.");

            m_IsDisposed = true;
            
            try
            {
                BeforeDispose();
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }

            for (var i = m_BindDataList.Count - 1; i >= 0; i--)
            {
                var bindData = m_BindDataList[i];

                try
                {
                    bindData.Unbind();
                }
                catch (LogicException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }

            m_RegisterTypes.Clear();
            m_RegisterInstances.Clear();
            m_BindDataList.Clear();
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        public void Register<T>(T instance) where T : TInterface
        {
            Register(typeof(T), instance);
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        public void Register<T>() where T : class, TInterface
        {
            Register(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// 注册实例
        /// </summary>
        public void Register<T, TImpl>() where T : TInterface where TImpl : class, T
        {
            Register<T>(Activator.CreateInstance<TImpl>());
        }

        public void Register(Type type, TInterface instance)
        {
            if (m_IsInitialized)
                throw new Exception("IOCContainer: Can't register instance after initialized.");

            if (m_RegisterTypes.Contains(type))
                throw new Exception($"IOCContainer: {type} has been registered.");

            m_RegisterTypes.Add(type);
            m_RegisterInstances.Add(instance);
        }

        protected virtual void AfterInitialize(){}
        protected virtual void BeforeDispose(){}
    }
}