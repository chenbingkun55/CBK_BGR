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
    /// 用户自定义网络错误事件。
    /// </summary>
    public sealed class NetworkCustomErrorEventArgs : AEventArgs
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
        /// 获取用户自定义错误数据。
        /// </summary>
        public object CustomErrorData
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建用户自定义网络错误事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>创建的用户自定义网络错误事件。</returns>
        public static NetworkCustomErrorEventArgs Create(INetworkChannel networkChannel, object customErrorData)
        {
            NetworkCustomErrorEventArgs networkCustomErrorEventArgs = Reference.ReferenceService.That.GetReference<NetworkCustomErrorEventArgs>();
            networkCustomErrorEventArgs.NetworkChannel = networkChannel;
            networkCustomErrorEventArgs.CustomErrorData = customErrorData;
            return networkCustomErrorEventArgs;
        }

        public override void OnRecycle()
        {
            NetworkChannel = null;
            CustomErrorData = null;
        }
    }
}
