namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面接口。
    /// </summary>
    public interface IUIForm
    {
        /// <summary>
        /// 获取界面键值。
        /// </summary>
        string UIKey { get; }

        /// <summary>
        /// 获取界面实例。
        /// </summary>
        object Handle { get; }
        
        /// <summary>
        /// 获取界面资源名称。
        /// </summary>
        string UIGroupName { get; }

        /// <summary>
        /// 获取界面所属的界面组。
        /// </summary>
        IUIGroup UIGroup { get; }

        /// <summary>
        /// 获取界面在界面组中的深度。
        /// </summary>
        int DepthInUIGroup { get; }

        /// <summary>
        /// 获取是否暂停被覆盖的界面。
        /// </summary>
        bool PauseCoveredUIForm { get; }
        
        /// <summary>
        /// 获取是否显示遮罩层。
        /// </summary>
        bool DisplayMaskLayer { get; }

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnInit(IUIGroup uiGroup, object userData);

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void OnOpen(object userData);

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isDisposing">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        void OnClose(bool isDisposing, object userData);
        
        /// <summary>
        /// 界面回收。
        /// </summary>
        void OnRecycle();

        /// <summary>
        /// 界面暂停。
        /// </summary>
        void OnPause();

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        void OnResume();

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        void OnCover();

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        void OnReveal();

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        void OnRefocus(object userData);

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void OnUpdate(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        void OnDepthChanged(int uiGroupDepth, int depthInUIGroup);
        
        /// <summary>
        /// 界面遮罩层点击。
        /// </summary>
        void OnMaskLayerClicked();
    }
}