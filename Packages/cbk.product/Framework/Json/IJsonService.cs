namespace CBK.Framework.Json
{
    /// <summary>
    /// Json服务接口。
    /// </summary>
    public interface IJsonService
    {
        /// <summary>
        /// 分配Json核心。不用时需要调用<see cref="IJsonCore.Dispose"/>释放。
        /// </summary>
        IJsonCore Allocate();
        
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