using System.Threading;
using CatLib.Container;
using Cysharp.Threading.Tasks;
using CBK.Framework.Scene;
using UnityEngine.SceneManagement;

namespace CBK.Framework.Business
{
    /// <summary>
    /// 场景加载控制器
    /// </summary>
    public sealed class SceneLoadController : AViewController, ISequenceLoadable
    {
        [Inject]
        public ISceneService SceneService { get; set; }

        private readonly string m_SceneName;
        private readonly LoadSceneMode m_LoadSceneMode;
        private readonly bool m_ActiveOnLoad;
        private readonly int m_Priority;

        public SceneLoadController(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activeOnLoad = true, int priority = 100)
        {
            m_SceneName = sceneName;
            m_ActiveOnLoad = activeOnLoad;
            m_LoadSceneMode = loadSceneMode;
            m_Priority = priority;
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnDispose()
        {
        }

        public UniTask LoadAsync(CancellationToken token)
        {
            return SceneService.LoadSceneAsync(m_SceneName, m_LoadSceneMode, m_ActiveOnLoad, m_Priority);
        }
    }
}