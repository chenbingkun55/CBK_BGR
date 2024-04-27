using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Extensions;
using CBK.Framework.Fsm;
using CBK.Framework.Procedure.Fsm;
using CBK.Framework.Update;

// ReSharper disable SuspiciousTypeConversion.Global
namespace CBK.Framework.Business
{
    /// <summary>
    /// 提供业务封装的流程抽象类
    /// </summary>
    public abstract class ABusinessProcedure : AFsmProcedure
    {
        public override void OnEnter(IFsm fsm)
        {
            if (m_ViewControllers == null)
            {
                m_ViewControllers = new List<IViewController>();
                OnAddControllers(m_ViewControllers);

                foreach (var controller in m_ViewControllers)
                {
                    if (controller is IUpdate update)
                        (m_Updates ??= new List<IUpdate>()).Add(update);

                    if (controller is ILoadable loadable)
                    {
                        if (controller is ISequenceLoadable sequenceLoadable)
                            (m_SequenceLoadableList ??= new List<ISequenceLoadable>()).Add(sequenceLoadable);
                        else
                            (m_ParallelLoadableList ??= new List<ILoadable>()).Add(loadable);
                    }
                    
                    if (controller is ILoadCompleteListener listener)
                        (m_LoadCompleteListenerList ??= new List<ILoadCompleteListener>()).Add(listener);
                }
            }

            var container = CatLib.App.That;

            foreach (var controller in m_ViewControllers)
            {
                container.Inject(controller);

                try
                {
                    controller.Initialize();
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
            }
            
            LoadAsync().Forget();
        }

        public override void OnLeave(IFsm fsm)
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

            if (m_TokenSource != null)
            {
                m_TokenSource.Cancel();
                m_TokenSource.Dispose();
                m_TokenSource = null;
            }
        }

        public override void OnUpdate(IFsm fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(fsm, elapseSeconds, realElapseSeconds);

            if (m_Updates == null)
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
        /// 进行需要加载的业务流程资源加载
        /// </summary>
        private async UniTask LoadAsync()
        {
            if (m_ParallelLoadableList != null || m_SequenceLoadableList != null)
            {
                var token = Token;

                // 先进行顺序加载
                if (m_SequenceLoadableList != null)
                {
                    foreach (var sequenceLoadable in m_SequenceLoadableList)
                    {
                        try
                        {
                            await sequenceLoadable.LoadAsync(token);
                        }
                        catch (Exception e)
                        {
                            Log.Exception(e);
                        }

                        if (token.IsCancellationRequested)
                            return;
                    }
                }

                // 然后进行并行加载
                if (m_ParallelLoadableList != null)
                {
                    if (m_ParallelLoadTasks == null)
                        m_ParallelLoadTasks = new List<UniTask>(m_ParallelLoadableList.Count);
                    else
                        m_ParallelLoadTasks.Clear();

                    foreach (var parallelLoadable in m_ParallelLoadableList)
                        m_ParallelLoadTasks.Add(parallelLoadable.LoadAsync(token));

                    await UniTask.WhenAll(m_ParallelLoadTasks);
                
                    if (token.IsCancellationRequested)
                        return;
                }
            }
            
            // 通知当前流程加载完成
            if (m_LoadCompleteListenerList != null)
            {
                foreach (var listener in m_LoadCompleteListenerList)
                {
                    try
                    {
                        listener.OnLoadComplete();
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }
                }
            }
            
            // 回调业务流程加载完成
            OnLoadComplete();
        }

        /// <summary>
        /// 派生类在该方法中添加所需的视图控制器
        /// </summary>
        protected virtual void OnAddControllers(List<IViewController> controllers)
        {
        }

        /// <summary>
        /// 流程业务加载完成回调
        /// </summary>
        protected virtual void OnLoadComplete()
        {
        }

        private List<IViewController> m_ViewControllers;
        private List<IUpdate> m_Updates;
        private List<ILoadable> m_ParallelLoadableList;
        private List<ISequenceLoadable> m_SequenceLoadableList;
        private List<UniTask> m_ParallelLoadTasks;
        private List<ILoadCompleteListener> m_LoadCompleteListenerList;

        protected CancellationToken Token => (m_TokenSource ??= new CancellationTokenSource()).Token;
        private CancellationTokenSource m_TokenSource;
    }
}