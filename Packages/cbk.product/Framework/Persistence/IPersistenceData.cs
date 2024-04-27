using System;

namespace CBK.Framework.Persistence
{
    /// <summary>
    /// 持久化数据实例
    /// </summary>
    public interface IPersistenceData : IDisposable
    {
        /// <summary>
        /// 是否有指定键值的数据
        /// </summary>
        bool HasKey(string key);
        
        /// <summary>
        /// 删除指定键值的数据
        /// </summary>
        void DeleteKey(string key);
        
        /// <summary>
        /// 删除所有数据
        /// </summary>
        void DeleteAll();
        
        /// <summary>
        /// 设置Int类型的数据
        /// </summary>
        void SetInt(string key, int value);
        
        /// <summary>
        /// 获取Int类型的数据
        /// </summary>
        int GetInt(string key, int defaultValue = 0);
        
        /// <summary>
        /// 设置Float类型的数据
        /// </summary>
        void SetFloat(string key, float value);
        
        /// <summary>
        /// 获取Float类型的数据
        /// </summary>
        float GetFloat(string key, float defaultValue = 0f);
        
        /// <summary>
        /// 设置String类型的数据
        /// </summary>
        void SetString(string key, string value);
        
        /// <summary>
        /// 获取String类型的数据
        /// </summary>
        string GetString(string key, string defaultValue = "");
    }
}