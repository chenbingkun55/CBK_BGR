using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CBK.Framework.UI.FairyGUI
{
    /// <summary>
    /// FairyGUI界面扩展。
    /// </summary>
    public static class FairyGUIFormExtensions
    {
        /// <summary>
        /// 打开FairyGUI界面。
        /// </summary>
        public static UniTask<int> OpenUIForm<T>(this IUIService service, object userdata = null, CancellationToken cancellationToken = default) where T : AFairyGUIFormLogic
        {
            var attribute = typeof(T).GetCustomAttribute<FairyGUIFormAttribute>();
            if (attribute == null)
            {
                return UniTask.FromResult((int)FrameworkErrorCode.UIFormUndefined);
            }
            
            return service.OpenUIForm(attribute.UIKey, userdata, cancellationToken);
        }

        /// <summary>
        /// 关闭FairyGUI界面。
        /// </summary>
        public static void CloseUIForm<T>(this IUIService service, object userdata = null) where T : AFairyGUIFormLogic
        {
            var attribute = typeof(T).GetCustomAttribute<FairyGUIFormAttribute>();
            if (attribute == null)
                return;
            
            service.CloseUIForm(attribute.UIKey, userdata);
        }
    }
}