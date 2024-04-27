//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System.Net.Sockets;

namespace CBK.Framework.Network
{
    internal sealed partial class NetworkServiceImpl
    {
        private sealed class ConnectState
        {
            public Socket Socket { get; }
            public object UserData { get; }
            
            public ConnectState(Socket socket, object userData)
            {
                Socket = socket;
                UserData = userData;
            }
        }
    }
}
