using System;
using CatLib.Container;
using CBK.Framework.Core;
using CBK.Framework.Update;
using UnityEngine;

namespace CBK.Framework.ScreenAdaptor
{
    /// <summary>
    /// 基于Unity实现的屏幕适配服务。
    /// </summary>
    internal sealed class UnityScreenAdaptorService : IScreenAdaptorService, IInitialize, IDisposable, IUpdate
    {
        public ObservableVariable<Rect> SafeArea { get; } = new ObservableVariable<Rect>();
        
        [Inject]
        public IUpdateService UpdateService { get; set; }
        
        private int m_Width;
        private int m_Height;

        public void Initialize()
        {
            SafeArea.Value = Screen.safeArea;
            m_Width = Screen.width;
            m_Height = Screen.height;
            
            UpdateService.RegisterUpdate(this);
        }

        public void Dispose()
        {
            UpdateService.UnRegisterUpdate(this);
        }

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (m_Width == Screen.width && m_Height == Screen.height) 
                return;
            
            // 屏幕尺寸发生变化时，更新安全区域
            m_Width = Screen.width;
            m_Height = Screen.height;
            SafeArea.Value = Screen.safeArea;
        }
    }
}