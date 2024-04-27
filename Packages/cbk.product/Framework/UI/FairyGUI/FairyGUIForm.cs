using FairyGUI;

namespace CBK.Framework.UI.FairyGUI
{
    internal sealed class FairyGUIForm : IUIForm
    {
        public string UIKey { get; }
        public object Handle => ContentPane;
        public GComponent ContentPane { get; }
        public string UIGroupName => m_UIFormLogic.GroupName;
        public bool PauseCoveredUIForm => m_UIFormLogic.PauseCoveredUIForm;
        public bool DisplayMaskLayer => m_UIFormLogic.DisplayMaskLayer;
        public IUIGroup UIGroup { get; private set; }
        public int DepthInUIGroup { get; private set; }

        /// <summary>
        /// 获取界面是否可用。
        /// </summary>
        public bool Available { get; private set; }

        /// <summary>
        /// 获取或设置界面是否可见。
        /// </summary>
        public bool Visible
        {
            get => Available && m_Visible;
            set
            {
                if (!Available)
                {
                    Log.Warning($"UI form '{UIKey}' is not available.");
                    return;
                }

                if (m_Visible == value)
                {
                    return;
                }

                m_Visible = value;
                InternalSetVisible(value);
            }
        }

        private readonly AFairyGUIFormLogic m_UIFormLogic;
        private bool m_Visible = false;

        public FairyGUIForm(string uiKey, GComponent contentPane, AFairyGUIFormLogic uiFormLogic)
        {
            UIKey = uiKey;
            ContentPane = contentPane;
            m_UIFormLogic = uiFormLogic;

            ContentPane.fairyBatching = true;
        }

        public void OnInit(IUIGroup uiGroup, object userData)
        {
            UIGroup = uiGroup;
            DepthInUIGroup = 0;

            try
            {
                m_UIFormLogic.OnFormInit(this, userData);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnInit with exception '{exception}'.");
            }
        }

        public void OnRecycle()
        {
            try
            {
                m_UIFormLogic.OnFormRecycle();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnRecycle with exception '{exception}'.");
            }

            DepthInUIGroup = 0;
        }

        public void OnOpen(object userData)
        {
            Available = true;
            Visible = true;
            
            var uiGroupHelper = (FairyGUIGroupHelper)UIGroup.Helper;
            uiGroupHelper.GroupRoot.AddChild(ContentPane);
            
            try
            {
                m_UIFormLogic.OnFormOpen(userData);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnOpen with exception '{exception}'.");
            }
        }

        public void OnClose(bool isDisposing, object userData)
        {
            Visible = false;
            Available = false;
            
            try
            {
                m_UIFormLogic.OnFormClose(isDisposing, userData);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnClose with exception '{exception}'.");
            }
            
            if (Available)
                return;
            
            ContentPane.RemoveFromParent();
        }

        public void OnPause()
        {
            Visible = false;

            try
            {
                m_UIFormLogic.OnFormPause();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnPause with exception '{exception}'.");
            }
        }

        public void OnResume()
        {
            Visible = true;
            
            try
            {
                m_UIFormLogic.OnFormResume();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnResume with exception '{exception}'.");
            }
        }

        public void OnCover()
        {
            try
            {
                m_UIFormLogic.OnFormCover();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnCover with exception '{exception}'.");
            }
        }

        public void OnReveal()
        {
            try
            {
                m_UIFormLogic.OnFormReveal();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnReveal with exception '{exception}'.");
            }
        }

        public void OnRefocus(object userData)
        {
            try
            {
                m_UIFormLogic.OnFormRefocus(userData);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnRefocus with exception '{exception}'.");
            }
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            try
            {
                m_UIFormLogic.OnFormUpdate(elapseSeconds, realElapseSeconds);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnUpdate with exception '{exception}'.");
            }
        }

        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            DepthInUIGroup = depthInUIGroup;
            ContentPane.sortingOrder = (depthInUIGroup << 1) + 1;

            try
            {
                m_UIFormLogic.OnFormDepthChanged(uiGroupDepth, depthInUIGroup);
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnDepthChanged with exception '{exception}'.");
            }
        }

        public void OnMaskLayerClicked()
        {
            try
            {
                m_UIFormLogic.OnFormMaskLayerClicked();
            }
            catch (System.Exception exception)
            {
                Log.Error($"UI form '[{UIKey}]' OnMaskLayerClicked with exception '{exception}'.");
            }
        }

        /// <summary>
        /// 设置界面的可见性。
        /// </summary>
        /// <param name="visible">界面的可见性。</param>
        private void InternalSetVisible(bool visible)
        {
            ContentPane.visible = visible;
        }
    }
}