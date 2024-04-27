using CBK.Framework.Reference;

namespace CBK.Framework.UI
{
    internal partial class UIServiceImpl
    {
        private sealed partial class UIGroup
        {
            /// <summary>
            /// 界面组界面信息。
            /// </summary>
            private sealed class UIFormInfo : AReference
            {
                public UIFormInfo()
                {
                    UIForm = null;
                    Paused = false;
                    Covered = false;
                }

                public IUIForm UIForm { get; private set; }

                public bool Paused { get; set; }

                public bool Covered { get; set; }

                public static UIFormInfo Create(IUIForm uiForm)
                {
                    if (uiForm == null)
                    {
                        throw new System.Exception("UI form is invalid.");
                    }

                    var uiFormInfo = Reference.ReferenceService.That.GetReference<UIFormInfo>();
                    uiFormInfo.UIForm = uiForm;
                    uiFormInfo.Paused = true;
                    uiFormInfo.Covered = true;
                    return uiFormInfo;
                }

                public override void OnRecycle()
                {
                    UIForm = null;
                    Paused = false;
                    Covered = false;
                }
            }
        }
    }
}