using UnityEngine;

namespace CBK.Framework.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 添加或获取组件
        /// </summary>
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.AddComponent<T>();
            return component;
        }
    }
}