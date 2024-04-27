//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CBK.Framework.Event;
using CBK.Framework.Json;

namespace CBK.Framework.Network
{
    internal sealed partial class NetworkServiceImpl
    {
        /// <summary>
        /// 网络频道基类。
        /// </summary>
        private abstract class NetworkChannelBase : INetworkChannel, IDisposable
        {
            private const float DefaultHeartBeatInterval = 30f;

            private readonly string m_Name;
            protected readonly Queue<Packet> m_SendPacketPool;
            protected readonly IEventPool m_ReceivePacketPool;
            protected readonly INetworkChannelHelper m_NetworkChannelHelper;
            protected AddressFamily m_AddressFamily;
            protected bool m_ResetHeartBeatElapseSecondsWhenReceivePacket;
            protected float m_HeartBeatInterval;
            protected Socket m_Socket;
            protected readonly SendState m_SendState;
            protected readonly ReceiveState m_ReceiveState;
            protected readonly HeartBeatState m_HeartBeatState;
            protected int m_SentPacketCount;
            protected int m_ReceivedPacketCount;
            protected bool m_Active;

            public Action<NetworkChannelBase, object> NetworkChannelConnected;
            public Action<NetworkChannelBase> NetworkChannelClosed;
            public Action<NetworkChannelBase, int> NetworkChannelMissHeartBeat;
            public Action<NetworkChannelBase, int, SocketError, string> NetworkChannelError;
            public Action<NetworkChannelBase, object> NetworkChannelCustomError;

            /// <summary>
            /// 初始化网络频道基类的新实例。
            /// </summary>
            /// <param name="name">网络频道名称。</param>
            /// <param name="networkChannelHelper">网络频道辅助器。</param>
            public NetworkChannelBase(string name, INetworkChannelHelper networkChannelHelper)
            {
                m_Name = name ?? string.Empty;
                m_SendPacketPool = new Queue<Packet>();
                m_ReceivePacketPool = Event.EventService.That.Allocate();
                m_ReceivePacketPool.EventDispatchTimeLimit = 60;
                m_NetworkChannelHelper = networkChannelHelper;
                m_AddressFamily = AddressFamily.Unknown;
                m_ResetHeartBeatElapseSecondsWhenReceivePacket = false;
                m_HeartBeatInterval = DefaultHeartBeatInterval;
                m_Socket = null;
                m_SendState = new SendState();
                m_ReceiveState = new ReceiveState();
                m_HeartBeatState = new HeartBeatState();
                m_SentPacketCount = 0;
                m_ReceivedPacketCount = 0;
                m_Active = false;

                NetworkChannelConnected = null;
                NetworkChannelClosed = null;
                NetworkChannelMissHeartBeat = null;
                NetworkChannelError = null;
                NetworkChannelCustomError = null;

                m_NetworkChannelHelper.Initialize(this);
            }
            
            /// <summary>
            /// 释放资源。
            /// </summary>
            public void Dispose()
            {
                Close();
                m_SendState.Dispose();
                m_ReceiveState.Dispose();
                m_ReceivePacketPool.Dispose();
                m_NetworkChannelHelper.Dispose();
            }

            /// <summary>
            /// 获取网络频道名称。
            /// </summary>
            public string Name => m_Name;

            /// <summary>
            /// 获取网络频道所使用的 Socket。
            /// </summary>
            public Socket Socket => m_Socket;

            /// <summary>
            /// 获取是否已连接。
            /// </summary>
            public bool Connected => m_Socket != null && m_Socket.Connected;

            /// <summary>
            /// 获取网络服务类型。
            /// </summary>
            public abstract ServiceType ServiceType { get; }

            /// <summary>
            /// 获取网络地址类型。
            /// </summary>
            public AddressFamily AddressFamily => m_AddressFamily;

            /// <summary>
            /// 获取要发送的消息包数量。
            /// </summary>
            public int SendPacketCount => m_SendPacketPool.Count;

            /// <summary>
            /// 获取累计发送的消息包数量。
            /// </summary>
            public int SentPacketCount => m_SentPacketCount;

            /// <summary>
            /// 获取已接收未处理的消息包数量。
            /// </summary>
            public int ReceivePacketCount => m_ReceivePacketPool.EventCount;

            /// <summary>
            /// 获取累计已接收的消息包数量。
            /// </summary>
            public int ReceivedPacketCount => m_ReceivedPacketCount;

            /// <summary>
            /// 获取或设置当收到消息包时是否重置心跳流逝时间。
            /// </summary>
            public bool ResetHeartBeatElapseSecondsWhenReceivePacket
            {
                get => m_ResetHeartBeatElapseSecondsWhenReceivePacket;
                set => m_ResetHeartBeatElapseSecondsWhenReceivePacket = value;
            }

