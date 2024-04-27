using CBK.Framework.Request;
using MessagePack;

namespace CBK.Framework.Network.MessagePack
{
    [MessagePackObject(true)]
    public abstract class ResponseMessage : Message, IResponse
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        public uint RpcId { get; set; }
    
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
    
        public override void OnRecycle()
        {
            base.OnRecycle();
            RpcId = 0;
            ErrorCode = 0;
        }
    }
}