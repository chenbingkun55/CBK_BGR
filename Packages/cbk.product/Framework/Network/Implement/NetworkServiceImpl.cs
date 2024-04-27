//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Event;
using CBK.Framework.Reference;
using CBK.Framework.Update;

namespace CBK.Framework.Network
{
    /// <summary>
    /// 网络管理器。
    /// </summary>
    internal sealed partial class NetworkServiceImpl : INetworkService, IInitialize, IDisposable, IUpdate
    {
        [Inject]
        public IUpdateService UpdateService { get; set; }

        [Inject]
        public IEventPool EventPool { get; set; }
        
        private readonly Dictionary<string, NetworkChannelBase> m_NetworkChannels = new(StringComparer.Ordinal);

        public void Initialize()
        {
            UpdateService.RegisterUpdate(this);
        }

        /// <summary>
        /// 关闭并清理网络管理器。
        /// </summary>
        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);
            
            foreach (var networkChannel in m_NetworkChannels)
            {
                var networkChannelBase = networkChannel.Value;
                networkChannelBase.NetworkChannelConnected -= OnNetworkChannelConnected;
                networkChannelBase.NetworkChannelClosed -= OnNetworkChannelClosed;
                networkChannelBase.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
                networkChannelBase.NetworkChannelError -= OnNetworkChannelError;
                networkChannelBase.NetworkChannelCustomError -= OnNetworkChannelCustomError;
                networkChannelBase.Dispose();
            }

            m_NetworkChannels.Clear();
            EventPool.Dispose();
        }

        /// <summary>
        /// 获取网络频道数量。
        /// </summary>
        public int NetworkChannelCount => m_NetworkChannels.Count;

        /// <summary>
        /// 网络管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            EventPool.OnUpdate(elapseSeconds, realElapseSeconds);
            
            foreach (var networkChannel in m_NetworkChannels)
            {
                networkChannel.Value.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 检查是否存在网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否存在网络频道。</returns>
        public bool HasNetworkChannel(string name)
        {
            return !string.IsNullOrEmpty(name) && m_NetworkChannels.ContainsKey(name);
        }

        /// <summary>
        /// 获取网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>要获取的网络频道。</returns>
        public INetworkChannel GetNetworkChannel(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return m_NetworkChannels.TryGetValue(name, out var networkChannel) ? networkChannel : null;
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <returns>所有网络频道。</returns>
        public INetworkChannel[] GetAllNetworkChannels()
        {
            var index = 0;
            var results = new INetworkChannel[m_NetworkChannels.Count];
            foreach (var networkChannel in m_NetworkChannels)
            {
                results[index++] = networkChannel.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有网络频道。
        /// </summary>
        /// <param name="results">所有网络频道。</param>
        public void GetAllNetworkChannels(List<INetworkChannel> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (var networkChannel in m_NetworkChannels)
            {
                results.Add(networkChannel.Value);
            }
        }

        /// <summary>
        /// 创建网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <param name="serviceType">网络服务类型。</param>
        /// <param name="networkChannelHelper">网络频道辅助器。</param>
        /// <returns>要创建的网络频道。</returns>
        public INetworkChannel CreateNetworkChannel(string name, ServiceType serviceType, INetworkChannelHelper networkChannelHelper)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Network channel name is invalid.");
            }

            if (networkChannelHelper == null)
            {
                throw new Exception("Network channel helper is invalid.");
            }

            if (networkChannelHelper.PacketHeaderLength < 0)
            {
                throw new Exception("Packet header length is invalid.");
            }

            if (HasNetworkChannel(name))
            {
                throw new Exception($"Already exist network channel '{name}'.");
            }

            NetworkChannelBase networkChannel;
            switch (serviceType)
            {
                case ServiceType.Tcp:
                    networkChannel = new TcpNetworkChannel(name, networkChannelHelper);
                    break;

                case ServiceType.TcpWithSyncReceive:
                    networkChannel = new TcpWithSyncReceiveNetworkChannel(name, networkChannelHelper);
                    break;

                default:
                    throw new Exception($"Not supported service type '{serviceType}'.");
            }

            networkChannel.NetworkChannelConnected += OnNetworkChannelConnected;
            networkChannel.NetworkChannelClosed += OnNetworkChannelClosed;
            networkChannel.NetworkChannelMissHeartBeat += OnNetworkChannelMissHeartBeat;
            networkChannel.NetworkChannelError += OnNetworkChannelError;
            networkChannel.NetworkChannelCustomError += OnNetworkChannelCustomError;
            m_NetworkChannels.Add(name, networkChannel);
            return networkChannel;
        }

        /// <summary>
        /// 销毁网络频道。
        /// </summary>
        /// <param name="name">网络频道名称。</param>
        /// <returns>是否销毁网络频道成功。</returns>
        public bool DestroyNetworkChannel(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!m_NetworkChannels.TryGetValue(name, out var networkChannel))
                return false;

            networkChannel.NetworkChannelConnected -= OnNetworkChannelConnected;
            networkChannel.NetworkChannelClosed -= OnNetworkChannelClosed;
            networkChannel.NetworkChannelMissHeartBeat -= OnNetworkChannelMissHeartBeat;
            networkChannel.NetworkChannelError -= OnNetworkChannelError;
            networkChannel.NetworkChannelCustomError -= OnNetworkChannelCustomError;
            networkChannel.Dispose();
            return m_NetworkChannels.Remove(name);
        }

        private void OnNetworkChannelConnected(NetworkChannelBase networkChannel, object userData)
        {
            EventPool.Raise(this, NetworkConnectedEventArgs.Create(networkChannel, userData));
        }

        private void OnNetworkChannelClosed(NetworkChannelBase networkChannel)
        {
            EventPool.Raise(this, NetworkClosedEventArgs.Create(networkChannel));
        }

        private void OnNetworkChannelMissHeartBeat(NetworkChannelBase networkChannel, int missHeartBeatCount)
        {
            EventPool.Raise(this, NetworkMissHeartBeatEventArgs.Create(networkChannel, missHeartBeatCount));
        }

        private void OnNetworkChannelError(NetworkChannelBase networkChannel, int errorCode, SocketError socketErrorCode, string errorMessage)
        {
            EventPool.Raise(this, NetworkErrorEventArgs.Create(networkChannel, errorCode, socketErrorCode, errorMessage));
        }

        private void OnNetworkChannelCustomError(NetworkChannelBase networkChannel, object customErrorData)
        {
            EventPool.Raise(this, NetworkCustomErrorEventArgs.Create(networkChannel, customErrorData));
        }
    }
}