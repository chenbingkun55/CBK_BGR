using System.Collections.Generic;
using CBK.Framework.Collections;
using CBK.Framework.Core;
using CBK.Framework.Reference;

// ReSharper disable UseStringInterpolation

namespace CBK.Framework.UI
{
    internal partial class UIServiceImpl
    {
        /// <summary>
        /// 界面组。
        /// </summary>
        private sealed partial class UIGroup : IUIGroup
        {
            private readonly string m_Name;
            private readonly int m_Depth;
            private readonly IUIGroupHelper m_UIGroupHelper;
            private readonly GameFrameworkLinkedList<UIFormInfo> m_UIFormInfos;
            private LinkedListNode<UIFormInfo> m_CachedNode;

            /// <summary>
            /// 初始化界面组的新实例。
            /// </summary>
            /// <param name="name">界面组名称。</param>
            /// <param name="depth">界面组深度。</param>
            /// <param name="uiGroupHelper">界面组辅助器。</param>
            public UIGroup(string name, int depth, IUIGroupHelper uiGroupHelper)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new System.Exception("UI group name is invalid.");
                }

                if (uiGroupHelper == null)
                {
                    throw new System.Exception("UI group helper is invalid.");
                }

                m_Name = name;
                m_UIGroupHelper = uiGroupHelper;
                m_UIFormInfos = new GameFrameworkLinkedList<UIFormInfo>();
                m_CachedNode = null;
                m_Depth = depth;
            }

            /// <summary>
            /// 获取界面组名称。
            /// </summary>
            public string Name => m_Name;

            /// <summary>
            /// 获取界面组中界面数量。
            /// </summary>
            public int UIFormCount => m_UIFormInfos.Count;

            /// <summary>
            /// 获取当前界面。
            /// </summary>
            public IUIForm CurrentUIForm => m_UIFormInfos.First?.Value.UIForm;

            /// <summary>
            /// 获取界面组辅助器。
            /// </summary>
            public IUIGroupHelper Helper => m_UIGroupHelper;

