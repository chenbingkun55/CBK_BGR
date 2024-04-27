using System;
using CBK.Framework.Core;
using UnityEngine;

namespace CBK.Framework.ScreenAdaptor
{
    /// <summary>
    /// 编辑器屏幕适配服务。
    /// </summary>
    internal sealed class EditorScreenAdaptorService : IScreenAdaptorService, IInitialize
    {
        public ObservableVariable<Rect> SafeArea { get; } = new ObservableVariable<Rect>();

        public void Initialize()
        {
            _debuggerGameObject = new GameObject("AdaptorDebugger");
            _debugger = _debuggerGameObject.AddComponent<AdaptorDebugger>();
            _debugger.OnDataDirty += () => SafeArea.Value = _debugger.GetSafeArea();

            SafeArea.Value = _debugger.GetSafeArea();
        }

        private GameObject _debuggerGameObject;
        private AdaptorDebugger _debugger;

        private sealed class AdaptorDebugger : MonoBehaviour
        {
            public event Action OnDataDirty;

            public int offsetTop;
            public int offsetBottom;
            public int offsetLeft;
            public int offsetRight;

            private int m_LastOffsetTop;
            private int m_LastOffsetBottom;
            private int m_LastOffsetLeft;
            private int m_LastOffsetRight;

            private int m_LastWidth;
            private int m_LastHeight;

            private void Awake()
            {
                DontDestroyOnLoad(gameObject);

                m_LastWidth = Screen.width;
                m_LastHeight = Screen.height;
            }

            public Rect GetSafeArea()
            {
                var x = offsetLeft;
                var y = offsetTop;
                var w = m_LastWidth - offsetRight - x;
                var h = m_LastHeight - offsetBottom - y;
                return new Rect(x, y, w, h);
            }

            private void Update()
            {
                var modify1 = UpdateInputData();
                var modify2 = UpdateScreenSize();

                if (!modify1 && !modify2)
                    return;

                OnDataDirty?.Invoke();
            }

            private bool UpdateInputData()
            {
                if (offsetTop == m_LastOffsetTop &&
                    offsetBottom == m_LastOffsetBottom &&
                    offsetLeft == m_LastOffsetLeft &&
                    offsetRight == m_LastOffsetRight)
                    return false;

                m_LastOffsetTop = offsetTop = Math.Max(0, offsetTop);
                m_LastOffsetBottom = offsetBottom = Math.Max(0, offsetBottom);
                m_LastOffsetLeft = offsetLeft = Math.Max(0, offsetLeft);
                m_LastOffsetRight = offsetRight = Math.Max(0, offsetRight);
                return true;
            }

            private bool UpdateScreenSize()
            {
                var width = Screen.width;
                var height = Screen.height;

                if (width == m_LastWidth && height == m_LastHeight)
                    return false;

                m_LastWidth = width;
                m_LastHeight = height;
                return true;
            }
        }
    }
}