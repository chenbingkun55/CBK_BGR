using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.Rpc
{
    public static class RpcServiceExtensions
    {
        /// <summary>
        /// 设置指定RPC请求的响应
        /// </summary>
        public static void SetErrorCode(this IRpcService service, uint rpcId, int errorCode)
        {
            var response = ReferenceService.That.GetReference<CommonResponse>();
            response.SetErrorCode(errorCode);
            service.SetResponse(rpcId, response);
        }
    }
}