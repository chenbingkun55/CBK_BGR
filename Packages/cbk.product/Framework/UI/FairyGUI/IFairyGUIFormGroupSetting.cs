namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 界面分组设置接口。
    /// </summary>
    public interface IFairyGUIFormGroupSetting
    {
        /// <summary>
        /// 界面分组名称
        /// </summary>
        string GroupName { get; }
        
        /// <summary>
        /// 获取界面是否暂停被覆盖的界面。
        /// </summary>
        bool PauseCoveredUIForm { get; }
    }
}