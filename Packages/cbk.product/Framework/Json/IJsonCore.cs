using System;

namespace CBK.Framework.Json
{
    /// <summary>
    /// Json核心接口
    /// </summary>
    public interface IJsonCore : IDisposable
    {
        /// <summary>
        /// 序列化对象为Json字符串。
        /// </summary>
        string Serialize<T>(T obj);
        
        /// <summary>
        /// 反序列化Json字符串为对象。
        /// </summary>
        T Deserialize<T>(string json);
    }
}