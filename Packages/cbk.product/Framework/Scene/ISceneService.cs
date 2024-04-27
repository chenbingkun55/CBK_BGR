using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CBK.Framework.Scene
{
    /// <summary>
    /// 场景管理服务
    /// </summary>
    public interface ISceneService
    {
        /// <summary>
        /// 异步加载场景
        /// </summary>
        UniTask<IScene> LoadSceneAsync(string assetKey, LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100);
    }
}