using System.Threading;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    /// <summary>
    /// RPC服务接口
    /// </summary>
    public interface IRpcService
    {
        /// <summary>
        /// 构造一个RPC任务
        /// </summary>
        RpcTask Allocate(CancellationToken token);
        
        /// <summary>
        /// 设置指定RPC请求的响应
        /// </summary>
        void SetResponse(uint rpcId, IResponse response);
    }
}