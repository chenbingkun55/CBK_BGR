using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CBK.Framework.ObjectPool
{
    /// <summary>
    /// 对象池核心接口
    /// </summary>
    public interface IObjectPoolCore
    {
        /// <summary>
        /// 设置指定资源的对象池中最大实例数量 超过该数量的实例将被销毁
        /// </summary>
        void SetLimit(string assetKey, int limit);
        
        /// <summary>
        /// 从对象池中获取指定资源的实例
        /// </summary>
        GameObject Instantiate(string assetKey);
        
        /// <summary>
        /// 从对象池中获取指定资源的实例
        /// </summary>
        UniTask<GameObject> InstantiateAsync(string assetKey);
        
        /// <summary>
        /// 释放指定实例
        /// </summary>
        void Release(GameObject instance);
        
        /// <summary>
        /// 销毁所有未使用的实例
        /// </summary>
        void DestroyUnusedObjects();
    }
}