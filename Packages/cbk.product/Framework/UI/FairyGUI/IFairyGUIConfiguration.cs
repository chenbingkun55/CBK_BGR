using UnityEngine;

namespace CBK.Framework.UI.FairyGUI
{
    public enum ScreenMatchMode
    {
        [Header("固定分辨率")]
        Constant,
        [Header("匹配宽度")]
        MatchWidth,
        [Header("匹配高度")]
        MatchHeight,
        [Header("匹配宽度或高度")]
        MatchWidthOrHeight,
        [Header("固定宽度并限制高度范围")]
        ConstantWidthAndLimitHeight,
        [Header("固定高度并限制宽度范围")]
        ConstantHeightAndLimitWidth,
    }
    
    /// <summary>
    /// FairyGUI 配置接口。
    /// </summary>
    public interface IFairyGUIConfiguration
    {
        /// <summary>
        /// 获取设计分辨率。
        /// </summary>
        Vector2Int DesignResolution { get; }
        
        /// <summary>
        /// 屏幕适配模式。
        /// </summary>
        ScreenMatchMode ScreenMatchMode { get; }
        
        /// <summary>
        /// 遮罩层颜色。
        /// </summary>
        Color MaskLayerColor { get; }
        
        /// <summary>
        /// 遮罩层模糊尺寸。0代表不开启模糊。
        /// </summary>
        float MaskLayerBlurSize { get; }
        
        /// <summary>
        /// 遮罩层降级采样等级 右移位数。
        /// </summary>
        int MaskLayerDownSample { get; }
    }
}