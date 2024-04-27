using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    /// <summary>
    /// RPC等待器接口
    /// </summary>
    public interface IRpcWaiter
    {
        /// <summary>
        /// RpcId
        /// </summary>
        uint RpcId { get; }
        
        /// <summary>
        /// 异步等待响应
        /// </summary>
        UniTask<IResponse> GetResponseAsync(CancellationToken token);
    }
}