            /// <summary>
            /// 界面组轮询。
            /// </summary>
            /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
            /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
                while (current != null)
                {
                    if (current.Value.Paused)
                    {
                        break;
                    }

                    m_CachedNode = current.Next;
                    current.Value.UIForm.OnUpdate(elapseSeconds, realElapseSeconds);
                    current = m_CachedNode;
                    m_CachedNode = null;
                }
            }

            /// <summary>
            /// 界面组中是否存在界面。
            /// </summary>
            /// <param name="uiKey">界面键值。</param>
            /// <returns>界面组中是否存在界面。</returns>
            public bool HasUIForm(string uiKey)
            {
                if (string.IsNullOrEmpty(uiKey))
                {
                    throw new System.Exception("UI form asset name is invalid.");
                }

                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    if (uiFormInfo.UIForm.UIKey == uiKey)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// 从界面组中获取界面。
            /// </summary>
            /// <param name="uiKey">界面键值。</param>
            /// <returns>要获取的界面。</returns>
            public IUIForm GetUIForm(string uiKey)
            {
                if (string.IsNullOrEmpty(uiKey))
                {
                    throw new System.Exception("UI form asset name is invalid.");
                }

                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    if (uiFormInfo.UIForm.UIKey == uiKey)
                    {
                        return uiFormInfo.UIForm;
                    }
                }

                return null;
            }

            /// <summary>
            /// 从界面组中获取所有界面。
            /// </summary>
            /// <returns>界面组中的所有界面。</returns>
            public IUIForm[] GetAllUIForms()
            {
                List<IUIForm> results = new List<IUIForm>();
                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    results.Add(uiFormInfo.UIForm);
                }

                return results.ToArray();
            }

            /// <summary>
            /// 从界面组中获取所有界面。
            /// </summary>
            /// <param name="results">界面组中的所有界面。</param>
            public void GetAllUIForms(List<IUIForm> results)
            {
                if (results == null)
                {
                    throw new System.Exception("Results is invalid.");
                }

                results.Clear();
                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    results.Add(uiFormInfo.UIForm);
                }
            }

            /// <summary>
            /// 往界面组增加界面。
            /// </summary>
            /// <param name="uiForm">要增加的界面。</param>
            public void AddUIForm(IUIForm uiForm)
            {
                m_UIFormInfos.AddFirst(UIFormInfo.Create(uiForm));
            }

            /// <summary>
            /// 从界面组移除界面。
            /// </summary>
            /// <param name="uiForm">要移除的界面。</param>
            public void RemoveUIForm(IUIForm uiForm)
            {
                UIFormInfo uiFormInfo = GetUIFormInfo(uiForm);
                if (uiFormInfo == null)
                {
                    throw new System.Exception(string.Format("Can not find UI form info for '{0}'.", uiForm.UIKey));
                }

                if (!uiFormInfo.Covered)
                {
                    uiFormInfo.Covered = true;
                    uiForm.OnCover();
                }

                if (!uiFormInfo.Paused)
                {
                    uiFormInfo.Paused = true;
                    uiForm.OnPause();
                }

                if (m_CachedNode != null && m_CachedNode.Value.UIForm == uiForm)
                {
                    m_CachedNode = m_CachedNode.Next;
                }

                if (!m_UIFormInfos.Remove(uiFormInfo))
                {
                    throw new System.Exception(string.Format("UI group '{0}' not exists specified UI form '[{1}]'.", m_Name, uiForm.UIKey));
                }

                uiFormInfo.Recycle();
            }

            /// <summary>
            /// 激活界面。
            /// </summary>
            /// <param name="uiForm">要激活的界面。</param>
            public void RefocusUIForm(IUIForm uiForm)
            {
                UIFormInfo uiFormInfo = GetUIFormInfo(uiForm);
                if (uiFormInfo == null)
                {
                    throw new System.Exception("Can not find UI form info.");
                }

                m_UIFormInfos.Remove(uiFormInfo);
                m_UIFormInfos.AddFirst(uiFormInfo);
            }

            /// <summary>
            /// 刷新界面组。
            /// </summary>
            public void Refresh()
            {
                LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
                bool pause = false;
                bool cover = false;
                int depth = UIFormCount;
                UIFormInfo maskLayerUIFormInfo = null;
                
                while (current != null && current.Value != null)
                {
                    LinkedListNode<UIFormInfo> next = current.Next;
                    current.Value.UIForm.OnDepthChanged(m_Depth, depth--);
                    if (current.Value == null)
                    {
                        return;
                    }

                    if (pause)
                    {
                        if (!current.Value.Covered)
                        {
                            current.Value.Covered = true;
                            current.Value.UIForm.OnCover();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }

                        if (!current.Value.Paused)
                        {
                            current.Value.Paused = true;
                            current.Value.UIForm.OnPause();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (current.Value.Paused)
                        {
                            current.Value.Paused = false;
                            current.Value.UIForm.OnResume();
                            if (current.Value == null)
                            {
                                return;
                            }
                        }

                        if (current.Value.UIForm.PauseCoveredUIForm)
                        {
                            pause = true;
                        }

                        if (cover)
                        {
                            if (!current.Value.Covered)
                            {
                                current.Value.Covered = true;
                                current.Value.UIForm.OnCover();
                                if (current.Value == null)
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (current.Value.Covered)
                            {
                                current.Value.Covered = false;
                                current.Value.UIForm.OnReveal();
                                if (current.Value == null)
                                {
                                    return;
                                }
                            }

                            cover = true;
                        }
                        
                        if (maskLayerUIFormInfo == null && current.Value.UIForm.DisplayMaskLayer)
                        {
                            maskLayerUIFormInfo = current.Value;
                        }
                    }

                    current = next;
                }
                
                Helper.SetMaskLayerForm(maskLayerUIFormInfo?.UIForm);
            }

            internal void InternalGetAllUIForms(List<IUIForm> results)
            {
                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    results.Add(uiFormInfo.UIForm);
                }
            }

            private UIFormInfo GetUIFormInfo(IUIForm uiForm)
            {
                if (uiForm == null)
                {
                    throw new System.Exception("UI form is invalid.");
                }

                foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
                {
                    if (uiFormInfo.UIForm == uiForm)
                    {
                        return uiFormInfo;
                    }
                }

                return null;
            }
        }
    }
}