using FairyGUI;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI 界面逻辑基类。
    /// </summary>
    public abstract partial class AFairyGUIFormLogic
    {
        /// <summary>
        /// 获取或设置界面名称。
        /// </summary>
        public string Name
        {
            get => ContentPane.displayObject.name;
            set => ContentPane.displayObject.name = value;
        }

        /// <summary>
        /// 界面分组名称
        /// </summary>
        public string GroupName => FormGroupSetting.GroupName;

        /// <summary>
        /// 获取界面是否暂停被覆盖的界面。
        /// </summary>
        public bool PauseCoveredUIForm => FormGroupSetting.PauseCoveredUIForm;

        /// <summary>
        /// 获取是否显示遮罩层。
        /// </summary>
        public virtual bool DisplayMaskLayer => false;

        /// <summary>
        /// 界面服务实例
        /// </summary>
        protected IUIService UIService { get; private set; }

        /// <summary>
        /// 获取界面。
        /// </summary>
        protected IUIForm UIForm { get; private set; }

        /// <summary>
        /// 获取FairyGUI组件实例
        /// </summary>
        protected GComponent ContentPane { get; private set; }

        /// <summary>
        /// 界面分组设置
        /// </summary>
        protected abstract IFairyGUIFormGroupSetting FormGroupSetting { get; }

        /// <summary>
        /// 关闭界面
        /// </summary>
        protected void CloseForm()
        {
            UIService.CloseUIForm(UIForm.UIKey);
        }
    }
}