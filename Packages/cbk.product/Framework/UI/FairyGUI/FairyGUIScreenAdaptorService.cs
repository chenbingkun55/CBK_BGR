using System;
using CatLib.Container;
using FairyGUI;
using CBK.Framework.Core;
using CBK.Framework.ScreenAdaptor;
using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 界面适配器服务。
    /// </summary>
    internal sealed class FairyGUIScreenAdaptorService : IUIScreenAdaptorService, IInitialize, IDisposable
    {
        [Inject]
        public IScreenAdaptorService ScreenAdaptorService { get; set; }
        
        public ObservableVariable<Vector2Int> UIScreenSize { get; } = new ObservableVariable<Vector2Int>();
        public ObservableVariable<Rect> UISafeArea { get; } = new ObservableVariable<Rect>();

        public void Initialize()
        {
            ScreenAdaptorService.SafeArea.Subscribe(OnSafeAreaChanged);
            GRoot.inst.onSizeChanged.Add(OnSizeChanged);
            UpdateSize();
        }

        public void Dispose()
        {
            GRoot.inst.onSizeChanged.Remove(OnSizeChanged);
            ScreenAdaptorService.SafeArea.Unsubscribe(OnSafeAreaChanged);
        }

        private void OnSafeAreaChanged(Rect _, Rect __)
        {
            UpdateSize();
        }

        private void OnSizeChanged()
        {
            UpdateSize();
        }
        
        private void UpdateSize()
        {
            var safeArea = ScreenAdaptorService.SafeArea.Value;
            var factor = 1f / GRoot.contentScaleFactor;
            safeArea.x = Mathf.CeilToInt(safeArea.x * factor);
            safeArea.y = Mathf.CeilToInt(safeArea.y * factor);
            safeArea.width = Mathf.CeilToInt(safeArea.width * factor);
            safeArea.height = Mathf.CeilToInt(safeArea.height * factor);
            
            UIScreenSize.Value = new Vector2Int((int)GRoot.inst.width, (int)GRoot.inst.height);
            UISafeArea.Value = safeArea;
        }
    }
}