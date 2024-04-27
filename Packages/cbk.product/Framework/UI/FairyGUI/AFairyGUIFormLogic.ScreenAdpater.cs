using System;
using FairyGUI;
using CBK.Framework.Core;
using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// 界面屏幕适配器接口
    /// </summary>
    public interface IUIFormScreenAdaptor : IDisposable
    {
        /// <summary>
        /// 初始化适配器
        /// </summary>
        /// <param name="contentPane">UI界面对象</param>
        /// <param name="uiRoot">UI分组根结点对象</param>
        void Initialize(GComponent contentPane, GComponent uiRoot);
    }

    /// <summary>
    /// 固定尺寸界面适配器, 界面尺寸不随屏幕尺寸变化, 提供位置适配功能
    /// </summary>
    public sealed class ConstantUIFormScreenAdaptor : IUIFormScreenAdaptor
    {
        private readonly bool m_IsHorizontalCenter;
        private readonly bool m_IsVerticalCenter;

        private GComponent m_ContentPane;
        private GComponent m_UIGroupRoot;

        public ConstantUIFormScreenAdaptor(bool isHorizontalCenter = true, bool isVerticalCenter = true)
        {
            m_IsHorizontalCenter = isHorizontalCenter;
            m_IsVerticalCenter = isVerticalCenter;
        }

        public void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            m_ContentPane = contentPane;
            m_UIGroupRoot = uiRoot;

            var xy = Vector2.zero;

            if (m_IsHorizontalCenter)
                xy.x = (uiRoot.width - contentPane.width) / 2;

            if (m_IsVerticalCenter)
                xy.y = (uiRoot.height - contentPane.height) / 2;

            contentPane.SetXY(xy.x, xy.y, true);

            if (m_IsHorizontalCenter)
                contentPane.AddRelation(uiRoot, RelationType.Center_Center);

            if (m_IsVerticalCenter)
                contentPane.AddRelation(uiRoot, RelationType.Middle_Middle);
        }

        public void Dispose()
        {
            if (m_IsHorizontalCenter)
                m_ContentPane.RemoveRelation(m_UIGroupRoot, RelationType.Center_Center);

            if (m_IsVerticalCenter)
                m_ContentPane.RemoveRelation(m_UIGroupRoot, RelationType.Middle_Middle);
        }
    }

    /// <summary>
    /// 全屏界面适配器, 界面尺寸随屏幕尺寸变化, 提供安全区域适配功能 节点名固定为safeArea
    /// </summary>
    public sealed class FullScreenUIFormScreenAdaptor : IUIFormScreenAdaptor
    {
        private GComponent m_ContentPane;
        private GComponent m_UIRoot;
        private GObject m_SafeAreaObject;
        private IDisposable m_Disposable;

        public void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            m_ContentPane = contentPane;
            m_UIRoot = uiRoot;
            m_SafeAreaObject = contentPane.GetChild("safeArea");

            m_ContentPane.size = m_UIRoot.size;
            m_ContentPane.xy = Vector2.zero;
            m_ContentPane.AddRelation(m_UIRoot, RelationType.Size);

            if (m_SafeAreaObject != null)
            {
                m_SafeAreaObject.relations.ClearAll(); // 清除所有关联 由框架根据安全区域重新设置
                m_Disposable = UIScreenAdaptorService.That.UISafeArea.SubscribeAsDisposable(_OnSafeAreaChanged, true);
            }
        }

        public void Dispose()
        {
            m_ContentPane.RemoveRelation(m_UIRoot, RelationType.Size);

            m_Disposable?.Dispose();
            m_Disposable = null;
        }

        private void _OnSafeAreaChanged(Rect _, Rect safeArea)
        {
            if (m_SafeAreaObject == null)
                return;

            m_SafeAreaObject.SetSize(safeArea.width, safeArea.height);
            m_SafeAreaObject.SetXY(safeArea.x, safeArea.y);
        }
    }

    /// <summary>
    /// 安全区域屏幕界面适配器, 界面尺寸始终与安全区域尺寸相同且位置始终与安全区域位置相同
    /// </summary>
    public sealed class SafeAreaUIFormScreenAdaptor : IUIFormScreenAdaptor
    {
        private GComponent m_ContentPane;
        private IDisposable m_Disposable;
        
        public void Initialize(GComponent contentPane, GComponent uiRoot)
        {
            m_ContentPane = contentPane;
            m_Disposable = UIScreenAdaptorService.That.UISafeArea.SubscribeAsDisposable(_OnSafeAreaChanged, true);
        }

        public void Dispose()
        {
            m_Disposable.Dispose();
            m_Disposable = null;
        }

        private void _OnSafeAreaChanged(Rect _, Rect safeArea)
        {
            m_ContentPane.SetSize(safeArea.width, safeArea.height);
            m_ContentPane.SetXY(safeArea.x, safeArea.y);
        }
    }

    public partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 屏幕适配器
        /// </summary>
        protected abstract IUIFormScreenAdaptor ScreenAdaptor { get; }

        private void _OnInitScreenAdapter()
        {
            var uiGroupHelper = (FairyGUIGroupHelper)UIForm.UIGroup.Helper;
            var uiGroupRoot = uiGroupHelper.GroupRoot;
            
            ScreenAdaptor.Initialize(ContentPane, uiGroupRoot);
        }

        private void _OnRecycleScreenAdapter()
        {
            ScreenAdaptor.Dispose();
        }
    }
}