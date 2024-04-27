using System;
using System.IO;
using CBK.Framework.Json;
using CBK.Framework.Reference;
using CBK.Framework.Rpc;
using MessagePack;

namespace CBK.Framework.Network.MessagePack
{
    /// <summary>
    /// 基于MessagePack的网络通道辅助器
    /// </summary>
    public sealed class MessagePackNetworkChannelHelper : INetworkChannelHelper
    {
        private const int MagicNumber = 0x1234;
        private const int MagicNumberAfterLeftShift = MagicNumber << 8;
        private const int SendHeaderLength = 4 + 8 + 4 + 4;
        private const int ReceiveHeaderLength = 4 + 4;

        public int PacketHeaderLength => ReceiveHeaderLength;

        private int m_Count;
        private readonly byte[] m_Buffer = new byte[8];

        private readonly IMessageFactory m_MessageFactory;
        private readonly IReferenceService m_ReferenceService;
        private readonly IRpcService m_RpcService;
        private INetworkChannel m_NetworkChannel;

        public MessagePackNetworkChannelHelper(IMessageFactory messageFactory, IReferenceService referenceService, IRpcService mRpcService)
        {
            m_MessageFactory = messageFactory;
            m_ReferenceService = referenceService;
            m_RpcService = mRpcService;
        }

        public void Dispose()
        {
            m_NetworkChannel?.UnregisterPacketInterceptor(PacketInterceptor);
        }

        public void Initialize(INetworkChannel networkChannel)
        {
            m_NetworkChannel = networkChannel;
            m_NetworkChannel?.RegisterPacketInterceptor(PacketInterceptor);
        }

        public void PrepareForConnecting()
        {
        }

        public bool SendHeartBeat()
        {
            return true; // TODO
        }

        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            if (packet is not Message message)
                return false;

            #region [Header 偏移]

            var beginPosition = destination.Position;
            var requiredLength = beginPosition + SendHeaderLength;

            if (destination.Length < requiredLength)
                destination.SetLength(requiredLength);

            destination.Position = requiredLength;

            #endregion

            MessagePackSerializer.Serialize(packet.GetType(), destination, packet);

            var endPosition = destination.Position;
            var bufferLength = endPosition - requiredLength;
            var totalLength = (int)bufferLength + SendHeaderLength;

            destination.Position = beginPosition;

            var magic = MagicNumber + ++m_Count;
            magic ^= MagicNumberAfterLeftShift;
            magic ^= totalLength;

            WriteInt32(destination, totalLength); // 4
            WriteInt64(destination, DateTime.Now.Ticks); // 8
            WriteInt32(destination, magic); // 4
            WriteInt32(destination, message.Route); // 4

            destination.Position = endPosition;

            // 回收实例
            message.Recycle();
            return true;
        }

        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            if (!TryReadInt32(source, out var totalLength))
            {
                customErrorData = "Read total length failure.";
                return null;
            }

            if (!TryReadInt32(source, out var route))
            {
                customErrorData = "Read route failure.";
                return null;
            }

            customErrorData = null;

            var header = m_ReferenceService.GetReference<MessagePackPacketHeader>();
            header.PacketLength = totalLength - ReceiveHeaderLength;
            header.Route = route;

            return header;
        }

        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            var header = (MessagePackPacketHeader)packetHeader;
            var route = header.Route;
            header.Recycle();

            var messageType = m_MessageFactory.FindType(route);
            if (messageType == null)
            {
                customErrorData = $"Can not find message type. Route: {route}";
                return null;
            }

            var message = MessagePackSerializer.Deserialize<Message>(source);
            if (message == null)
            {
                customErrorData = $"Deserialize message failure. Route: {route}";
                return null;
            }

            if (message.Route != route)
            {
                customErrorData = $"Route is not match. {message.Route} != {route}";
                message.Recycle();
                return null;
            }

            customErrorData = null;
            return message;
        }

        private void WriteInt32(Stream destination, int value)
        {
            m_Buffer[0] = (byte)(value >> 24);
            m_Buffer[1] = (byte)(value >> 16);
            m_Buffer[2] = (byte)(value >> 8);
            m_Buffer[3] = (byte)value;
            destination.Write(m_Buffer, 0, 4);
        }

        private void WriteInt64(Stream destination, long value)
        {
            m_Buffer[0] = (byte)(value >> 56);
            m_Buffer[1] = (byte)(value >> 48);
            m_Buffer[2] = (byte)(value >> 40);
            m_Buffer[3] = (byte)(value >> 32);
            m_Buffer[4] = (byte)(value >> 24);
            m_Buffer[5] = (byte)(value >> 16);
            m_Buffer[6] = (byte)(value >> 8);
            m_Buffer[7] = (byte)value;
            destination.Write(m_Buffer, 0, 8);
        }

        private bool TryReadInt32(Stream destination, out int value)
        {
            var num = destination.Read(m_Buffer, 0, 4);
            if (num != 4)
            {
                value = 0;
                return false;
            }

            value = m_Buffer[0] << 24 | m_Buffer[1] << 16 | m_Buffer[2] << 8 | m_Buffer[3];
            return true;
        }

        private bool TryReadInt64(Stream destination, out long value)
        {
            var num = destination.Read(m_Buffer, 0, 8);
            if (num != 8)
            {
                value = 0;
                return false;
            }

            value = (long)m_Buffer[0] << 56 | (long)m_Buffer[1] << 48 | (long)m_Buffer[2] << 40 |
                    (long)m_Buffer[3] << 32 | (long)m_Buffer[4] << 24 | (long)m_Buffer[5] << 16 |
                    (long)m_Buffer[6] << 8 | (long)m_Buffer[7];
            return true;
        }

        private bool PacketInterceptor(string eventname, object sender, EventArgs e)
        {
            if (NetworkUtility.EnableMessageLog)
                Log.Info($"Receive {e.GetType().Name}: {e.ToJson()}");
            
            if (e is not ResponseMessage responseMessage)
                return false;

            if (responseMessage.RpcId == 0)
                return false;

            m_RpcService.SetResponse(responseMessage.RpcId, responseMessage);
            return true;
        }

        private sealed class MessagePackPacketHeader : AReference, IPacketHeader
        {
            public int PacketLength { get; set; }
            public int Route { get; set; }

            public override void OnRecycle()
            {
                PacketLength = 0;
                Route = 0;
            }
        }
    }
}