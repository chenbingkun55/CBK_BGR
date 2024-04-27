using System;

namespace CBK.Framework.Network.MessagePack
{
    /// <summary>
    /// 消息工厂接口
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// 通过消息类型id获取消息类型
        /// </summary>
        Type FindType(int route);
    }
}