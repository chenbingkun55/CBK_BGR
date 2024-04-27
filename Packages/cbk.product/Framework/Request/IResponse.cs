using CBK.Framework.Reference;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 响应实例接口
    /// </summary>
    public interface IResponse : IReference
    {
        /// <summary>
        /// 错误码
        /// </summary>
        int ErrorCode { get; set; }
    }
}