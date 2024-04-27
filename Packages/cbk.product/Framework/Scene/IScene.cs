using Cysharp.Threading.Tasks;

namespace CBK.Framework.Scene
{
    /// <summary>
    /// 场景实例接口
    /// </summary>
    public interface IScene
    {
        /// <summary>
        /// Unity场景实例
        /// </summary>
        UnityEngine.SceneManagement.Scene UnityScene { get; }
        
        /// <summary>
        /// 是否是主场景
        /// </summary>
        bool IsMainScene { get; }
        
        /// <summary>
        /// 激活场景
        /// </summary>
        bool Activate();
        
        /// <summary>
        /// 卸载场景(仅限子场景使用)
        /// </summary>
        /// <returns></returns>
        UniTask Unload();
    }
}