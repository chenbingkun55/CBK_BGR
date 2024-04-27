using System.Threading;
using Cysharp.Threading.Tasks;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    /// <summary>
    /// RPC请求抽象类
    /// </summary>
    public abstract class ARpcRequest : ARequest
    {
        protected override UniTask<IResponse> OnExecute(CancellationToken cancellationToken)
        {
            var rpcTask = RpcService.That.Allocate(cancellationToken);
            OnExecuteRpc(rpcTask.RpcId, cancellationToken);
            return rpcTask.Task;
        }

        protected abstract void OnExecuteRpc(uint rpcId, CancellationToken cancellationToken);
    }
}