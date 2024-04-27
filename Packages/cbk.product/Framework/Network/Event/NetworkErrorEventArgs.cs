//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Net.Sockets;
using CBK.Framework.Event;
using CBK.Framework.Reference;

namespace CBK.Framework.Network
{
    /// <summary>
    /// 网络错误事件。
    /// </summary>
    public sealed class NetworkErrorEventArgs : AEventArgs
    {
        /// <summary>
        /// 获取网络频道。
        /// </summary>
        public INetworkChannel NetworkChannel { get; private set; }

        /// <summary>
        /// 获取错误码。
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// 获取 Socket 错误码。
        /// </summary>
        public SocketError SocketErrorCode { get; private set; }

        /// <summary>
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 创建网络错误事件。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        /// <param name="errorCode">错误码。</param>
        /// <param name="socketErrorCode">Socket 错误码。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <returns>创建的网络错误事件。</returns>
        public static NetworkErrorEventArgs Create(INetworkChannel networkChannel, int errorCode, SocketError socketErrorCode, string errorMessage)
        {
            NetworkErrorEventArgs networkErrorEventArgs = Reference.ReferenceService.That.GetReference<NetworkErrorEventArgs>();
            networkErrorEventArgs.NetworkChannel = networkChannel;
            networkErrorEventArgs.ErrorCode = errorCode;
            networkErrorEventArgs.SocketErrorCode = socketErrorCode;
            networkErrorEventArgs.ErrorMessage = errorMessage;
            return networkErrorEventArgs;
        }

        public override void OnRecycle()
        {
            NetworkChannel = null;
            ErrorCode = (int)FrameworkErrorCode.NetworkUnknown;
            SocketErrorCode = SocketError.Success;
            ErrorMessage = null;
        }
    }
}