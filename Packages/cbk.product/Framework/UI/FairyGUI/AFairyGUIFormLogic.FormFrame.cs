using FairyGUI;

namespace CBK.Framework.UI.FairyGUI
{
    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 界面的框架组件 约定节点名为frame的组件为框架组件
        /// </summary>
        protected GComponent Frame { get; private set; }

        /// <summary>
        /// 界面的关闭按钮 当它被点击时会调用OnCloseButtonClick方法 约定框架组件下名为closeButton的组件为默认的关闭按钮
        /// </summary>
        protected GObject CloseButton
        {
            get => m_CloseButton;
            set
            {
                if (m_CloseButton == value)
                    return;
                
                if (m_CloseButton != null)
                    m_CloseButton.onClick.Remove(OnCloseButtonClick);

                m_CloseButton = value;
                
                if (m_CloseButton != null)
                    m_CloseButton.onClick.Add(OnCloseButtonClick);
            }
        }

        protected GObject BackButton
        {
            get => m_BackButton;
            set
            {
                if (m_BackButton == value)
                    return;
                
                if (m_BackButton != null)
                    m_BackButton.onClick.Remove(OnBackButtonClick);

                m_BackButton = value;
                
                if (m_BackButton != null)
                    m_BackButton.onClick.Add(OnBackButtonClick);
            }
        }

        private GObject m_CloseButton;
        private GObject m_BackButton;

        private void _OnInitFormFrame()
        {
            Frame = ContentPane.GetChild("frame") as GComponent;
            if (Frame != null)
            {
                CloseButton = Frame.GetChild("closeButton");
                BackButton = Frame.GetChild("backButton");
            }
        }

        private void _OnRecycleFormFrame()
        {
            CloseButton = null;
            Frame = null;
        }

        /// <summary>
        /// 关闭按钮被点击时调用 默认实现为关闭界面
        /// </summary>
        protected virtual void OnCloseButtonClick()
        {
            CloseForm();
        }

        /// <summary>
        /// 返回按钮被点击时调用 默认实现为触发关闭按钮被点击的事件
        /// </summary>
        protected virtual void OnBackButtonClick()
        {
            OnCloseButtonClick();
        }
    }
}