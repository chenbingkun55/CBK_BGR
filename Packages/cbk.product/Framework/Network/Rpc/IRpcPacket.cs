namespace CBK.Framework.Network.Rpc
{
    /// <summary>
    /// Rpc消息包接口
    /// </summary>
    public interface IRpcPacket
    {
        /// <summary>
        /// 消息唯一id
        /// </summary>
        uint RpcId { get; set; }
    }
}