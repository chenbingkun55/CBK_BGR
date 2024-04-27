//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using CBK.Framework.Event;
using CBK.Framework.Reference;

namespace CBK.Framework.Network
{
    /// <summary>
    /// 网络心跳包丢失事件。
    /// </summary>
    public sealed class NetworkMissHeartBeatEventArgs : AEventArgs
    {
        /// <summary>
        /// 获取网络频道。
        /// </summary>
        public INetworkChannel NetworkChannel
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取心跳包已丢失次数。
        /// </summary>
        public int MissCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建网络心跳包丢失事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="missCount">心跳包已丢失次数。</param>
        /// <returns>创建的网络心跳包丢失事件。</returns>
        public static NetworkMissHeartBeatEventArgs Create(INetworkChannel networkChannel, int missCount)
        {
            NetworkMissHeartBeatEventArgs networkMissHeartBeatEventArgs = Reference.ReferenceService.That.GetReference<NetworkMissHeartBeatEventArgs>();
            networkMissHeartBeatEventArgs.NetworkChannel = networkChannel;
            networkMissHeartBeatEventArgs.MissCount = missCount;
            return networkMissHeartBeatEventArgs;
        }

        public override void OnRecycle()
        {
            NetworkChannel = null;
            MissCount = 0;
        }
    }
}
