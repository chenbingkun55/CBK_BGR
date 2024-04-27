using Cysharp.Threading.Tasks;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    /// <summary>
    /// RPC任务
    /// </summary>
    public struct RpcTask
    {
        /// <summary>
        /// Rpc请求的唯一id
        /// </summary>
        public uint RpcId { get; }
        
        /// <summary>
        /// 可等待的任务
        /// </summary>
        public UniTask<IResponse> Task { get; }

        public RpcTask(uint rpcId, UniTask<IResponse> task)
        {
            RpcId = rpcId;
            Task = task;
        }
    }
}