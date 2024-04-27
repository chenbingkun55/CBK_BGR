using System;
using Cysharp.Threading.Tasks;
using CBK.Framework.Core;
using UnityEngine.SceneManagement;
using YooAsset;

namespace CBK.Framework.Scene.YooAsset
{
    /// <summary>
    /// 基于YooAsset的场景服务
    /// </summary>
    internal sealed class YooAssetSceneService : ISceneService, IInitialize, IDisposable
    {
        public void Initialize()
        {
        }

        public void Dispose()
        {
        }

        public async UniTask<IScene> LoadSceneAsync(string assetKey, LoadSceneMode sceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            var handle = YooAssets.LoadSceneAsync(assetKey, sceneMode, activateOnLoad, priority);
            
            await handle.ToUniTask();

            return handle.IsValid ? new YooAssetScene(handle) : null;
        }

        private sealed class YooAssetScene : IScene
        {
            private readonly SceneOperationHandle m_Handle;

            public UnityEngine.SceneManagement.Scene UnityScene => m_Handle.SceneObject;

            public bool IsMainScene => m_Handle.IsMainScene();

            public YooAssetScene(SceneOperationHandle handle)
            {
                m_Handle = handle;
            }

            public bool Activate()
            {
                return m_Handle.ActivateScene();
            }

            public UniTask Unload()
            {
                return m_Handle.UnloadAsync().ToUniTask();
            }
        }
    }
}