            /// <summary>
            /// 获取丢失心跳的次数。
            /// </summary>
            public int MissHeartBeatCount => m_HeartBeatState.MissHeartBeatCount;

            /// <summary>
            /// 获取或设置心跳间隔时长，以秒为单位。
            /// </summary>
            public float HeartBeatInterval
            {
                get => m_HeartBeatInterval;
                set => m_HeartBeatInterval = value;
            }

            /// <summary>
            /// 获取心跳等待时长，以秒为单位。
            /// </summary>
            public float HeartBeatElapseSeconds => m_HeartBeatState.HeartBeatElapseSeconds;

            /// <summary>
            /// 网络频道轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public virtual void Update(float elapseSeconds, float realElapseSeconds)
            {
                if (m_Socket == null || !m_Active)
                {
                    return;
                }

                ProcessSend();
                ProcessReceive();
                if (m_Socket == null || !m_Active)
                {
                    return;
                }

                m_ReceivePacketPool.OnUpdate(elapseSeconds, realElapseSeconds);

                if (m_HeartBeatInterval > 0f)
                {
                    var sendHeartBeat = false;
                    var missHeartBeatCount = 0;
                    lock (m_HeartBeatState)
                    {
                        if (m_Socket == null || !m_Active)
                        {
                            return;
                        }

                        m_HeartBeatState.HeartBeatElapseSeconds += realElapseSeconds;
                        if (m_HeartBeatState.HeartBeatElapseSeconds >= m_HeartBeatInterval)
                        {
                            sendHeartBeat = true;
                            missHeartBeatCount = m_HeartBeatState.MissHeartBeatCount;
                            m_HeartBeatState.HeartBeatElapseSeconds = 0f;
                            m_HeartBeatState.MissHeartBeatCount++;
                        }
                    }

                    if (sendHeartBeat && m_NetworkChannelHelper.SendHeartBeat())
                    {
                        if (missHeartBeatCount > 0 && NetworkChannelMissHeartBeat != null)
                        {
                            NetworkChannelMissHeartBeat(this, missHeartBeatCount);
                        }
                    }
                }
            }

            /// <summary>
            /// 连接到远程主机。
            /// </summary>
            /// <param name="ipAddress">远程主机的 IP 地址。</param>
            /// <param name="port">远程主机的端口号。</param>
            public void Connect(IPAddress ipAddress, int port)
            {
                Connect(ipAddress, port, null);
            }

            /// <summary>
            /// 连接到远程主机。
            /// </summary>
            /// <param name="ipAddress">远程主机的 IP 地址。</param>
            /// <param name="port">远程主机的端口号。</param>
            /// <param name="userData">用户自定义数据。</param>
            public virtual void Connect(IPAddress ipAddress, int port, object userData)
            {
                if (m_Socket != null)
                {
                    Close();
                    m_Socket = null;
                }

                switch (ipAddress.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        m_AddressFamily = AddressFamily.IPv4;
                        break;

                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        m_AddressFamily = AddressFamily.IPv6;
                        break;

                    default:
                        var errorMessage = $"Not supported address family '{ipAddress.AddressFamily}'.";
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, (int)FrameworkErrorCode.NetworkAddressFamilyError, SocketError.Success, errorMessage);
                            return;
                        }

                        throw new Exception(errorMessage);
                }

