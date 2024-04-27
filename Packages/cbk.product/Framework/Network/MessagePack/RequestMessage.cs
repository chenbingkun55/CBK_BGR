using CBK.Framework.Network.Rpc;
using MessagePack;

namespace CBK.Framework.Network.MessagePack
{
    [MessagePackObject(true)]
    public abstract class RequestMessage : Message, IRpcPacket
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        public uint RpcId { get; set; }

        public override void OnRecycle()
        {
            base.OnRecycle();
            RpcId = 0;
        }
    }
}