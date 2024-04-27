using CatJson;
using CBK.Framework.Reference;
using MessagePack;

namespace CBK.Framework.Network.MessagePack
{
    /// <summary>
    /// 消息接口
    /// </summary>
    [MessagePackObject(true)]
    public abstract class Message : Packet
    {
        /// <summary>
        /// 路由 消息类型id
        /// </summary>
        [IgnoreMember]
        [JsonIgnore]
        public abstract int Route { get; }
        
        [IgnoreMember]
        [JsonIgnore]
        public override int SerialId { get; set; }

        [IgnoreMember]
        [JsonIgnore]
        public override IReferenceService ReferenceService { get; set; }

        public override void OnRecycle()
        {
        }
    }
}