using CBK.Framework.UI.FairyGUI;

namespace CBK.Logic.UI.{{ package_name }}
{
    public partial class {{ name }} : AFairyGUIFormLogic
    {
        #region [界面设置]

        /// <summary>
        /// 界面分组设置
        /// </summary>
        protected override IFairyGUIFormGroupSetting FormGroupSetting { get; } = GameMainFormGroupSetting.That;

        /// <summary>
        /// 屏幕适配器
        /// </summary>
        protected override IUIFormScreenAdaptor ScreenAdaptor { get; } = new FullScreenUIFormScreenAdaptor();

        #endregion

        private void OnInitialize(object userData)
        {
        }

        protected override void OnRecycle()
        {
        }
    }
}