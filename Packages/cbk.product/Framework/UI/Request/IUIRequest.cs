using System;
using CBK.Framework.Request;
using CancellationToken = System.Threading.CancellationToken;

namespace CBK.Framework.UI
{
    /// <summary>
    /// UI请求接口
    /// </summary>
    public interface IUIRequest : IRequest
    {
        /// <summary>
        /// 应答包更新事件
        /// </summary>
        event Action<IUIRequest> OnResponseUpdated;
        
        /// <summary>
        /// 请求令牌
        /// </summary>
        CancellationToken CancellationToken { get; }
        
        /// <summary>
        /// 设置应答包实例
        /// </summary>
        void SetResponse(IResponse response);
    }
}