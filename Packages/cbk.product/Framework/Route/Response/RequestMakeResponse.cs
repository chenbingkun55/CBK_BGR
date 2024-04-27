using CBK.Framework.Reference;
using CBK.Framework.Request;

namespace CBK.Framework.Route
{
    /// <summary>
    /// 路由请求构造响应实例
    /// </summary>
    public sealed class RequestMakeResponse : AResponse
    {
        /// <summary>
        /// 构造的请求实例
        /// </summary>
        public IRequest Request { get; set; }
        
        public override void OnRecycle()
        {
            base.OnRecycle();
            Request?.Recycle();
            Request = null;
        }
    }
}