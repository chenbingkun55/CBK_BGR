using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using CBK.Framework.Update;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面请求队列
    /// </summary>
    public sealed class UIRequestQueue<T> : IUIRequestQueue where T: class, IUIRequest
    {
        public delegate UniTask ExecuteUIRequestHandler(T request);

        public event Action OnQueueEmpty;

        public bool IsEmpty => m_Requests.Count == 0;

        public IUIRequest CurrentRequest => m_ExecutingRequests.Count > 0 ? m_ExecutingRequests[0] : null;

        private readonly Action m_QueueUpdateMethod;
        private readonly ExecuteUIRequestHandler m_Handler;
        private readonly int m_ErrorCodeWhileUIFormClosed;
        private readonly int m_ErrorCodeWhileOverride;
        private readonly IUpdateService m_UpdateService;

        private readonly List<T> m_Requests = new();
        private readonly List<T> m_ExecutingRequests = new();
        private readonly List<T> m_Buffer = new();

        private bool m_FormOpened = false;

        public UIRequestQueue(UIRequestQueueMode mode, ExecuteUIRequestHandler handler, 
            int errorCodeWhileUIFormClosed = (int) FrameworkErrorCode.UIRequestFormClosed, 
            int errorCodeWhileOverride = (int) FrameworkErrorCode.UIRequestOverride)
        {
            m_QueueUpdateMethod = mode switch
            {
                UIRequestQueueMode.Sequence => QueueUpdateOnSequenceMode,
                UIRequestQueueMode.Reverse => QueueUpdateOnReverseMode,
                UIRequestQueueMode.Override => QueueUpdateOnOverrideMode,
                UIRequestQueueMode.Parallel => QueueUpdateOnParallelMode,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
            m_Handler = handler;
            m_ErrorCodeWhileUIFormClosed = errorCodeWhileUIFormClosed;
            m_ErrorCodeWhileOverride = errorCodeWhileOverride;
            m_UpdateService = UpdateService.That;
        }

        public void Enqueue(IUIRequest request)
        {
            if (request == null)
            {
                m_UpdateService.RegisterDelayCall(UpdateQueue);
                return;
            }
            
            if (request.CancellationToken.IsCancellationRequested)
            {
                m_UpdateService.RegisterDelayCall(UpdateQueue);
                return;
            }
            
            request.OnResponseUpdated += OnRequestResponseUpdated;
            
            if (request is T dstRequest)
            {
                m_Requests.Add(dstRequest);
                dstRequest.CancellationToken.Register(OnTokenCancelled, dstRequest);
                UpdateQueue();
            }
            else
            {
                request.SetResponse(FrameworkErrorCode.UIRequestTypeInvalid);
            }
        }

        private void OnTokenCancelled(object obj)
        {
            var request = (T)obj;
            
            if (!m_Requests.Contains(request))
                return;
            
            request.SetResponse(FrameworkErrorCode.RequestHasBeenCanceled);
        }

        private void OnRequestResponseUpdated(IUIRequest request)
        {
            request.OnResponseUpdated -= OnRequestResponseUpdated;

            if (request is T dstRequest)
            {
                m_Requests.Remove(dstRequest);
                m_ExecutingRequests.Remove(dstRequest);
            }
            
            m_UpdateService.RegisterDelayCall(UpdateQueue);
        }

        private void UpdateQueue()
        {
            if (!m_FormOpened)
                return;
            
            if (m_Requests.Count == 0)
            {
                OnQueueEmpty?.Invoke();
                return;
            }
            
            m_QueueUpdateMethod.Invoke();
        }

        private void QueueUpdateOnParallelMode()
        {
            if (m_ExecutingRequests.Count == m_Requests.Count)
                return;
            
            m_Buffer.Clear();
            m_Buffer.AddRange(m_Requests);
            
            foreach (var request in m_Buffer)
            {
                if (m_ExecutingRequests.Contains(request))
                    continue;
                
                m_ExecutingRequests.Add(request);
            
                m_Handler.Invoke(request).Forget();
            }
        }

        private void QueueUpdateOnOverrideMode()
        {
            if (m_Requests.Count > 1)
            {
                m_Buffer.Clear();
                
                for (var i = 0; i < m_Requests.Count - 1; i++)
                    m_Buffer.Add(m_Requests[i]);
            
                foreach (var request in m_Buffer)
                    request.SetResponse(m_ErrorCodeWhileOverride);
            }

            {
                var request = m_Requests[^1];
                if (m_ExecutingRequests.Contains(request))
                    return;
                
                m_ExecutingRequests.Add(request);
            
                m_Handler.Invoke(request).Forget();
            }
        }

        private void QueueUpdateOnSequenceMode()
        {
            if (m_ExecutingRequests.Count > 0)
                return;

            var request = m_Requests[0];
            m_ExecutingRequests.Add(request);
            
            m_Handler.Invoke(request).Forget();
        }
        
        private void QueueUpdateOnReverseMode()
        {
            var request = m_Requests[^1];
            if (m_ExecutingRequests.Contains(request))
                return;
                
            m_ExecutingRequests.Clear();
            m_ExecutingRequests.Add(request);
            
            m_Handler.Invoke(request).Forget();
        }

        public void OnUIFormOpened()
        {
            m_FormOpened = true;
        }

        public void OnUIFormClosed()
        {
            m_FormOpened = false;
            
            if (m_Requests.Count == 0)
                return;
            
            m_Buffer.Clear();
            m_Buffer.AddRange(m_Requests);
            
            foreach (var request in m_Buffer)
                request.SetResponse(m_ErrorCodeWhileUIFormClosed);
        }
    }
}