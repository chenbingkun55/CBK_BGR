using System.Collections.Generic;

namespace Libraries.FairyGUI.Runtime.Utils
{
    public static class SerializeUtils
    {
        /// <summary>
        /// 反序列化委托
        /// </summary>
        public delegate Dictionary<string, object> DeserializeDelegate(string str);

        public static DeserializeDelegate DeserializeFunc;
        
        public static Dictionary<string, object> Deserialize(string str)
        {
            return !string.IsNullOrEmpty(str) ? DeserializeFunc?.Invoke(str) : null;
        }
    }
}