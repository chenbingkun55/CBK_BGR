using System.Threading;

namespace CBK.Framework.UI.FairyGUI
{
    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 在Close时会被取消的令牌
        /// </summary>
        protected CancellationToken CancellationTokenOnClose => (m_CancellationTokenSourceOnClose ??= new CancellationTokenSource()).Token;
        
        /// <summary>
        /// 在Pause时会被取消的令牌
        /// </summary>
        protected CancellationToken CancellationTokenOnPause => (m_CancellationTokenSourceOnPause ??= new CancellationTokenSource()).Token;
        
        /// <summary>
        /// 在Recycle时会被取消的令牌
        /// </summary>
        protected CancellationToken CancellationTokenOnRecycle => (m_CancellationTokenSourceOnRecycle ??= new CancellationTokenSource()).Token;
        
        private CancellationTokenSource m_CancellationTokenSourceOnClose;
        private CancellationTokenSource m_CancellationTokenSourceOnPause;
        private CancellationTokenSource m_CancellationTokenSourceOnRecycle;
        
        private void _OnRecycleCancellationToken()
        {
            if (m_CancellationTokenSourceOnRecycle != null)
            {
                m_CancellationTokenSourceOnRecycle.Cancel();
                m_CancellationTokenSourceOnRecycle.Dispose();
                m_CancellationTokenSourceOnRecycle = null;
            }
        }

        private void _OnCloseCancelToken()
        {
            if (m_CancellationTokenSourceOnClose != null)
            {
                m_CancellationTokenSourceOnClose.Cancel();
                m_CancellationTokenSourceOnClose.Dispose();
                m_CancellationTokenSourceOnClose = null;
            }
        }

        private void _OnPauseCancellationToken()
        {
            if (m_CancellationTokenSourceOnPause != null)
            {
                m_CancellationTokenSourceOnPause.Cancel();
                m_CancellationTokenSourceOnPause.Dispose();
                m_CancellationTokenSourceOnPause = null;
            }
        }
    }
}