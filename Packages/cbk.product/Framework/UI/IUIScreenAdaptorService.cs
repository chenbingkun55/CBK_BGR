using CBK.Framework.Core;
using UnityEngine;

namespace CBK.Framework.UI
{
    /// <summary>
    /// UI 界面适配器服务接口。
    /// </summary>
    public interface IUIScreenAdaptorService
    {
        /// <summary>
        /// UI 屏幕大小。(逻辑尺寸)
        /// </summary>
        ObservableVariable<Vector2Int> UIScreenSize { get; }
        
        /// <summary>
        /// UI 安全区域。(逻辑尺寸)
        /// </summary>
        ObservableVariable<Rect> UISafeArea { get; }
    }
}