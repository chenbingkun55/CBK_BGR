using CBK.Framework.Request;

namespace CBK.Framework.Http
{
    /// <summary>
    /// Http应答包
    /// </summary>
    public sealed class HttpResponse : AResponse
    {
        /// <summary>
        /// 响应内容
        /// </summary>
        public string Content { get; set; }

        public override void OnRecycle()
        {
            base.OnRecycle();
            Content = string.Empty;
        }
    }
}