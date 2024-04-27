namespace CBK.Framework.UI.FairyGUI
{
    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="fairyGUIForm">FairyGUIForm实例</param>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnFormInit(FairyGUIForm fairyGUIForm, object userData)
        {
            UIService = UI.UIService.That;
            UIForm = fairyGUIForm;
            ContentPane = fairyGUIForm.ContentPane;
            Name = fairyGUIForm.UIKey;
            
            _OnInitScreenAdapter();
            _OnInitFormFrame();
            
            OnInit(userData);
        }

        /// <summary>
        /// 界面回收。
        /// </summary>
        internal void OnFormRecycle()
        {
            OnRecycle();
            
            _OnRecycleFormFrame();
            _OnRecycleScreenAdapter();
            _OnRecycleCancellationToken();
            _OnRecycleDisposableGroup();
        }

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnFormOpen(object userData)
        {
            OnOpen(userData);
        }

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnFormClose(bool isShutdown, object userData)
        {
            OnClose(isShutdown, userData);
            
            _OnCloseCancelToken();
            _OnCloseDisposableGroup();
        }

        /// <summary>
        /// 界面暂停。
        /// </summary>
        internal void OnFormPause()
        {
            OnPause();
            
            _OnPauseCancellationToken();
            _OnPauseDisposableGroup();
        }

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        internal void OnFormResume()
        {
            OnResume();
        }

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        internal void OnFormCover()
        {
            OnCover();
        }

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        internal void OnFormReveal()
        {
            OnReveal();
        }

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        internal void OnFormRefocus(object userData)
        {
            OnRefocus(userData);
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal void OnFormUpdate(float elapseSeconds, float realElapseSeconds)
        {
            OnUpdate(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        internal void OnFormDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }

        /// <summary>
        /// 界面遮罩层点击。
        /// </summary>
        internal void OnFormMaskLayerClicked()
        {
            OnMaskLayerClick();
        }
    }
}