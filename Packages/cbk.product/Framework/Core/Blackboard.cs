using System.Collections.Generic;
using CBK.Framework.Reference;

namespace CBK.Framework.Core
{
    /// <summary>
    /// 数据黑板
    /// </summary>
    public sealed class Blackboard : AReference
    {
        /// <summary>
        /// 是否包含指定的key
        /// </summary>
        public bool Contains(string key)
        {
            return m_ObjectValues.ContainsKey(key) || m_NumberValues.ContainsKey(key);
        }

        /// <summary>
        /// 移除指定的key
        /// </summary>
        public void Remove(string key)
        {
            m_ObjectValues.Remove(key);
            m_NumberValues.Remove(key);
        }
        
        /// <summary>
        /// 获取int类型的值
        /// </summary>
        public int GetInt(string key)
        {
            return unchecked((int) GetNumber(key));
        }
        
        /// <summary>
        /// 获取uint类型的值
        /// </summary>
        public uint GetUInt(string key)
        {
            return unchecked((uint) GetNumber(key));
        }
        
        /// <summary>
        /// 获取long类型的值
        /// </summary>
        public long GetLong(string key)
        {
            return unchecked((long) GetNumber(key));
        }
        
        /// <summary>
        /// 获取ulong类型的值
        /// </summary>
        public ulong GetULong(string key)
        {
            return GetNumber(key);
        }
        
        /// <summary>
        /// 获取string类型的值
        /// </summary>
        public string GetString(string key)
        {
            return m_ObjectValues.TryGetValue(key, out var value) ? value as string : null;
        }
        
        /// <summary>
        /// 获取指定类型的值
        /// </summary>
        public T GetObject<T>(string key) where T : class
        {
            return m_ObjectValues.TryGetValue(key, out var value) ? value as T : null;
        }
        
        /// <summary>
        /// 设置int类型的值
        /// </summary>
        public void SetInt(string key, int value)
        {
            m_NumberValues[key] = unchecked((uint) value);
        }
        
        /// <summary>
        /// 设置uint类型的值
        /// </summary>
        public void SetUInt(string key, uint value)
        {
            m_NumberValues[key] = value;
        }
        
        /// <summary>
        /// 设置long类型的值
        /// </summary>
        public void SetLong(string key, long value)
        {
            m_NumberValues[key] = unchecked((ulong) value);
        }
        
        /// <summary>
        /// 设置ulong类型的值
        /// </summary>
        public void SetULong(string key, ulong value)
        {
            m_NumberValues[key] = value;
        }
        
        /// <summary>
        /// 设置string类型的值
        /// </summary>
        public void SetString(string key, string value)
        {
            m_ObjectValues[key] = value;
        }
        
        /// <summary>
        /// 设置指定类型的值
        /// </summary>
        public void SetObject<T>(string key, T value) where T : class
        {
            m_ObjectValues[key] = value;
        }
        
        private ulong GetNumber(string key)
        {
            return m_NumberValues.TryGetValue(key, out var value) ? value : 0;
        }
        
        public override void OnRecycle()
        {
            m_NumberValues.Clear();
            m_ObjectValues.Clear();
        }
        
        private readonly Dictionary<string, ulong> m_NumberValues = new Dictionary<string, ulong>();
        private readonly Dictionary<string, object> m_ObjectValues = new Dictionary<string, object>();
    }
}