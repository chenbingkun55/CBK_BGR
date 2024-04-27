using CBK.Framework.Reference;

namespace CBK.Framework.Request
{
    /// <summary>
    /// 响应实例抽象类
    /// </summary>
    public abstract class AResponse : AReference, IResponse
    {
        public int ErrorCode { get; set; }

        public override void OnRecycle()
        {
            ErrorCode = 0;
        }
    }
    
    /// <summary>
    /// 通用响应实例
    /// </summary>
    public sealed class CommonResponse : AResponse
    {
    }
}