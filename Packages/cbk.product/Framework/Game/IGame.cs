using System;
using System.Collections.Generic;
using CBK.Framework.Core;
using CBK.Framework.Update;
using CBK.Framework.Utility;

namespace CBK.Framework.Game
{
    /// <summary>
    /// 玩法接口
    /// </summary>
    public interface IGame : IInitialize, IDisposable, IFixedUpdate
    {
    }

    /// <summary>
    /// 玩法抽象类
    /// </summary>
    public abstract class AGame : IGame
    {
        private readonly GameRepositoryRegister m_RepositoryRegister = new GameRepositoryRegister();
        private readonly GameSystemRegister m_SystemRegister = new GameSystemRegister();

        public void Initialize()
        {
            OnRegisterRepository(m_RepositoryRegister);
            OnRegisterSystem(m_SystemRegister);

            m_RepositoryRegister.Initialize();
            m_SystemRegister.Initialize();
            
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
            
            m_SystemRegister.Dispose();
            m_RepositoryRegister.Dispose();
        }

        public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
        {
            m_SystemRegister.OnFixedUpdate(elapseSeconds, realElapseSeconds);
        }

        protected abstract void OnRegisterRepository(IGameRepositoryRegister register);
        protected abstract void OnRegisterSystem(IGameSystemRegister register);
        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        /// <summary>
        /// 玩法仓库注册器
        /// </summary>
        protected interface IGameRepositoryRegister : IIOCContainer<IGameRepository>
        {
        }

        /// <summary>
        /// 玩法系统注册器
        /// </summary>
        protected interface IGameSystemRegister : IIOCContainer<IGameSystem>
        {
        }

        protected sealed class GameRepositoryRegister : AIOCContainer<IGameRepository>, IGameRepositoryRegister
        {
        }

        protected sealed class GameSystemRegister : AIOCContainer<IGameSystem>, IGameSystemRegister, IFixedUpdate
        {
            private readonly List<IFixedUpdate> m_FixedUpdateSystems = new List<IFixedUpdate>();

            protected override void AfterInitialize()
            {
                base.AfterInitialize();
                foreach (var instance in RegisterInstances)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (instance is IFixedUpdate fixedUpdate)
                        m_FixedUpdateSystems.Add(fixedUpdate);
                }
            }

            public void OnFixedUpdate(float elapseSeconds, float realElapseSeconds)
            {
                foreach (var update in m_FixedUpdateSystems)
                {
                    try
                    {
                        update.OnFixedUpdate(elapseSeconds, realElapseSeconds);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
                }
            }
        }
    }
}