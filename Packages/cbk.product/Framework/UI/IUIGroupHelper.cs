using System;

namespace CBK.Framework.UI
{
    /// <summary>
    /// 界面组辅助器接口。
    /// </summary>
    public interface IUIGroupHelper : IDisposable
    {
        /// <summary>
        /// 初始化界面组辅助器。
        /// </summary>
        /// <param name="groupName">分组名</param>
        void Initialize(string groupName);
        
        /// <summary>
        /// 设置界面组深度。
        /// </summary>
        /// <param name="depth">界面组深度。</param>
        void SetDepth(int depth);

        /// <summary>
        /// 设置遮罩层关联的界面。
        /// </summary>
        /// <param name="uiForm">界面实例。</param>
        void SetMaskLayerForm(IUIForm uiForm);
    }
}