                m_SendState.Reset();
                m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);
            }

            /// <summary>
            /// 关闭连接并释放所有相关资源。
            /// </summary>
            public void Close()
            {
                lock (this)
                {
                    if (m_Socket == null)
                    {
                        return;
                    }

                    m_Active = false;

                    try
                    {
                        m_Socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        m_Socket.Close();
                        m_Socket = null;

                        if (NetworkChannelClosed != null)
                        {
                            NetworkChannelClosed(this);
                        }
                    }

                    m_SentPacketCount = 0;
                    m_ReceivedPacketCount = 0;

                    lock (m_SendPacketPool)
                    {
                        m_SendPacketPool.Clear();
                    }

                    m_ReceivePacketPool.Clear();

                    lock (m_HeartBeatState)
                    {
                        m_HeartBeatState.Reset(true);
                    }
                }
            }

            /// <summary>
            /// 向远程主机发送消息包。
            /// </summary>
            /// <typeparam name="T">消息包类型。</typeparam>
            /// <param name="packet">要发送的消息包。</param>
            public void Send<T>(T packet) where T : Packet
            {
                if (m_Socket == null)
                {
                    var errorMessage = "You must connect first.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, (int)FrameworkErrorCode.NetworkSendError, SocketError.Success, errorMessage);
                        return;
                    }

                    throw new Exception(errorMessage);
                }

                if (!m_Active)
                {
                    var errorMessage = "Socket is not active.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, (int)FrameworkErrorCode.NetworkSendError, SocketError.Success, errorMessage);
                        return;
                    }

                    throw new Exception(errorMessage);
                }

                if (packet == null)
                {
                    var errorMessage = "Packet is invalid.";
                    if (NetworkChannelError != null)
                    {
                        NetworkChannelError(this, (int)FrameworkErrorCode.NetworkSendError, SocketError.Success, errorMessage);
                        return;
                    }

                    throw new Exception(errorMessage);
                }

                if (NetworkUtility.EnableMessageLog)
                    Log.Info($"Send {packet.GetType().Name}: {packet.ToJson()}");

                lock (m_SendPacketPool)
                {
                    m_SendPacketPool.Enqueue(packet);
                }
            }

            public void RegisterPacketInterceptor(EventInterceptor interceptor)
            {
                m_ReceivePacketPool.RegisterInterceptor(interceptor);
            }

            public void UnregisterPacketInterceptor(EventInterceptor interceptor)
            {
                m_ReceivePacketPool.UnregisterInterceptor(interceptor);
            }

            protected virtual bool ProcessSend()
            {
                if (m_SendState.Stream.Length > 0 || m_SendPacketPool.Count <= 0)
                {
                    return false;
                }

                while (m_SendPacketPool.Count > 0)
                {
                    Packet packet = null;
                    lock (m_SendPacketPool)
                    {
                        packet = m_SendPacketPool.Dequeue();
                    }

                    var serializeResult = false;
                    try
                    {
                        serializeResult = m_NetworkChannelHelper.Serialize(packet, m_SendState.Stream);
                    }
                    catch (Exception exception)
                    {
                        m_Active = false;
                        if (NetworkChannelError != null)
                        {
                            var socketException = exception as SocketException;
                            NetworkChannelError(this, (int)FrameworkErrorCode.NetworkSerializeError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                            return false;
                        }

                        throw;
                    }

                    if (!serializeResult)
                    {
                        var errorMessage = "Serialized packet failure.";
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, (int)FrameworkErrorCode.NetworkSerializeError, SocketError.Success, errorMessage);
                            return false;
                        }

                        throw new Exception(errorMessage);
                    }
                }

                m_SendState.Stream.Position = 0L;
                return true;
            }

            protected virtual void ProcessReceive()
            {
            }

            protected virtual bool ProcessPacketHeader()
            {
                try
                {
                    object customErrorData = null;
                    var packetHeader = m_NetworkChannelHelper.DeserializePacketHeader(m_ReceiveState.Stream, out customErrorData);

                    if (customErrorData != null && NetworkChannelCustomError != null)
                    {
                        NetworkChannelCustomError(this, customErrorData);
                    }

                    if (packetHeader == null)
                    {
                        var errorMessage = "Packet header is invalid.";
                        if (NetworkChannelError != null)
                        {
                            NetworkChannelError(this, (int)FrameworkErrorCode.NetworkDeserializePacketHeaderError, SocketError.Success, errorMessage);
                            return false;
                        }

                        throw new Exception(errorMessage);
                    }

                    m_ReceiveState.PrepareForPacket(packetHeader);
                    if (packetHeader.PacketLength <= 0)
                    {
                        var processSuccess = ProcessPacket();
                        m_ReceivedPacketCount++;
                        return processSuccess;
                    }
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        var socketException = exception as SocketException;
                        NetworkChannelError(this, (int)FrameworkErrorCode.NetworkDeserializePacketHeaderError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                        return false;
                    }

                    throw;
                }

                return true;
            }

            protected virtual bool ProcessPacket()
            {
                lock (m_HeartBeatState)
                {
                    m_HeartBeatState.Reset(m_ResetHeartBeatElapseSecondsWhenReceivePacket);
                }

                try
                {
                    object customErrorData = null;
                    var packet = m_NetworkChannelHelper.DeserializePacket(m_ReceiveState.PacketHeader, m_ReceiveState.Stream, out customErrorData);

                    if (customErrorData != null && NetworkChannelCustomError != null)
                    {
                        NetworkChannelCustomError(this, customErrorData);
                    }

                    if (packet != null)
                    {
                        m_ReceivePacketPool.Raise(this, packet);
                    }

                    m_ReceiveState.PrepareForPacketHeader(m_NetworkChannelHelper.PacketHeaderLength);
                }
                catch (Exception exception)
                {
                    m_Active = false;
                    if (NetworkChannelError != null)
                    {
                        var socketException = exception as SocketException;
                        NetworkChannelError(this, (int)FrameworkErrorCode.NetworkDeserializePacketError, socketException != null ? socketException.SocketErrorCode : SocketError.Success, exception.ToString());
                        return false;
                    }

                    throw;
                }

                return true;
            }
        }
    }
}