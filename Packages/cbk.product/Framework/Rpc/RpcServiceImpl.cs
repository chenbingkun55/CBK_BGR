using System;
using System.Collections.Generic;
using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    internal sealed partial class RpcServiceImpl : IInitialize, IDisposable, IRpcService
    {
        [Inject]
        public IReferenceService ReferenceService { get; set; }

        private uint m_RpcIdIncrease;
        private readonly Dictionary<uint, IPromise<IResponse>> m_Promises = new Dictionary<uint, IPromise<IResponse>>();

        public void Initialize()
        {
        }

        public void Dispose()
        {
        }
        
        public RpcTask Allocate(CancellationToken token)
        {
            var rpcId = ++m_RpcIdIncrease;
            if (token.IsCancellationRequested)
            {
                // 如果已经取消了，直接返回一个取消的任务
                return new RpcTask(rpcId, UniTask.FromResult((IResponse)ReferenceService.GetReference<CommonResponse>()
                    .SetErrorCode(FrameworkErrorCode.RequestHasBeenCanceled)));
            }

            var promise = AutoResetUniTaskCompletionSource<IResponse>.Create();
            m_Promises.Add(rpcId, promise);
            
            token.Register(OnTokenCancelled, rpcId);
            
            return new RpcTask(rpcId, promise.Task);
        }

        public void SetResponse(uint rpcId, IResponse response)
        {
            if (!m_Promises.TryGetValue(rpcId, out var promise))
            {
                response.Recycle();
                return;
            }
            
            m_Promises.Remove(rpcId);
            promise.TrySetResult(response);
        }

        private void OnTokenCancelled(object obj)
        {
            var rpcId = (uint)obj;
            if (!m_Promises.TryGetValue(rpcId, out var promise))
                return;
            
            m_Promises.Remove(rpcId);
            promise.TrySetResult(ReferenceService.GetReference<CommonResponse>().SetErrorCode(FrameworkErrorCode.RequestHasBeenCanceled));
        }
    }
}