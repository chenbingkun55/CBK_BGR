using Cysharp.Threading.Tasks;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 响应UI请求类界面基类
    /// </summary>
    public abstract class AFairyGUIRequestFormLogic<T> : AFairyGUIFormLogic where T : class, IUIRequest
    {
        /// <summary>
        /// UI请求队列
        /// </summary>
        protected IUIRequestQueue UIRequestQueue { get; private set; }

        /// <summary>
        /// UI请求队列模式
        /// </summary>
        protected abstract UIRequestQueueMode UIRequestQueueMode { get; }

        /// <summary>
        /// 界面被关闭时返回的错误码
        /// </summary>
        protected virtual int ErrorCodeWhileUIFormClosed => (int)FrameworkErrorCode.UIRequestFormClosed;

        /// <summary>
        /// 界面被覆盖时返回的错误码
        /// </summary>
        protected virtual int ErrorCodeWhileOverride => (int)FrameworkErrorCode.UIRequestOverride;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            UIRequestQueue = new UIRequestQueue<T>(UIRequestQueueMode,
                ExecuteUIRequest,
                ErrorCodeWhileUIFormClosed,
                ErrorCodeWhileOverride);

            UIRequestQueue.OnQueueEmpty += OnUIRequestQueueEmpty;
        }

        protected sealed override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            OnOpen();
            UIRequestQueue.OnUIFormOpened();
            HandleUserData(userData);
        }

        protected sealed override void OnClose(bool isShutdown, object userData)
        {
            UIRequestQueue.OnUIFormClosed();
            OnClose();
            base.OnClose(isShutdown, userData);
        }

        protected sealed override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            OnRefocus();
            HandleUserData(userData);
        }

        /// <summary>
        /// 处理用户数据
        /// </summary>
        private void HandleUserData(object userData)
        {
            var request = userData as IUIRequest;
            if (request == null)
            {
                request = ConvertToRequest(userData);

                if (request == null)
                    Log.Error("AFairyGUIRequestFormLogic HandleUserData: userData is not an UIRequest.");
            }

            UIRequestQueue.Enqueue(request);
        }

        /// <summary>
        /// UI请求队列为空时关闭界面
        /// </summary>
        private void OnUIRequestQueueEmpty()
        {
            CloseForm();
        }

        /// <summary>
        /// 执行UI请求
        /// </summary>
        protected abstract UniTask ExecuteUIRequest(T request);

        /// <summary>
        /// 将用户数据转换为UI请求
        /// </summary>
        protected virtual IUIRequest ConvertToRequest(object userData)
        {
            return null;
        }

        protected virtual void OnOpen()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnRefocus()
        {
        }
    }
}