//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.IO;

namespace CBK.Framework.Network
{
    internal sealed partial class NetworkServiceImpl
    {
        private sealed class ReceiveState : IDisposable
        {
            private const int DefaultBufferLength = 1024 * 64;

            public MemoryStream Stream { get; private set; } = new(DefaultBufferLength);
            public IPacketHeader PacketHeader { get; private set; }

            public void PrepareForPacketHeader(int packetHeaderLength)
            {
                Reset(packetHeaderLength, null);
            }

            public void PrepareForPacket(IPacketHeader packetHeader)
            {
                if (packetHeader == null)
                {
                    throw new Exception("Packet header is invalid.");
                }

                Reset(packetHeader.PacketLength, packetHeader);
            }

            public void Dispose()
            {
                if (Stream == null) 
                    return;
                
                Stream.Dispose();
                Stream = null;
            }

            private void Reset(int targetLength, IPacketHeader packetHeader)
            {
                if (targetLength < 0)
                {
                    throw new Exception("Target length is invalid.");
                }

                Stream.Position = 0L;
                Stream.SetLength(targetLength);
                PacketHeader = packetHeader;
            }
        }
    }
}
