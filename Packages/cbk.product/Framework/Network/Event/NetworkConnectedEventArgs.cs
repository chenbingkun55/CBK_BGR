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
    /// 网络连接成功事件。
    /// </summary>
    public sealed class NetworkConnectedEventArgs : AEventArgs
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
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建网络连接成功事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>创建的网络连接成功事件。</returns>
        public static NetworkConnectedEventArgs Create(INetworkChannel networkChannel, object userData)
        {
            NetworkConnectedEventArgs networkConnectedEventArgs = Reference.ReferenceService.That.GetReference<NetworkConnectedEventArgs>();
            networkConnectedEventArgs.NetworkChannel = networkChannel;
            networkConnectedEventArgs.UserData = userData;
            return networkConnectedEventArgs;
        }

        public override void OnRecycle()
        {
            NetworkChannel = null;
            UserData = null;
        }
    }
}
