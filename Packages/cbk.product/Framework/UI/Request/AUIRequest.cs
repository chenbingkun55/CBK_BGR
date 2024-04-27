using System;
using Cysharp.Threading.Tasks;
using CBK.Framework.Reference;
using CBK.Framework.Request;
using CancellationToken = System.Threading.CancellationToken;

namespace CBK.Framework.UI
{
    /// <summary>
    /// UI请求抽象类
    /// </summary>
    public abstract class AUIRequest : ARequest, IUIRequest
    {
        public event Action<IUIRequest> OnResponseUpdated;

        /// <summary>
        /// UI键值
        /// </summary>
        protected abstract string UIKey { get; }
        
        /// <summary>
        /// 取消令牌
        /// </summary>
        public CancellationToken CancellationToken { get; private set; }

        /// <summary>
        /// 应答包实例
        /// </summary>
        private IResponse m_Response;
        
        protected sealed override async UniTask<IResponse> OnExecute(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
            var referenceService = ReferenceService;
            
            var code = await UIService.That.OpenUIForm(UIKey, this, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return referenceService.GetReference<CommonResponse>().SetErrorCode(FrameworkErrorCode.RequestHasBeenCanceled);

            if (code != 0)
                return referenceService.GetReference<CommonResponse>().SetErrorCode(code);

            await UniTask.WaitUntil(HasResponse, cancellationToken: cancellationToken).SuppressCancellationThrow();
            
            return m_Response;
        }

        public void SetResponse(IResponse response)
        {
            if (m_Response != null)
            {
                // 重复的响应包
                Log.Warning($"重复的响应包: {UIKey}");
                response.Recycle();
                return;
            }
            
            OnResponseUpdated?.Invoke(this);
            m_Response = response;
        }

        private bool HasResponse()
        {
            return m_Response != null;
        }

        public sealed override void OnRecycle()
        {
            OnRecycleExtend();
            CancellationToken = new CancellationToken(true);
            m_Response = null;
            OnResponseUpdated = null;
        }

        protected abstract void OnRecycleExtend();
    }
}