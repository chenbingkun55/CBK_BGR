using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.CoroutineLock;
using CBK.Framework.Reference;
using CBK.Framework.Update;
using CBK.Framework.Utility;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面服务实现
    /// </summary>
    internal partial class UIServiceImpl : IInitialize, IDisposable, IUIService, IUpdate
    {
        [Inject]
        public IUpdateService UpdateService { get; set; }

        [Inject]
        public ICoroutineLockService CoroutineLockService { get; set; }

        [Inject]
        public IUIFormHelper UIFormHelper { get; set; }

        [Inject]
        public IUIGroupConfiguration UIGroupConfiguration { get; set; }

        private readonly Dictionary<string, UIGroup> m_UIGroups = new(StringComparer.Ordinal);
        private readonly Dictionary<string, CancellationTokenSource> m_LoadingCancellationTokenSources = new(StringComparer.Ordinal);
        private readonly Dictionary<string, DisposableRefCounter> m_LoadingUIFormRefCounters = new(StringComparer.Ordinal);
        private readonly Dictionary<string, IUIForm> m_LoadedUIForms = new(StringComparer.Ordinal);
        private readonly HashSet<string> m_RemoveBuffer = new();

        private bool m_IsDisposing;

        public int UIGroupCount => m_UIGroups.Count;

        public void Initialize()
        {
            m_IsDisposing = false;

            var depth = 0;
            foreach (var groupName in UIGroupConfiguration.GroupNames)
                AddUIGroup(groupName, depth++);

            UpdateService.RegisterUpdate(this);
        }

        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);

            m_IsDisposing = true;
            CloseAllUIForms();
            DestroyUnusedUIForms();
            
            foreach (var mUIGroup in m_UIGroups)
                mUIGroup.Value.Helper.Dispose();
            
            m_UIGroups.Clear();
            m_LoadingCancellationTokenSources.Clear();
            m_LoadedUIForms.Clear();
            m_LoadingUIFormRefCounters.Clear();
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var uiGroup in m_UIGroups)
                uiGroup.Value.Update(elapseSeconds, realElapseSeconds);
        }

        private bool HasUIGroup(string uiGroupName)
        {
            return m_UIGroups.ContainsKey(uiGroupName);
        }

        private IUIGroup GetUIGroup(string uiGroupName)
        {
            return m_UIGroups.TryGetValue(uiGroupName, out var uiGroup) ? uiGroup : null;
        }

        private void AddUIGroup(string uiGroupName, int uiGroupDepth)
        {
            if (string.IsNullOrEmpty(uiGroupName))
                throw new System.Exception("UI group name is invalid.");

            if (HasUIGroup(uiGroupName)) 
                return;

            var groupHelper = CatLib.App.That.Make<IUIGroupHelper>();
            groupHelper.Initialize(uiGroupName);
            m_UIGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, groupHelper));
        }

        public bool HasUIForm(string uiKey)
        {
            foreach (var uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUIForm(uiKey))
                    return true;
            }

            return false;
        }

        public IUIForm[] GetAllLoadedUIForms()
        {
            var results = new List<IUIForm>();
            foreach (var uiGroup in m_UIGroups)
                results.AddRange(uiGroup.Value.GetAllUIForms());

            return results.ToArray();
        }

        public void GetAllLoadedUIForms(List<IUIForm> results)
        {
            results.Clear();
            foreach (var uiGroup in m_UIGroups)
                uiGroup.Value.InternalGetAllUIForms(results);
        }

        public bool IsLoadingUIForm(string uiKey)
        {
            return m_LoadingUIFormRefCounters.TryGetValue(uiKey, out var refCounter) && refCounter.RefCount > 0;
        }

        public async UniTask<int> OpenUIForm(string uiKey, object userdata = null, CancellationToken cancellationToken = default)
        {
            if (!m_LoadingUIFormRefCounters.TryGetValue(uiKey, out var refCounter))
                m_LoadingUIFormRefCounters[uiKey] = refCounter = new DisposableRefCounter();

            using var _ = refCounter.AddRef();

            if (!m_LoadingCancellationTokenSources.TryGetValue(uiKey, out var cancellationTokenSource))
                m_LoadingCancellationTokenSources[uiKey] = cancellationTokenSource = new CancellationTokenSource();

            var batchToken = cancellationTokenSource.Token;
            using var mergeTokenSource = CancellationTokenSource.CreateLinkedTokenSource(batchToken, cancellationToken);
            var mergeToken = mergeTokenSource.Token;

            // 打开相同UI的协程锁
            using var __ = await CoroutineLockService.Allocate(GameFrameworkCoroutineLockType.OpenUI, uiKey.GetHashCode(), mergeToken);

            if (cancellationToken.IsCancellationRequested)
                return (int)FrameworkErrorCode.RequestHasBeenCanceled;

            if (batchToken.IsCancellationRequested)
                return (int)FrameworkErrorCode.UIHasBeenClosed;

            if (!m_LoadedUIForms.TryGetValue(uiKey, out var uiForm))
            {
                uiForm = await UIFormHelper.CreateUIForm(uiKey, batchToken);
                
                if (batchToken.IsCancellationRequested)
                    return (int)FrameworkErrorCode.UIHasBeenClosed;
                    
                if (uiForm == null)
                {
                    Log.Error($"Can not create UI form '{uiKey}'.");
                    return (int)FrameworkErrorCode.UIResourceLoadFailed;
                }

                m_LoadedUIForms.Add(uiKey, uiForm);

                var uiGroup = GetUIGroup(uiForm.UIGroupName);
                if (uiGroup == null)
                {
                    Log.Error($"Can not find UI group '{uiForm.UIGroupName}' for UI form '{uiKey}'.");
                    return (int)FrameworkErrorCode.UIGroupUndefined;
                }

                uiForm.OnInit(uiGroup, userdata);
                
                if (cancellationToken.IsCancellationRequested)
                    return (int)FrameworkErrorCode.RequestHasBeenCanceled;
            }

            // 全局UI同时Open的协程锁 用于处理嵌套打开UI的问题
            using var ___ = await CoroutineLockService.Allocate(GameFrameworkCoroutineLockType.OpenUI, 0, mergeToken);

            if (cancellationToken.IsCancellationRequested)
                return (int)FrameworkErrorCode.RequestHasBeenCanceled;

            if (batchToken.IsCancellationRequested)
                return (int)FrameworkErrorCode.UIHasBeenClosed;

            {
                var uiGroup = (UIGroup)uiForm.UIGroup;
                if (uiGroup.HasUIForm(uiKey))
                {
                    // 如果UI已经打开了，那么就是重新聚焦
                    uiGroup.RefocusUIForm(uiForm);
                    uiGroup.Refresh();
                    uiForm.OnRefocus(userdata);
                }
                else
                {
                    // 如果UI没有打开，那么就是打开
                    uiGroup.AddUIForm(uiForm);
                    uiForm.OnOpen(userdata);
                    uiGroup.Refresh();
                }
            }

            return (int)FrameworkErrorCode.Success;
        }

        public void CloseUIForm(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
                throw new System.Exception("UI form is invalid.");

            var uiGroup = (UIGroup)uiForm.UIGroup;
            if (uiGroup == null)
                throw new System.Exception("UI group is invalid.");
            
            if (!uiGroup.HasUIForm(uiForm.UIKey))
                return;

            uiGroup.RemoveUIForm(uiForm);
            uiForm.OnClose(m_IsDisposing, userData);
            uiGroup.Refresh();
        }

        public void CloseUIForm(string uiKey, object userData = null)
        {
            // 加载中的UI
            if (m_LoadingCancellationTokenSources.TryGetValue(uiKey, out var cancellationTokenSource))
            {
                m_LoadingCancellationTokenSources.Remove(uiKey);
                
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                
                m_LoadingUIFormRefCounters.Remove(uiKey);
            }
            
            // 已经加载的UI
            if (m_LoadedUIForms.TryGetValue(uiKey, out var uiForm))
                CloseUIForm(uiForm, userData);
        }

        public void CloseAllUIForms(object userData = null)
        {
            CloseAllLoadingUIForms();
            CloseAllLoadedUIForms(userData);
        }

        public void CloseAllLoadedUIForms(object userData)
        {
            var uiForms = GetAllLoadedUIForms();
            foreach (var uiForm in uiForms)
                CloseUIForm(uiForm, userData);
        }

        private void CloseAllLoadingUIForms()
        {
            foreach (var cancellationTokenSource in m_LoadingCancellationTokenSources.Values)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            m_LoadingCancellationTokenSources.Clear();
            m_LoadingUIFormRefCounters.Clear();
        }

        public void DestroyUnusedUIForms()
        {
            m_RemoveBuffer.Clear();

            foreach (var (key, uiForm) in m_LoadedUIForms)
            {
                if (((UIGroup)uiForm.UIGroup).HasUIForm(key))
                    continue;

                m_RemoveBuffer.Add(key);
                uiForm.OnRecycle();
                UIFormHelper.RecycleUIForm(uiForm);
            }

            foreach (var key in m_RemoveBuffer)
                m_LoadedUIForms.Remove(key);
        }

        public void RefocusUIForm(string uiKey, object userData = null)
        {
            if (m_LoadedUIForms.TryGetValue(uiKey, out var uiForm))
                RefocusUIForm(uiForm, userData);
        }

        public void RefocusUIForm(IUIForm uiForm, object userData)
        {
            if (uiForm == null)
                throw new System.Exception("UI form is invalid.");

            var uiGroup = (UIGroup)uiForm.UIGroup;
            if (uiGroup == null)
                throw new System.Exception("UI group is invalid.");

            uiGroup.RefocusUIForm(uiForm);
            uiGroup.Refresh();
            uiForm.OnRefocus(userData);
        }
    }
}