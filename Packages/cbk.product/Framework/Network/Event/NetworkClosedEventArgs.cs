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
    /// 网络连接关闭事件。
    /// </summary>
    public sealed class NetworkClosedEventArgs : AEventArgs
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
        /// 创建网络连接关闭事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <returns>创建的网络连接关闭事件。</returns>
        public static NetworkClosedEventArgs Create(INetworkChannel networkChannel)
        {
            NetworkClosedEventArgs networkClosedEventArgs = Reference.ReferenceService.That.GetReference<NetworkClosedEventArgs>();
            networkClosedEventArgs.NetworkChannel = networkChannel;
            return networkClosedEventArgs;
        }

        public override void OnRecycle()
        {
            NetworkChannel = null;
        }
    }
}
