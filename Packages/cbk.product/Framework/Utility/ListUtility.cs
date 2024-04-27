using System.Collections.Generic;

namespace CBK.Framework.Utility
{
    public static class ListUtility
    {
        /// <summary>
        /// 从列表中随机一个元素
        /// </summary>
        public static T Random<T>(this IList<T> buffer)
        {
            if (buffer == null || buffer.Count == 0)
                return default;
            
            return buffer[UnityEngine.Random.Range(0, buffer.Count)];
        }
    }
}