using CBK.Framework.Utility;

namespace CBK.Framework.UI.FairyGUI
{
    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 在Close时会调用Dispose的DisposableGroup
        /// </summary>
        protected DisposableGroup DisposableGroupOnClose => m_DisposableGroupOnClose ??= new DisposableGroup();
        
        /// <summary>
        /// 在Pause时会调用Dispose的DisposableGroup
        /// </summary>
        protected DisposableGroup DisposableGroupOnPause => m_DisposableGroupOnPause ??= new DisposableGroup();
        
        /// <summary>
        /// 在Recycle时会调用Dispose的DisposableGroup
        /// </summary>
        protected DisposableGroup DisposableGroupOnRecycle => m_DisposableGroupOnRecycle ??= new DisposableGroup();
        
        private DisposableGroup m_DisposableGroupOnClose;
        private DisposableGroup m_DisposableGroupOnPause;
        private DisposableGroup m_DisposableGroupOnRecycle;
        
        private void _OnRecycleDisposableGroup()
        {
            if (m_DisposableGroupOnRecycle != null)
            {
                m_DisposableGroupOnRecycle.Dispose();
            }
        }

        private void _OnCloseDisposableGroup()
        {
            if (m_DisposableGroupOnClose != null)
            {
                m_DisposableGroupOnClose.Dispose();
            }
        }

        private void _OnPauseDisposableGroup()
        {
            if (m_DisposableGroupOnPause != null)
            {
                m_DisposableGroupOnPause.Dispose();
            }
        }
    }
}