using System;
using CBK.Framework.Event;
using CBK.Framework.Request;
using CBK.Framework.Rpc;

namespace CBK.Framework.Network.Rpc
{
    /// <summary>
    /// Rpc消息包拦截器
    /// </summary>
    public sealed class RpcPacketInterceptor : IEventInterceptor
    {
        private readonly IRpcService m_RpcService;

        public RpcPacketInterceptor(IRpcService mRpcService)
        {
            m_RpcService = mRpcService;
        }

        public bool Intercept(string eventName, object sender, EventArgs e)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (e is not IResponse response)
                return false;
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (e is not IRpcPacket packet)
                return false;
            
            if (packet.RpcId <= 0) 
                return false;
            
            m_RpcService.SetResponse(packet.RpcId, response);
            return true;
        }
    }
}